using AppProyectoFinal.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AppProyectoFinal.Vistas;

public partial class DashBoardCategories : ContentPage
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
    public ObservableCollection<ModelCategory> AllCategories { get; set; }
    public ObservableCollection<ModelCategory> rawCategories { get; set; }

    public DashBoardCategories()
	{
		InitializeComponent();

        // Inicializa colecciones y dependencias
        AllCategories = new ObservableCollection<ModelCategory>();
        rawCategories = new ObservableCollection<ModelCategory>();
        httpClient = new HttpClient();
        messageManager = new MessageManager(mainPage, mainlayout);
        BindingContext = this;

        // Cargar datos en las colecciones
        LoadCategories();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de gestión de categorías para administrador");
        AllCategoriesCollection.SelectedItem = null;
    }

    /// <summary>
    /// Metodo que filtra los libros por título usando el texto introducido en la barra de busqueda.
    /// </summary>
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = RemoveDiacritics(e.NewTextValue?.ToLower() ?? string.Empty);
        AllCategories.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var category in rawCategories)
            {
                AllCategories.Add(category);
            }
        }
        else
        {
            // Filtra los elementos de rawBooks por el título
            foreach (var category in rawCategories.Where(b => RemoveDiacritics(b.Category.ToLower()).Contains(searchText)))
            {
                AllCategories.Add(category);
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
    /// Carga todas las categorias de Firebase y los añade a la colección AllCategories.
    /// </summary>
    private async Task LoadCategories()
    {
        if (App.AuthClient.User != null)
        {
            ModelUser user = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", App.AuthClient.User.Uid);
            userLabel.Text = user.Email;
        }
        try
        {
            var categories = await firebaseDatabaseManager.GetAllCollectionAsync<ModelCategory>("Categories");

            foreach (var category in categories)
            {
                try
                {
                    var categoryData = category.Object;
                    string Category = categoryData.Category ?? string.Empty;

                    AllCategories.Add(categoryData);
                    rawCategories.Add(categoryData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar categoria: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar categorias de firebase: {ex.Message}");
        }
    }

    /// <summary>
    /// Método para eliminar una categoría
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnDeleteCategoryClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton imageButton && imageButton.BindingContext is ModelCategory selectedCategory)
        {
            await DisplayAlert(
                LocalizationResourceManager.Instance["msnDeleteCategory"].ToString() ?? "Borrar Categoría",
                (LocalizationResourceManager.Instance["msnAskDelete"].ToString() ?? "¿Estás seguro de que desea eliminar la categoría ") + selectedCategory.Category +"?",
                LocalizationResourceManager.Instance["txtConfirmar"].ToString() ?? "Confirmar",
                LocalizationResourceManager.Instance["btCancelar"].ToString() ?? "Cancelar"
            ).ContinueWith(async (task) =>
            {
                if (task.Result)
                {
                    // Acción para Confirmar
                    await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnDeletingCat"].ToString() ?? "Eliminando categoría...");
                    await firebaseDatabaseManager.DeleteItemAsync("Categories", selectedCategory.Id);
                    AllCategories.Clear();
                    await LoadCategories();
                }
                // En caso de Cancelar, no es necesario hacer nada, ya que simplemente se cierra el diálogo.
            });
        }
    }

    /// <summary>
    /// Método para añadir una categoría
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        string categoryName = await DisplayPromptAsync(
           LocalizationResourceManager.Instance["msnNewCategory"].ToString() ?? "Nueva Categoría",
           LocalizationResourceManager.Instance["txtEnterCat"].ToString() ?? "Introduce el nombre de la categoría:",
           placeholder: LocalizationResourceManager.Instance["phCategory"].ToString() ?? "Ej: Ciencia Ficción",
           maxLength: 50,
           keyboard: Keyboard.Text
        );

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            string normalizedCategory = NormalizeString(categoryName);

            // Verificar si la categoría ya existe en AllCategories
            bool categoryExists = AllCategories.Any(c => NormalizeString(c.Category).Equals(normalizedCategory, StringComparison.OrdinalIgnoreCase));

            if (categoryExists)
            {
                await DisplayAlert(LocalizationResourceManager.Instance["txtWarning"].ToString() ?? "Advertencia",
                    LocalizationResourceManager.Instance["txtAlreadyCat"].ToString() ?? "La categoría ya existe", "OK");
                return;
            }

            try
            {
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var categoryData = new Dictionary<string, object>
                {
                    ["id"] = timestamp,
                    ["category"] = categoryName,
                    ["timestamp"] = timestamp,
                    ["uid"] = App.AuthClient.User.Uid,
                };

                await firebaseDatabaseManager.UpdateItemAsync("Categories", timestamp.ToString(), categoryData);
                await DisplayAlert(LocalizationResourceManager.Instance["txtSuccess"].ToString() ?? "Éxito",
                    LocalizationResourceManager.Instance["txtCatOk"].ToString() ?? "Categoría creada correctamente", "OK");
                AllCategories.Clear();
                await LoadCategories();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", (LocalizationResourceManager.Instance["txtFailCat"].ToString() ?? "No se pudo crear la categoría: ") + ex.Message, "OK");
            }
        }
    }

    /// <summary>
    /// Método para normalizar el texto
    /// </summary>
    private string NormalizeString(string input)
    {
        // Convertir a minúsculas
        string result = input.ToLowerInvariant();

        // Eliminar acentos y caracteres diacríticos
        result = string.Concat(result.Normalize(NormalizationForm.FormD)
                      .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

        // Eliminar caracteres que no sean letras (a-z)
        result = Regex.Replace(result, "[^a-z]", "");

        return result;
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
    /// Método que te lleva a la página de los libros subidos.
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void gotoBookPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DashBoardBooks());
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