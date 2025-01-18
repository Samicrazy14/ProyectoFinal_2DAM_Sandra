using AppProyectoFinal.Data;
using iText.Kernel.Pdf;
using System.Collections.ObjectModel;

namespace AppProyectoFinal.Vistas;

public partial class EditPdfData : ContentPage
{
    //Manejadores de BBDD y mensajes
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    private FirebaseStorageManager firebaseStorageManager = new FirebaseStorageManager();
    private MessageManager messageManager;
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    //Atributos
    public ObservableCollection<ModelCategory> AllCategories { get; set; }
    private ModelCategory? _selectedCategory;
    public ModelCategory SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged(nameof(SelectedCategory));

            if (_selectedCategory != null)
            {
                Console.WriteLine($"ID de la categoría seleccionada: {_selectedCategory.Id}");
            }
        }
    }
    private FileResult? pdfFile;
    private int pages = 0;
    private ModelPdf? bookEdit;
    private string type;

    public EditPdfData(ModelPdf bookEdit, string type)
	{
		InitializeComponent();

        this.bookEdit = bookEdit;
        this.type = type;
        AllCategories = new ObservableCollection<ModelCategory>();
        BindingContext = this;
        messageManager = new MessageManager(mainPage);

        InitializeData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de edición de datos de un libro");
    }

    /// <summary>
    /// Método inicializar los datos
    /// </summary>
    private async void InitializeData()
    {
        var categories = await firebaseDatabaseManager.GetAllCollectionAsync<ModelCategory>("Categories");
        foreach (var category in categories)
        {
            try
            {
                var categoryData = category.Object;
                string Category = categoryData.Category ?? string.Empty;
                AllCategories.Add(categoryData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar categoria: {ex.Message}");
            }
        }
        
        //Logic for only EDIT no ADD
        if (bookEdit != null) {
            //Adaptar UI
            attachDocument.IsVisible = false;
            textPage.IsVisible = false;
            titlePage.Text = LocalizationResourceManager.Instance["txtEditBook"].ToString() ?? "Editar información del libro";
            updateButton.Text = LocalizationResourceManager.Instance["btActualizar"].ToString() ?? "Actualizar";
            //Load Data
            TitleEntry.Text = bookEdit.title;
            DescriptionEntry.Text = bookEdit.description;
            if (bookEdit.categoryId != null)
            {
                SelectedCategory = await firebaseDatabaseManager.GetItemByIdAsync<ModelCategory>("Categories", bookEdit.categoryId);
            }
        }
    }

    /// <summary>
    /// Método para añadir el documento del libro
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnAttachPdfClicked(object sender, EventArgs e)
    {
        try
        {
            // Configura el FilePicker para solo aceptar archivos PDF
             pdfFile = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf
            });

            if (pdfFile != null)
            {
                await messageManager.ShowMessage((LocalizationResourceManager.Instance["txtSelected"].ToString() ?? "Seleccionado el archivo: ") + pdfFile.FileName);
                await GetPdfPageCountAsync(pdfFile);
            }
            else
            {
                pages = 0;
                Console.WriteLine("No se seleccionó ningún archivo.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al seleccionar el archivo PDF: {ex.Message}");
        }
    }

    /// <summary>
    /// Método obtener el numero de páginas del libro
    /// <list type="number">
    /// <item><param name="pdfFile">archivo sobre el que contar páginas</param></item>
    /// </list>
    /// </summary>
    private async Task GetPdfPageCountAsync(FileResult pdfFile)
    {
        using var stream = await pdfFile.OpenReadAsync();
        using var pdfReader = new PdfReader(stream);
        using var pdfDocument = new PdfDocument(pdfReader);
        pages = pdfDocument.GetNumberOfPages();
    }


    /// <summary>
    /// Método que valida los datos e inicia el proceso para subir el libro
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnUploadBookClicked(object sender, EventArgs e)
    {
        // Asigna los valores de las entradas de texto y el Picker
        var title = TitleEntry.Text?.Trim();
        var description = DescriptionEntry.Text?.Trim();
        var category = SelectedCategory?.Id ?? null;

        // Validaciones
        if (string.IsNullOrEmpty(title))
        {
            await DisplayAlert("Error", LocalizationResourceManager.Instance["txtAskTitulo"].ToString() ?? "Introduce el título", "OK");
        }
        else if (string.IsNullOrEmpty(description))
        {
            await DisplayAlert("Error", LocalizationResourceManager.Instance["txtAskDesc"].ToString() ?? "Introduce la descripción", "OK");
        }
        else if (string.IsNullOrEmpty(category))
        {
            await DisplayAlert("Error", LocalizationResourceManager.Instance["txtAskCat"].ToString() ?? "Selecciona una categoría", "OK");
        }
        else if (pdfFile == null && bookEdit == null)
        {
            await DisplayAlert("Error", LocalizationResourceManager.Instance["txtAskpdf"].ToString() ?? "Selecciona un PDF", "OK");
        }
        else
        {
            if (bookEdit == null) {
                await UploadPdfToStorage(title, description, category);
            }
            else
            {
                await UploadPdfInfoToDbAsync("", 0, title, description, category);
            }
            
        }
    }

    /// <summary>
    /// Método que sube el documento pdf a Storage
    /// </summary>
    private async Task UploadPdfToStorage(string title, string description, string categoryId)
    {
        long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        string filePathAndName = $"Books/{timestamp}";
        await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtUploading"].ToString() ?? "Subiendo el documento...");
        try
        {
            using var stream = await pdfFile.OpenReadAsync();
            // Subir PDF a Firebase Storage
            await firebaseStorageManager.PushItemAsync("Books", timestamp.ToString(), stream);
            // Obtener URL de descarga
            var uploadedPdfUrl = await firebaseStorageManager.GetItemDownloadUrlAsync("Books", timestamp.ToString());
            await UploadPdfInfoToDbAsync(uploadedPdfUrl.ToString(), timestamp, title, description, categoryId);
        }
        catch (Exception e)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["txtFailUpload"].ToString() ?? "Fallo al subir el documento: ") + e.Message);
        }
    }

    /// <summary>
    /// Método que sube los datos del libro a la BBDD
    /// <list type="number">
    /// <item><param name="uploadedPdfUrl">Url del archivo pdf</param></item>
    /// <item><param name="timestamp">id del libro</param></item>
    /// </list>
    /// </summary>
    private async Task UploadPdfInfoToDbAsync(string uploadedPdfUrl, long timestamp, string title, string description, string categoryId)
    {
        await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtUploadInfo"].ToString() ?? "Subiendo la información del documento... ");

        if (bookEdit == null)
        {
            string uid = App.AuthClient.User.Uid;

            // Datos del libro
            var bookData = new Dictionary<string, object>
            {
                { "uid", uid },
                { "id", timestamp.ToString() },
                { "title", title },
                { "description", description },
                { "categoryId", categoryId },
                { "pagecount", pages },
                { "url", uploadedPdfUrl },
                { "timestamp", timestamp },
                { "viewsCount", 0 }
            };
            try
            {
                await firebaseDatabaseManager.UpdateItemAsync("PendingBooks", timestamp.ToString(), bookData);
                await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtSuccesUpload"].ToString() ?? "Información subida con éxito, pendiente de aprobación por un administrador");

                pdfFile = null;
                pages = 0;
                await Navigation.PopAsync();
            }
            catch (Exception e)
            {
                await messageManager.ShowMessage((LocalizationResourceManager.Instance["txtFailUploadInfo"].ToString() ?? "Fallo al subir la información del documento: ")+ e.Message);
            }
        }
        else
        {
            try {
                await firebaseDatabaseManager.UpdateBookDataAsync(type, bookEdit.id, title, description, categoryId);
                await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtSuccesUploadInfo"].ToString() ?? "Información actualizada con éxito");

                pdfFile = null;
                await Navigation.PopAsync();
            }
            catch (Exception e)
            {
                await messageManager.ShowMessage((LocalizationResourceManager.Instance["txtFailUpdateInfo"].ToString() ?? "Fallo al actualizar la información del documento: ") + e.Message);
            }
        }
    }
}