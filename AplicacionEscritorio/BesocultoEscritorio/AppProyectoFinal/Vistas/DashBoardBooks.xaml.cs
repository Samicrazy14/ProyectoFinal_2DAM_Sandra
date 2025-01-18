using AppProyectoFinal.Data;
using Syncfusion.PdfToImageConverter;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppProyectoFinal.Vistas;

public partial class DashBoardBooks : ContentPage, INotifyPropertyChanged
{
    //Manejadores de BBDD y mensajes
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    private FirebaseStorageManager firebaseStorageManager = new FirebaseStorageManager();
    private MessageManager messageManager;
    private LogManager logManager = new LogManager();
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    //Variables
    private HttpClient httpClient;
    private int currentPage = 1;

    //Colleccion de datos
    public ObservableCollection<ModelPdf> AllBooks { get; set; }
    public ObservableCollection<ModelPdf> rawBooks { get; set; }


    private bool _isBookToggled = false;
    public bool isBookToggled
    {
        get => _isBookToggled;
        set
        {
            if (_isBookToggled != value)
            {
                _isBookToggled = value;
                OnPropertyChanged(nameof(isBookToggled));
            }
        }
    }

    private bool _openOption = false;
    public bool openOption
    {
        get => _openOption;
        set
        {
            if (_openOption != value)
            {
                _openOption = value;
                OnPropertyChanged(nameof(openOption));
            }
        }
    }

    public new event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DashBoardBooks()
	{
		InitializeComponent();

        // Inicializa colecciones y dependencias
        AllBooks = new ObservableCollection<ModelPdf>();
        rawBooks = new ObservableCollection<ModelPdf>();
        httpClient = new HttpClient();
        messageManager = new MessageManager(mainPage, mainlayout);
        BindingContext = this;

#if WINDOWS
          Microsoft.Maui.Handlers.SwitchHandler.Mapper.AppendToMapping("NoLabel", (handler, View) =>
          {
              handler.PlatformView.OnContent = null;
              handler.PlatformView.OffContent = null;

              // Add this to remove the padding around the switch as well
              // handler.PlatformView.MinWidth = 0;
          });
#endif
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de gestión de libros para administrador");
        AllBooksCollection.SelectedItem = null;
        UpdateBooksCollection();
    }

    /// <summary>
    /// Metodo que filtra los libros por título usando el texto introducido en la barra de busqueda.
    /// </summary>
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = RemoveDiacritics(e.NewTextValue?.ToLower() ?? string.Empty);
        AllBooks.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var book in rawBooks)
            {
                AllBooks.Add(book);
            }
        }
        else
        {
            // Filtra los elementos de rawBooks por el título
            foreach (var book in rawBooks.Where(b => RemoveDiacritics(b.title.ToLower()).Contains(searchText)))
            {
                AllBooks.Add(book);
            }
        }
    }

    /// <summary>
    /// Metodo que normaliza el texto eliminando tildes.
    /// <list type="number">
    /// <item><param name="text">Texto a normalizar</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve el valor de entrada normalizado.</returns>
    private string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var normalizedText = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var c in normalizedText)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }

    /// <summary>
    /// Carga todos los libros desde Books en Firebase y los añade a la colección AllBooks.
    /// </summary>
    private async Task LoadBooks()
    {
        try
        {
            var books = await firebaseDatabaseManager.GetAllCollectionAsync<ModelPdf>("Books");

            foreach (var book in books)
            {
                try
                {
                    var bookData = book.Object;

                    bookData.title = string.IsNullOrEmpty(bookData.title)
                        ? LocalizationResourceManager.Instance["txtNoTitulo"].ToString() ?? "Título desconocido"
                        : bookData.title;
                    bookData.description = string.IsNullOrEmpty(bookData.description) 
                        ? LocalizationResourceManager.Instance["txtNoDescripcion"].ToString() ?? "Sin descripción"
                        : bookData.description;
                    //Generamos datos extra para la visualizacion del libro
                    await LoadCategoryForBook(bookData);
                    await LoadPdfImage(bookData);

                    AllBooks.Add(bookData);
                    rawBooks.Add(bookData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar libro: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar libros de firebase: {ex.Message}");
        }
    }

    /// <summary>
    /// Carga la categoría de un libro específico consultando Categories en Firebase.
    /// <list type="number">
    /// <item><param name="book">Instancia de ModelPdf que representa el libro</param></item>
    /// </list>
    /// </summary>
    private async Task LoadCategoryForBook(ModelPdf book)
    {
        try
        {
            var category = await firebaseDatabaseManager.GetItemByIdAsync<ModelCategory>("Categories", book.categoryId);

            if (category != null)
            {
                book.Category = category.Category;
            }
            else
            {
                book.Category = LocalizationResourceManager.Instance["txtNoCategoria"].ToString() ?? "Categoría desconocida";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar categoría para libro {book.id}: {ex.Message}");
            book.Category = LocalizationResourceManager.Instance["txtNoCategoria"].ToString() ?? "Categoría desconocida";
        }
    }

    /// <summary>
    /// Genera y establece la imagen de portada del PDF para el libro.
    /// <list type="number">
    /// <item><param name="book">Instancia de ModelPdf que representa el libro</param></item>
    /// </list>
    /// </summary>
    public async Task LoadPdfImage(ModelPdf book)
    {
        try
        {
            // Cargamos la imagen desde su ubicación en el directorio caché si ya existe
            string tempImagePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, $"cover_{book.id}.png");
            if (File.Exists(tempImagePath))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    book.CoverImage = ImageSource.FromFile(tempImagePath);
                });
                return;
            }

            // Si no existe en cache obtenemos la URL de descarga desde Firebase Storage
            var downloadUrl = await firebaseStorageManager.GetItemDownloadUrlAsync("Books", book.id);

            using (var httpClient = new HttpClient())
            {
                byte[] pdfBytes = await httpClient.GetByteArrayAsync(downloadUrl);

                // Verificar que tenemos datos
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    throw new Exception("No se pudieron descargar los datos del PDF");
                }

                using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
                {
                    PdfToImageConverter converter = new PdfToImageConverter();
                    converter.Load(pdfStream);

                    // Convertir específicamente a PNG
                    using (Stream imageStream = converter.Convert(0, false, false))
                    {
                        // Asegurarnos de que estamos al inicio del stream
                        imageStream.Position = 0;

                        using (FileStream fileStream = File.Create(tempImagePath))
                        {
                            await imageStream.CopyToAsync(fileStream);
                        }

                        // Verificar que el archivo existe 
                        if (File.Exists(tempImagePath))
                        {
                            var fileInfo = new FileInfo(tempImagePath);
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                // Cargar la imagen desde el archivo
                                book.CoverImage = ImageSource.FromFile(tempImagePath);
                            });
                        }
                        else
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                Console.WriteLine("No se pudo crear el archivo de imagen");
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Console.WriteLine($"Error: {ex.Message}");
            });
        }

    }

    /// <summary>
    /// Metodo que te lleva a la vista de editar/subir un libro
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnAddBookClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditPdfData(null, ""));
    }

    /// <summary>
    /// Metodo para editar o borrar libro
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnShowOptionsBookClicked(object sender, EventArgs e)
    {

        if (sender is ImageButton imageButton && imageButton.BindingContext is ModelPdf selectedBook)
        {
            var modalPage = new BookModal(selectedBook, isBookToggled);
            modalPage.OnResult += ModalPage_OnResult;
            await Navigation.PushModalAsync(modalPage);
        }
    }

    /// <summary>
    /// Método que espera el resultado del modal de editar o borrar libro
    /// <list type="number">
    /// <item><param name="selectedBook">Libro del evento</param></item>
    /// <item><param name="result">Resultado del modal</param></item>
    /// </list>
    /// </summary>
    private void ModalPage_OnResult(ModelPdf selectedBook, string result)
    {
        OnDoOptionsBookClicked(result, selectedBook);
    }

    /// <summary>
    /// Metodo para editar o borrar libro (click derecho)
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void OnOptionsBookClicked(object sender, EventArgs e)
    {
        //Opciones de libro aprobado y pending, editar o borrar
        if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.BindingContext is ModelPdf selectedBook)
        {
            MenuFlyoutItem? menuItem = sender as MenuFlyoutItem;
            var result = menuItem?.CommandParameter as string;
            OnDoOptionsBookClicked(result??"", selectedBook);
        }
    }

    /// <summary>
    /// Metodo que procesa la seleccion de la opcion del libor hecha
    /// <list type="number">
    /// <item><param name="result">seleccion hecha</param></item>
    /// <item><param name="selectedBook">libro vinculado</param></item>
    /// </list>
    /// </summary>
    private async void OnDoOptionsBookClicked(string result, ModelPdf selectedBook)
    {
        await Task.Delay(1);
        // Manejo de la opción seleccionada
        if (result == "Editar")
        {
            string type = isBookToggled ? "PendingBooks" : "Books";
            await Navigation.PushAsync(new EditPdfData(selectedBook, type));
        }
        else if (result == "Borrar")
        {
            await DisplayAlert(
                LocalizationResourceManager.Instance["msnDelBook"].ToString() ?? "Borrar Libro",
                (LocalizationResourceManager.Instance["msnAskDelBook"].ToString() ?? "¿Estás seguro de que desea eliminar el libro: ") + selectedBook.title + "?",
                LocalizationResourceManager.Instance["txtConfirmar"].ToString() ?? "Confirmar",
                LocalizationResourceManager.Instance["btCancelar"].ToString() ?? "Cancelar"
            ).ContinueWith(async (task) =>
            {
                if (task.Result)
                {
                    string collection = isBookToggled ? "PendingBooks" : "Books";
                    // Acción para Confirmar
                    await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnDeletingBook"].ToString() ?? "Eliminando libro...");
                    await firebaseStorageManager.DeleteItemAsync("Books", selectedBook.id);
                    await firebaseDatabaseManager.DeleteItemAsync(collection, selectedBook.id);
                    UpdateBooksCollection();
                }
                // En caso de Cancelar, no es necesario hacer nada, ya que simplemente se cierra el diálogo.
            });
        }
        else if (result == "Subir")
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtUploading"].ToString() ?? "Subiendo libro...");
            try
            {
                if (selectedBook != null)
                {
                    var bookData = new Dictionary<string, object>
                    {
                        ["categoryId"] = selectedBook.categoryId,
                        ["description"] = selectedBook.description,
                        ["id"] = selectedBook.id,
                        ["timestamp"] = selectedBook.timestamp,
                        ["title"] = selectedBook.title,
                        ["uid"] = selectedBook.uid,
                        ["url"] = selectedBook.url,
                        ["viewsCount"] = 0
                    };

                    await firebaseDatabaseManager.UpdateItemAsync("Books", selectedBook.id, bookData);
                    await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnDelAux"].ToString() ?? "Borrando registro provisional...");
                    await firebaseDatabaseManager.DeleteItemAsync("PendingBooks", selectedBook.id);
                    UpdateBooksCollection();
                }
            }
            catch (Exception ex)
            {
                await messageManager.ShowMessage((LocalizationResourceManager.Instance["txtFailUpload"].ToString() ?? "Error al subir el libro: ") + ex.Message);
            }
        }
    }

    /// <summary>
    /// Alterna la visualización de libros subidos o pendientes
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void OnSwitchBooksToggled(object sender, ToggledEventArgs e)
    {
        isBookToggled = e.Value;
        OnPropertyChanged(nameof(isBookToggled));
        UpdateBooksCollection();
    }

    /// <summary>
    /// Si el usuario esta logeado navega a la página de su perfil.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnProfileClicked(object sender, EventArgs e)
    {
        if (App.AuthClient.User != null)
        {
            await Navigation.PushAsync(new Profile());
        }
        else
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtRequestLog"].ToString() ?? "Logeate para acceder a tu perfil");
        }
    }

    /// <summary>
    /// Deslogea al usuario si lo esta y navega a la página de inicio.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        if (App.AuthClient.User != null)
        {
            App.AuthClient.SignOut();
            logManager.DeleteUserCredentials();
        }
        await Navigation.PushAsync(new MainPage());
    }

    /// <summary>
    /// Al salir deslogearse si lo esta
    /// </summary>
    protected override bool OnBackButtonPressed()
    {
        if (App.AuthClient.User != null)
        {
            App.AuthClient.SignOut();
            logManager.DeleteUserCredentials();
        }
        Navigation.PushAsync(new MainPage());
        return true;
    }

    /// <summary>
    /// Metodo para actualizar la collecion de libros tras un cambio.
    /// </summary>
    private async void UpdateBooksCollection()
    {
        if (App.AuthClient.User != null)
        {
            ModelUser user = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", App.AuthClient.User.Uid);
            userLabel.Text = user.Email;
        }
        if (isBookToggled)
        {
            searchBarId.Text = "";
            AllBooks.Clear();
            try
            {
                var books = await firebaseDatabaseManager.GetAllCollectionAsync<ModelPdf>("PendingBooks");

                foreach (var book in books)
                {
                    try
                    {
                        var bookData = book.Object;

                        bookData.title = string.IsNullOrEmpty(bookData.title)
                            ? LocalizationResourceManager.Instance["txtNoTitulo"].ToString() ?? "Título desconocido"
                            : bookData.title;
                        bookData.description = string.IsNullOrEmpty(bookData.description)
                            ? LocalizationResourceManager.Instance["txtNoDescripcion"].ToString() ?? "Sin descripción"
                            : bookData.description;
                        //Generamos datos extra para la visualizacion del libro
                        await LoadCategoryForBook(bookData);
                        await LoadPdfImage(bookData);

                        AllBooks.Add(bookData);
                        rawBooks.Add(bookData);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar libro: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar libros de firebase: {ex.Message}");
            }
        }
        else
        {
            AllBooks.Clear();
            await LoadBooks();
        }
    }

    /// <summary>
    /// Evento al seleccionar un libro. Navega a la página de detalle
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnBookSelected(object sender, SelectionChangedEventArgs e)
    {
        // Obtén el elemento seleccionado (book)
        var selectedBook = e.CurrentSelection.FirstOrDefault() as ModelPdf;

        if (selectedBook == null) return;

        string type = isBookToggled ? "PendingBooks" : "Books";

        await Navigation.PushAsync(new Libro(selectedBook.id, type));
    }

    /// <summary>
    /// Método que te lleva a la página de las categorias.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void gotoCategoryPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DashBoardCategories());
    }

    /// <summary>
    /// Método que te lleva a la página de los usuarios.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void gotoUsersPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DashBoardUsers());
    }

    /// <summary>
    /// Método que te lleva a la página de Ayuda.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void gotoAyudaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Ayuda());
    }
}