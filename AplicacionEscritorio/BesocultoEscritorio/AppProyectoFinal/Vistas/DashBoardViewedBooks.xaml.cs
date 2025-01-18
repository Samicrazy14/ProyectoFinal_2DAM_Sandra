using AppProyectoFinal.Data;
using Syncfusion.PdfToImageConverter;
using System.Collections.ObjectModel;

namespace AppProyectoFinal.Vistas;

public partial class DashBoardViewedBooks : ContentPage
{
    //Colleccion de datos
    public ObservableCollection<ModelPdf> MostViewedBooks { get; set; }
    public ObservableCollection<ModelPdf> rawBooks { get; set; }

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
    public DashBoardViewedBooks()
	{
		InitializeComponent();

        // Inicializa colecciones y dependencias
        MostViewedBooks = new ObservableCollection<ModelPdf>();
        rawBooks = new ObservableCollection<ModelPdf>();
        httpClient = new HttpClient();
        messageManager = new MessageManager(mainPage, mainlayout);
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página principal con un listado de los libros más vistos en la app");
        MostViewedBooksCollection.SelectedItem = null;
        InitializeData();
    }

    /// <summary>
    /// Configura la vista de la página en función del tipo de usuario
    /// </summary>
    private async Task InitializeData()
    {
        MostViewedBooks.Clear();
        if (App.AuthClient.User != null)
        {
            ModelUser user = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", App.AuthClient.User.Uid);
            viewedAuthorButton.IsVisible = (user.UserType == "author");
            userLabel.Text = user.Email;
            await LoadMostViewedBooks();
        }
    }

    /// <summary>
    /// Metodo que filtra los libros por título usando el texto introducido en la barra de busqueda.
    /// </summary>
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = RemoveDiacritics(e.NewTextValue?.ToLower() ?? string.Empty);
        MostViewedBooks.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var book in MostViewedBooks)
            {
                MostViewedBooks.Add(book);
            }
        }
        else
        {
            // Filtra los elementos de rawBooks por el título
            foreach (var book in MostViewedBooks.Where(b => RemoveDiacritics(b.title.ToLower()).Contains(searchText)))
            {
                MostViewedBooks.Add(book);
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
    /// Carga los libros más vistos desde Books en Firebase en orden de visualizaciones y los añade a la colección MostViewedBooks.
    /// </summary>
    private async Task LoadMostViewedBooks()
    {
        try
        {
            var viewedBooks = await firebaseDatabaseManager.GetMostViewedBooksAsync(10);

            foreach (var book in viewedBooks)
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

                    MostViewedBooks.Add(bookData);
                    rawBooks.Add(bookData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar libro: {ex.Message}");
                }
            }
            MostViewedBooks = new ObservableCollection<ModelPdf>(MostViewedBooks.OrderByDescending(b => b.viewsCount));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar libros de firebase por orden de vistas: {ex.Message}");
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
            string tempImagePath = Path.Combine(FileSystem.CacheDirectory, $"cover_{book.id}.png");
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
    /// Evento al seleccionar un libro. Navega a la página de detalle y actualiza la base de datos si es un libro de Gutenberg.
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

        await Navigation.PushAsync(new Libro(selectedBook.id, "Books"));
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
    /// Método que permite leer a un usuario un pdf de su equipo.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnReadBookClicked(object sender, EventArgs e)
    {
        try
        {
            // Permitir al usuario seleccionar un archivo PDF
            var pdfFile = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf
            });

            if (pdfFile != null)
            {
                // Abrir el archivo PDF como un Stream
                using Stream pdfStream = await pdfFile.OpenReadAsync();
                await Navigation.PushAsync(new Lectura("", "propio", pdfStream));
            }
            else
            {
                await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtNoFile"].ToString() ?? "No se seleccionó ningún archivo.");
            }
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["txtErrorPdf"].ToString() ?? "Error al leer el PDF: ") + ex.Message);
        }
    }

    /// <summary>
    /// Método que permite a un autor subir un pdf de su equipo.
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
    /// Método que te lleva a la página de los libros más vistos.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void gotoNewPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DashBoardNewBooks());
    }

    /// <summary>
    /// Método que te lleva a la página de los libros públicos.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void gotoPublicPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DashBoardPublicBooks());
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