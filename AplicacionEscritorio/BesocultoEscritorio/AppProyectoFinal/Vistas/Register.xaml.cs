using AppProyectoFinal.Data;
using Firebase.Auth;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AppProyectoFinal.Vistas;

public partial class Register : ContentPage
{
    //Atributos por defecto
    private string name = "";
    private string email = "";
    private string password = "";

    private bool _isPasswordVisible = false;
    public bool IsPasswordVisible
    {
        get => _isPasswordVisible;
        set
        {
            if (_isPasswordVisible != value)
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isPasswordVisible2 = false;
    public bool IsPasswordVisible2
    {
        get => _isPasswordVisible2;
        set
        {
            if (_isPasswordVisible2 != value)
            {
                _isPasswordVisible2 = value;
                OnPropertyChanged();
            }
        }
    }

    //Manejadores de BBDD y mensajes
    private MessageManager messageManager;
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    /// <summary>
    /// Página donde se puede registrar un nuevo usuario
    /// </summary>
    public Register()
	{
		InitializeComponent();
        BindingContext = this;
        messageManager = new MessageManager(mainPage);
        passwordEntry.IsPassword = true;
        cPasswordEntry.IsPassword = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de registro, por favor introduzca sus datos");
    }

    /// <summary>
    /// Método para ver u ocultar la contraseña
    /// </summary>
    private void OnEyeButtonClicked(object sender, EventArgs e)
    {
        // Alternar visibilidad de la contraseña
        IsPasswordVisible = !IsPasswordVisible;
        passwordEntry.IsPassword = !IsPasswordVisible;

        // Cambiar el icono según el estado de visibilidad
        ((ImageButton)sender).Source = IsPasswordVisible
            ? ThemeManager.Instance.OpenEyeIcon
            : ThemeManager.Instance.CloseEyeIcon;
    }
    
    /// <summary>
    /// Método para ver u ocultar la contraseña confirmada
    /// </summary>
    private void OnEyeButtonClicked2(object sender, EventArgs e)
    {
        // Alternar visibilidad de la contraseña
        IsPasswordVisible2 = !IsPasswordVisible2;
        cPasswordEntry.IsPassword = !IsPasswordVisible2;

        // Cambiar el icono según el estado de visibilidad
        ((ImageButton)sender).Source = IsPasswordVisible2
            ? ThemeManager.Instance.OpenEyeIcon
            : ThemeManager.Instance.CloseEyeIcon;
    }

    /// <summary>
    /// Método para iniciar el registro de un nuevo usuario
    /// </summary>
    private async void doRegistro_Clicked(object sender, EventArgs e)
    {
        if (await ValidateData())
        {
            await CreateUserAccount();
        }
    }

    /// <summary>
    /// Método que comprueba la validez de los datos introducidos para el nuevo usuario
    /// </summary>
    /// <returns>Tarea asíncrona que devuelve un booleano con el resultado de la comprobación de datos</returns>
    private async Task<bool> ValidateData()
    {
        name = nameEntry.Text?.Trim() ?? "";
        email = emailEntry.Text?.Trim() ?? "";
        password = passwordEntry.Text?.Trim() ?? "";
        var cPassword = cPasswordEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(name))
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnName"].ToString() ?? "Introduce tu nombre");
            return false;
        }
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnEmail"].ToString() ?? "Introduce un email válido");
            return false;
        }
        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnPassword"].ToString() ?? "Introduce tu contraseña (6 caracteres mínimo)");
            return false;
        }
        if (string.IsNullOrEmpty(cPassword))
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnCPassword"].ToString() ?? "Confirma tu contraseña");
            return false;
        }
        if (password != cPassword)
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnCheckPassword"].ToString() ?? "Las contraseñas no coinciden"); 
            return false;
        }
        return true;
    }

    /// <summary>
    /// Método que crea el nuevo usuario registrado.
    /// </summary>
    private async Task CreateUserAccount()
    {
        try
        {
            messageManager.ShowLoading(LocalizationResourceManager.Instance["msnCreateUser"].ToString() ?? "Creando cuenta...");

            var authResult = await App.AuthClient.CreateUserWithEmailAndPasswordAsync(email, password);
            var user = authResult.User;
            if (user != null)
            {
                await UpdateUserInfo(user);
            }
        }
        catch (FirebaseAuthException ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnErrorCreateUser"].ToString() ?? "Fallo al crear la cuenta: ") + ex.Message);
        }
        finally
        {
            messageManager.HideLoading();
        }
    }

    /// <summary>
    /// Método que sube la información del nuevo usuario a la BBDD.
    /// <list type="number">
    /// <item><param name="user">Objeto de la clase User con la información del usuario</param></item>
    /// </list>
    /// </summary>
    private async Task UpdateUserInfo(User user)
    {
        try
        {
            messageManager.ShowLoading(LocalizationResourceManager.Instance["msnSaveUser"].ToString() ?? "Guardando la información del usuario...");
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var uid = user.Uid;

            var userData = new Dictionary<string, object>
            {
                ["uid"] = uid,
                ["email"] = email,
                ["name"] = name,
                ["profileImage"] = "",
                ["userType"] = "user",
                ["timestamp"] = timestamp
            };

            await firebaseDatabaseManager.UpdateItemAsync("Users", uid, userData);

            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnSuccesUser"].ToString() ?? "Usuario creado con éxito");
            await Navigation.PushAsync(new Login());
            Navigation.RemovePage(this);
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnFailSaveUser"].ToString() ?? "Fallo al guardar al usuario: ") + ex.Message);
        }
        finally
        {
            messageManager.HideLoading();
        }
    }
}