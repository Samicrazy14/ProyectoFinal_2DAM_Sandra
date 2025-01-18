using AppProyectoFinal.Data;
using Syncfusion.PdfToImageConverter;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppProyectoFinal.Vistas;

public partial class Profile : ContentPage, INotifyPropertyChanged
{
    //Manejadores de BBDD y mensajes
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    private FirebaseStorageManager firebaseStorageManager = new FirebaseStorageManager();
    private MessageManager messageManager;
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;
    private LogManager logManager = new LogManager();

    //Atributos
    private ModelUser _currentUser;
    public ModelUser CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedJoinDate));
            }
        }
    }
    private ObservableCollection<ModelPdf> _booksCollection;
    public ObservableCollection<ModelPdf> BooksCollection
    {
        get => _booksCollection;
        set
        {
            if (_booksCollection != value)
            {
                _booksCollection = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FavoriteBooksCount));
            }
        }
    }
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    public string FormattedJoinDate => DateTimeOffset
       .FromUnixTimeMilliseconds(CurrentUser?.Timestamp ?? 0)
       .DateTime
       .ToString("dd/MM/yyyy");
    public string FavoriteBooksCount => BooksCollection?.Count.ToString() ?? "0";

    public new event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Página donde se puede ver tus datos como usuario y libros favoritos
    /// </summary>
    public Profile()
	{
		InitializeComponent();
        _booksCollection = new ObservableCollection<ModelPdf>();
        messageManager = new MessageManager(mainPage);
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a su perfil de usuario");
        FavBooksCollection.SelectedItem = null;
        Appearing += async (s, e) => await InitializeAsync();
    }

    /// <summary>
    /// Inicializa los datos de la página
    /// </summary>
    private async Task InitializeAsync()
    {
        if (CurrentUser == null)
        {
            IsLoading = true;
            try
            {
                await LoadUserDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Carga los datos de la página
    /// </summary>
    private async Task LoadUserDataAsync()
    {
        await LoadUserInfo();
        await LoadFavoriteBooks();
    }

    /// <summary>
    /// Carga los datos del usuario
    /// </summary>
    private async Task LoadUserInfo()
    {
        try
        {
            CurrentUser = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", App.AuthClient.User.Uid);

            if (string.IsNullOrEmpty(CurrentUser.ProfileImage))
            {
                ProfileImage.Source = ImageSource.FromFile("ic_person_notnight.png");
            }
            else
            {
                ProfileImage.Source = ImageSource.FromUri(new Uri(CurrentUser.ProfileImage));
            }
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnErrorLoadUser"].ToString() ?? "Error al cargar los datos de usuario: ") + ex.Message);
        }
    }

    /// <summary>
    /// Carga los datos de los libros favoritos del usuario
    /// </summary>
    private async Task LoadFavoriteBooks()
    {
        try
        {
            var favs = await firebaseDatabaseManager.GetFavoritesBooksAsync(App.AuthClient.User.Uid);

            BooksCollection.Clear();

            foreach (var fav in favs)
            {
                try
                {
                    var favData = fav.Object;
                    var bookId = favData.bookId.ToString();
                    var type = favData.type.ToString();

                    var book = await firebaseDatabaseManager.GetItemByIdAsync<ModelPdf>(type, bookId);
                    if (book != null)
                    {
                        book.title = string.IsNullOrEmpty(book.title) 
                            ? LocalizationResourceManager.Instance["txtNoTitulo"].ToString() ?? "Título desconocido" 
                            : book.title;
                        book.description = string.IsNullOrEmpty(book.description) 
                            ? LocalizationResourceManager.Instance["txtNoDescripcion"].ToString() ?? "Sin descripción" 
                            : book.description;
                        if (type == "GutembergBooks")
                        {
                            book.Category = book.categoryId;
                            book.CoverImage = book.imagenUrl;
                        }
                        else {
                            await LoadCategoryForBook(book);
                            await LoadPdfImage(book);
                        }
                        BooksCollection.Add(book);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar libro: {ex.Message}");
                }
            }
            // Notifica el cambio en el recuento de libros favoritos
            OnPropertyChanged(nameof(FavoriteBooksCount));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    /// <summary>
    /// Carga la información de categoría del libro dado
    /// <list type="number">
    /// <item><param name="book">Objeto de ModelPdf con la info del libro</param></item>
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
    /// Carga la imagen del libro dado
    /// <list type="number">
    /// <item><param name="book">Objeto de ModelPdf con la info del libro</param></item>
    /// </list>
    /// </summary>
    public async Task LoadPdfImage(ModelPdf book)
    {
        try
        {
            // Ubicación de la imagen en el directorio de caché
            string tempImagePath = Path.Combine(FileSystem.CacheDirectory, $"cover_{book.id}.png");
            if (File.Exists(tempImagePath))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    book.CoverImage = ImageSource.FromFile(tempImagePath);
                });
                return;
            }

            // Obtener la URL de descarga desde Firebase Storage
            var downloadUrl = await firebaseStorageManager.GetItemDownloadUrlAsync("Books", book.id);

            using (var httpClient = new HttpClient())
            {
                byte[] pdfBytes = await httpClient.GetByteArrayAsync(downloadUrl);

                // Verificar que tenemos datos del PDF
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    throw new Exception("No se pudieron descargar los datos del PDF");
                }

                using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
                {
                    // Configurar el convertidor con opciones específicas
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

                        // Verificar que el archivo existe y tiene tamaño
                        if (File.Exists(tempImagePath))
                        {
                            var fileInfo = new FileInfo(tempImagePath);
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
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
    /// Al clickar en el rol de la cuenta permite en algunos casos pedir la verificación del email
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnVerifyAccountClicked(object sender, EventArgs e)
    {
        switch (CurrentUser.UserType)
        {
            case "author":
                await DisplayAlert("Info", LocalizationResourceManager.Instance["msnVerified"].ToString() ?? "Ya estas verificado", "OK");
                break;

            case "pending user":
                await DisplayAlert("Info", LocalizationResourceManager.Instance["msnPendingVerified"].ToString() ?? "Verificación pendiente de ser aprobada", "OK");
                break;

            case "admin":
                break;
            default:
                await ShowEmailVerificationDialog();
                break;
        }
    }

    /// <summary>
    /// Muestra un dialogo de confirmación antes de hacer la petición
    /// </summary>
    private async Task ShowEmailVerificationDialog()
    {
        bool result = await DisplayAlert(
            LocalizationResourceManager.Instance["txtVerificar"].ToString() ?? "Verificar Email",
            (LocalizationResourceManager.Instance["msnAskVerify"].ToString() ?? "¿Estas seguro de querer verificar tu usuario ") + $"{CurrentUser.Email}?" +
            LocalizationResourceManager.Instance["msnInfoVerify"].ToString() ?? "Se actualizará tu cuenta a una de escritor tras ser procesada tu solicitud.",
            LocalizationResourceManager.Instance["btEnviar"].ToString() ?? "Enviar",
            LocalizationResourceManager.Instance["btCancelar"].ToString() ?? "Cancelar");

        if (result)
        {
            await SendEmailVerification();
        }
    }

    /// <summary>
    /// Envía la petición de verificar email
    /// </summary>
    private async Task SendEmailVerification()
    {
        try
        {

            await firebaseDatabaseManager.UpdateUserTypeAsync("Users", App.AuthClient.User.Uid, "pending user");

            await messageManager.ShowMessage((LocalizationResourceManager.Instance["txtSuccess"].ToString() ?? "Éxito")+ "\n" + LocalizationResourceManager.Instance["msnSuccesRequest"].ToString() ?? "Solicitud creada con éxito");
            await LoadUserInfo(); // Recargar info del usuario para actualizar el estado
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage("Error \n" + (LocalizationResourceManager.Instance["msnErrorRequest"].ToString() ?? "Fallo al solicitar la petición: ") + ex.Message);
        }
    }

    /// <summary>
    /// Formatea la fecha de creación
    /// <list type="number">
    /// <item><param name="timestamp">fecha</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve la fecha formateada</returns>
    private string FormatTimestamp(long timestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp)
            .LocalDateTime.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Al clickar a un libro se va a su página de detalle
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnBookSelected(object sender, SelectionChangedEventArgs e)
    {
        // Obtiene el elemento seleccionado (book)
        var selectedBook = e.CurrentSelection.FirstOrDefault() as ModelPdf;

        if (selectedBook == null) return;

        var type = "Books";
        if (selectedBook.uid == "Project Gutenberg")
        {
            type = "GutembergBooks";
        }
        
        await Navigation.PushAsync(new Libro(selectedBook.id, type));
    }

    /// <summary>
    /// Va a la modificación de perfil
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void goAjustes_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ProfileEdit());
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

    /// <summary>
    /// Método que te lleva a la página de inicio.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void gotoMainClicked(object sender, EventArgs e)
    {
        if (CurrentUser.UserType != "admin")
        {
            await Navigation.PushAsync(new DashBoardPublicBooks());
        }
        else
        {
            await Navigation.PushAsync(new DashBoardCategories());
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
}