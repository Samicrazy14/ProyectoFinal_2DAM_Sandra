using AppProyectoFinal.Data;
using AppProyectoFinal.Servicios;
using System.Collections.ObjectModel;

namespace AppProyectoFinal.Vistas;

public partial class DashBoardPublicBooks : ContentPage
{
    //Colleccion de datos
    public ObservableCollection<ModelPdf> GutenbergBooks { get; set; }
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
    private bool isLoading = false;

    public DashBoardPublicBooks()
	{
		InitializeComponent();

        // Inicializa colecciones y dependencias
        GutenbergBooks = new ObservableCollection<ModelPdf>();
        rawBooks = new ObservableCollection<ModelPdf>();
        httpClient = new HttpClient();
        messageManager = new MessageManager(mainPage, mainlayout);
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página principal con un listado de obras públicas disponibles");
        GutenbergBooksCollection.SelectedItem = null;
        InitializeData();
    }

    /// <summary>
    /// Configura la vista de la página en función del tipo de usuario
    /// </summary>
    private async Task InitializeData()
    {
        GutenbergBooks.Clear();
        if (App.AuthClient.User != null)
        {
            ModelUser user = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", App.AuthClient.User.Uid);
            publicAuthorButton.IsVisible = (user.UserType == "author");
            userData.IsVisible = true;
            viewedButton.IsVisible = true;
            newButton.IsVisible = true;
            userLabel.Text = user.Email;
            await LoadGutembergBooks();
        }
        else
        {
            userLabel.Text = LocalizationResourceManager.Instance["txtNoUser"].ToString() ?? "Usuario desconocido";
            userData.IsVisible = false;
            publicAuthorButton.IsVisible = false;
            viewedButton.IsVisible = false;
            newButton.IsVisible = false;
            await LoadGutembergBooks();
        }
    }


    /// <summary>
    /// Metodo que filtra los libros por título usando el texto introducido en la barra de busqueda.
    /// </summary>
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = RemoveDiacritics(e.NewTextValue?.ToLower() ?? string.Empty);
        GutenbergBooks.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var book in rawBooks)
            {
                GutenbergBooks.Add(book);
            }
        }
        else
        {
            // Filtra los elementos de rawBooks por el título
            foreach (var book in rawBooks.Where(b => RemoveDiacritics(b.title.ToLower()).Contains(searchText)))
            {
                GutenbergBooks.Add(book);
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
    /// Carga libros de Gutenberg y Google Books mediante APIs y los añade a GutenbergBooks.
    /// </summary>
    private async Task LoadGutembergBooks()
    {
        var googleBooksAPI = new GoogleBooksAPI(httpClient);
        var gutenbergAPI = new GutenbergAPI(httpClient);
        await gutenbergAPI.LoadGutembergBooks(currentPage, GutenbergBooks, rawBooks, googleBooksAPI.GetBookInfoAsync);
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
        // Obtiene el elemento seleccionado (book)
        var selectedBook = e.CurrentSelection.FirstOrDefault() as ModelPdf;

        if (selectedBook == null) return;

        var bookData = new Dictionary<string, object>
        {
            ["author"] = selectedBook.author,
            ["categoryId"] = selectedBook.categoryId,
            ["description"] = selectedBook.description,
            ["id"] = selectedBook.id,
            ["imagenUrl"] = selectedBook.imagenUrl,
            ["pagecount"] = selectedBook.pagecount,
            ["timestamp"] = selectedBook.timestamp,
            ["title"] = selectedBook.title,
            ["uid"] = selectedBook.uid,
            ["url"] = selectedBook.url,
            ["viewsCount"] = selectedBook.viewsCount
        };

        var bookRef = await firebaseDatabaseManager.GetItemByIdAsync<ModelPdf>("GutembergBooks", selectedBook.id);
        if (bookRef == null)
        {
            await firebaseDatabaseManager.UpdateItemAsync("GutembergBooks", selectedBook.id, bookData);
        }

        await Navigation.PushAsync(new Libro(selectedBook.id, "GutembergBooks"));
    }

    /// <summary>
    /// Metodo que carga más contenido al llegar al fin del scroll
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {

        var scrollView = sender as ScrollView;
        // Calcula si el usuario ha llegado al final del scroll
        double scrollingSpace = scrollView.ContentSize.Height - scrollView.Height;

        if (scrollingSpace <= e.ScrollY)
        {
            if (isLoading) return;
            messageManager.ShowLoading(LocalizationResourceManager.Instance["txtLoadingBooks"].ToString() ?? "Cargando más libros ...");

            isLoading = true;
            currentPage++;
            await LoadGutembergBooks();
            isLoading = false;
            await Task.Delay(2000);
            messageManager.HideLoading();

        }
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
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtRequestLog"].ToString() ?? "Inicia sesión para acceder a tu perfil");
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
    /// Método que te lleva a la página de los libros más vistos.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void gotoViewedPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DashBoardViewedBooks());
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