using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AppProyectoFinal.Data;

namespace AppProyectoFinal.Vistas;

public partial class Login : ContentPage, INotifyPropertyChanged
{

    //Manejadores de BBDD y mensajes
    private MessageManager messageManager;
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    private LogManager logManager = new LogManager();
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

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

    /// <summary>
    /// P�gina donde se puede logear un usuario
    /// </summary>
    public Login()
    {
        InitializeComponent();
        BindingContext = this;
        messageManager = new MessageManager(mainPage);
        PasswordEntry.IsPassword = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la p�gina de inicio de sesi�n, introduzca su usuario");
    }

    /// <summary>
    /// M�todo que comprueba los datos introducidos e inicia el login del usuario
    /// </summary>
    private async void doLogin_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnEmail"].ToString() ?? "Introduzca un email v�lido");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["phPassword"].ToString() ?? "introduzca su contrase�a");
            return;
        }

        await LoginUser(email, password);
    }

    /// <summary>
    /// M�todo que te dirige a la p�gina de recuperaci�n de contrase�a
    /// </summary>
    private void goForgotPassword_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ForgotPassword());
    }

    /// <summary>
    /// M�todo que te dirige a la p�gina de registro de nuevos usuarios
    /// </summary>
    private void goRegistro_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Register());
    }

    /// <summary>
    /// M�todo para ver u ocultar la contrase�a
    /// </summary>
    private void OnEyeButtonClicked(object sender, EventArgs e)
    {
        // Alternar visibilidad de la contrase�a
        IsPasswordVisible = !IsPasswordVisible;
        PasswordEntry.IsPassword = !IsPasswordVisible;

        // Cambiar el icono seg�n el estado de visibilidad
        ((ImageButton)sender).Source = IsPasswordVisible
            ? ThemeManager.Instance.OpenEyeIcon
            : ThemeManager.Instance.CloseEyeIcon;
    }

    /// <summary>
    /// M�todo para logear al usuario con los datos introducidos
    /// <list type="number">
    /// <item><param name="email">Email con el que se logea</param></item>
    /// <item><param name="password">Contrase�a introducida por el usuario</param></item>
    /// </list>
    /// </summary>
    private async Task LoginUser(string email, string password)
    {
        try
        {
            messageManager.ShowLoading(LocalizationResourceManager.Instance["msnLogin"].ToString() ?? "Iniciando sesi�n...");

            var auth = await App.AuthClient.SignInWithEmailAndPasswordAsync(email, password);
            var firebaseToken = auth.User.Uid;

            await CheckUser(firebaseToken);
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnFailLog"].ToString() ?? "Fallo al iniciar sesi�n: ") + ex.Message);
        }
        finally
        {
            messageManager.HideLoading();
        }
    }

    /// <summary>
    /// M�todo que redirige al usuario a la p�gina principal comprobando su rol
    /// <list type="number">
    /// <item><param name="userId">Id del usuario</param></item>
    /// </list>
    /// </summary>
    private async Task CheckUser(string userId)
    {
        try
        {
            messageManager.ShowLoading(LocalizationResourceManager.Instance["msnCheckUser"].ToString() ?? "Comprobando usuario...");

            var user = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", userId);

            if (user != null)
            {
                string password = PasswordEntry.Text?.Trim() ?? "";
                await logManager.SaveUserCredentialsAsync(user.Email, password);

                //Pone la configuraci�n del usuario
                var config = await firebaseDatabaseManager.GetConfigUserAsync(userId); 
                
                if (config != null)
                {
                    int fontSize = 1;
                    switch (config.fontSize) {
                        case "Peque�a":
                            fontSize = 0;
                            break;
                        case "Mediana":
                            fontSize = 1;
                            break;
                        case "Grande":
                            fontSize = 2;
                            break;
                    }
                    FontSizeManager.Instance.UpdateFontSizes(fontSize);

                    int theme = (config.theme == "light") ? 0 : 1;
                    ThemeManager.Instance.UpdateThemeColors(theme);

                    CultureInfo language = (config.language == "es") ? new CultureInfo("es") : new CultureInfo("en");
                    LocalizationResourceManager.Instance.SetCulture(language);
                }
                else {
                    FontSizeManager.Instance.UpdateFontSizes(1);
                    ThemeManager.Instance.UpdateThemeColors(0);
                    LocalizationResourceManager.Instance.SetCulture(new CultureInfo("es"));
                }

                if (user.UserType != "admin")
                {
                    await Navigation.PushAsync(new DashBoardPublicBooks());
                }
                else
                {
                    await Navigation.PushAsync(new DashBoardCategories());
                }

                Navigation.RemovePage(this);
            }
            else
            {
                await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnNoUserFound"].ToString() ?? "Usuario no encontrado");
            }
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnFailUser"].ToString() ?? "Fallo al comprobar el usuario: ") + ex.Message);
        }
        finally
        {
            messageManager.HideLoading();
        }
    }
}
