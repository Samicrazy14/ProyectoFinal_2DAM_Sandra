using AppProyectoFinal.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace AppProyectoFinal.Vistas;

public partial class ProfileEdit : ContentPage, INotifyPropertyChanged
{
    //Manejadores de BBDD
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    private FirebaseStorageManager firebaseStorageManager = new FirebaseStorageManager();
    private MessageManager messageManager;

    //Atributos
    private FileResult? selectedImage;
    private ModelUser _currentUser;
    public ModelUser CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }
    }
    public new event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ICommand PickImageCommand { get; private set; }

    //Valores finales/inicales
    int initialThemeIndex = 0;
    int initialSizeIndex = 1;
    int initialLanguageIndex = 0;
    //Valores provisionales
    int selectedThemeIndex = 0;
    int selectedSizeIndex = 1;
    int selectedLanguageIndex = 0;

    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    //Colleciones de los picker
    public ObservableCollection<string> FontSizeOptions { get; set; } = new ObservableCollection<string>
    {
        LocalizationResourceManager.Instance["txtPequena"].ToString() ?? "Pequeña",
        LocalizationResourceManager.Instance["txtMediana"].ToString() ?? "Mediana",
        LocalizationResourceManager.Instance["txtGrande"].ToString() ?? "Grande"
    };

    public ObservableCollection<string> ThemeOptions { get; set; } = new ObservableCollection<string>
    {
        LocalizationResourceManager.Instance["txtClaro"].ToString() ?? "Claro",
        LocalizationResourceManager.Instance["txtOscuro"].ToString() ?? "Oscuro"
    };

    public ObservableCollection<string> LanguageOptions { get; set; } = new ObservableCollection<string>
    {
        LocalizationResourceManager.Instance["txtEspanol"].ToString() ?? "Español",
        LocalizationResourceManager.Instance["txtIngles"].ToString()?? "Inglés"
    };


    /// <summary>
    /// Página donde se pueden modificar tus datos como usuario y configuraciones
    /// </summary>
    public ProfileEdit()
	{
		InitializeComponent();
        PickImageCommand = new Command(async () => await PickImage());
        messageManager = new MessageManager(mainPage);
        BindingContext = this;

        // Cargar datos cuando la página aparezca
        LoadUserConfig();
        Appearing += async (s, e) => await LoadUserInfo();

        // Actualización dinamica del idioma en las opciones de los pickers
        LocalizationResourceManager.Instance.PropertyChanged += (s, e) =>
        {
            // Actualiza los textos traducidos
            FontSizeOptions[0] = LocalizationResourceManager.Instance["txtPequena"].ToString() ?? "Pequeña";
            FontSizeOptions[1] = LocalizationResourceManager.Instance["txtMediana"].ToString() ?? "Mediana";
            FontSizeOptions[2] = LocalizationResourceManager.Instance["txtGrande"].ToString() ?? "Grande";
            ThemeOptions[0] = LocalizationResourceManager.Instance["txtClaro"].ToString() ?? "Claro";
            ThemeOptions[1] = LocalizationResourceManager.Instance["txtOscuro"].ToString() ?? "Oscuro";
            LanguageOptions[0] = LocalizationResourceManager.Instance["txtEspanol"].ToString() ?? "Español";
            LanguageOptions[1] = LocalizationResourceManager.Instance["txtIngles"].ToString() ?? "Inglés";
        };
    }

    /// <summary>
    /// Al salir si no ha guardado los datos mantiene los iniciales
    /// </summary>
    protected override bool OnBackButtonPressed()
    {
        FontSizeManager.Instance.UpdateFontSizes(initialSizeIndex);
        ThemeManager.Instance.UpdateThemeColors(initialThemeIndex);
        CultureInfo cultureInfo = (initialLanguageIndex == 0) ? new CultureInfo("es") : new CultureInfo("en");
        LocalizationResourceManager.Instance.SetCulture(cultureInfo);
        return false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de ajustes de usuario, configure sus preferencias y datos como desee");
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
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnErrorLoadUser"].ToString() ?? "Error al cargar la información del ususario: ") + ex.Message);
        }

    }

    /// <summary>
    /// Carga la configuracion en uso del usuario
    /// </summary>
    private void LoadUserConfig()
    {
        double currentTitleSize = FontSizeManager.Instance.SizeTitulo;
        if (currentTitleSize <= 24)
            initialSizeIndex = 0;
        else if (currentTitleSize <= 28)
            initialSizeIndex = 1;
        else
            initialSizeIndex = 2;
        FontPicker.SelectedIndex = initialSizeIndex;
        selectedSizeIndex = initialSizeIndex;

        LanguagePicker.SelectedIndex = initialLanguageIndex;
        selectedLanguageIndex = initialLanguageIndex;

        Color currentTheme = ThemeManager.Instance.BackgroundPage;
        if (currentTheme == Colors.White)
            initialThemeIndex = 0;
        else
            initialThemeIndex = 1;
        ThemePicker.SelectedIndex = initialThemeIndex;
        selectedThemeIndex = initialThemeIndex;
    }

    /// <summary>
    /// Selecciona una imagen del equipo del usuario 
    /// </summary>
    private async Task PickImage()
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                selectedImage = result;
                // Cargar la imagen seleccionada en el Image
                var stream = await result.OpenReadAsync();
                ProfileImage.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnErrorImage"].ToString() ?? "No se pudo seleccionar la imagen.") + ex.Message);
        }
    }

    /// <summary>
    /// Aplica los cambios realizados
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void doCambios_Clicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(name))
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnName"].ToString() ?? "Introduce un nombre de usuario");
        }
        else
        {
            if (selectedImage == null)
            {
                await UpdateProfile(name, "");
            }
            else
            {
                await UploadImage(name);
            }
            await Navigation.PopAsync();
        }
    }

    /// <summary>
    /// Actualiza el perfil con los datos del usuario
    /// <list type="number">
    /// <item><param name="name">Nuevo nombre del perfil</param></item>
    /// </list>
    /// </summary>
    private async Task UpdateProfile(string name, string uploadedImageUrl)
    {
        // Mostrar el mensaje de progreso
        messageManager.ShowLoading(LocalizationResourceManager.Instance["msnUpdatePerfil"].ToString() ?? "Actualizando perfil...");
        try
        {
            // Actualiza el nombre
            await firebaseDatabaseManager.UpdateUserNameAsync("Users", App.AuthClient.User.Uid, name);

            if (!string.IsNullOrEmpty(uploadedImageUrl))
            {
                await firebaseDatabaseManager.UpdateUserImageAsync("Users", App.AuthClient.User.Uid, uploadedImageUrl);
            }

            string fontSize = "Mediana";
            switch (selectedSizeIndex)
            {
                case 0:
                    fontSize = "Pequeña";
                    break;
                case 1:
                    fontSize = "Mediana";
                    break;
                case 2:
                    fontSize = "Grande";
                    break;
            }
            var userConfig = new Dictionary<string, object>
            {
                ["fontSize"] = fontSize,
                ["language"] = (selectedLanguageIndex == 0) ? "es" : "en",
                ["theme"] = (selectedThemeIndex == 0) ? "light" : "dark"
            };
            await firebaseDatabaseManager.UpdateConfigUserAsync(App.AuthClient.User.Uid, userConfig);

            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnSuccessUploadPerfil"].ToString() ?? "Perfil actualizado con éxito");
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnFailUpdatePerfil"].ToString() ?? "Fallo al actualizar el perfil: ") + ex.Message);
        }
        finally
        {
            messageManager.HideLoading();
        }
    }

    /// <summary>
    /// Sube la imagen a firestore y luego actualiza el perfil
    /// <list type="number">
    /// <item><param name="name">Nuevo nombre del perfil</param></item>
    /// </list>
    /// </summary>
    private async Task UploadImage(string name)
    {
        messageManager.ShowLoading(LocalizationResourceManager.Instance["msnUploadImage"].ToString() ?? "Subiendo imagen...");
        try
        {
            if (selectedImage != null) {
                var stream = await selectedImage.OpenReadAsync();
                await firebaseStorageManager.PushItemAsync("Profile", App.AuthClient.User.Uid, stream);

                var uploadedImageUrl = await firebaseStorageManager.GetItemDownloadUrlAsync("Profile", App.AuthClient.User.Uid);
                await UpdateProfile(name, uploadedImageUrl.ToString());
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error",(LocalizationResourceManager.Instance["msnFailUploadImage"].ToString() ?? "Fallo al actualizar la imagen:") +"\n {ex.Message}", "ok");
        }
        finally
        {
            selectedImage = null;
            messageManager.HideLoading();
        }
    }

    /// <summary>
    /// Vuelve al perfil sin hacer modificaciones
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void doCancelar_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Profile());
        var previousPage = Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];
        Navigation.RemovePage(previousPage);
    }

    /// <summary>
    /// Actualiza el tamaño de fuente con el nuevo valor seleccionado
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void FontPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
       
        if (picker.SelectedIndex == -1) return;

        selectedSizeIndex = picker.SelectedIndex;
        FontSizeManager.Instance.UpdateFontSizes(selectedSizeIndex);
    }

    /// <summary>
    /// Actualiza el tema con el nuevo valor seleccionado
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        
        if (picker.SelectedIndex == -1) return;

        selectedThemeIndex = picker.SelectedIndex;
        ThemeManager.Instance.UpdateThemeColors(selectedThemeIndex);
    }

    /// <summary>
    /// Actualiza el idioma con el nuevo valor seleccionado
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void LanguagePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
       
        if (picker.SelectedIndex == -1) return;

        selectedLanguageIndex = picker.SelectedIndex;
        var switchToCulture = (selectedLanguageIndex == 0) ? new CultureInfo("es") : new CultureInfo("en");

        LocalizationResourceManager.Instance.SetCulture(switchToCulture);
    }
}