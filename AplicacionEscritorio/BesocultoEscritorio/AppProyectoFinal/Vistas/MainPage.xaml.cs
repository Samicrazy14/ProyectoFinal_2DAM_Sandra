using AppProyectoFinal.Data;
using AppProyectoFinal.Vistas;
using System.Globalization;

namespace AppProyectoFinal
{
    public partial class MainPage : ContentPage
    {
        //Manejadores de BBDD y mensajes
        private MessageManager messageManager;
        private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
        private LogManager logManager = new LogManager();
        public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

        /// <summary>
        /// Página de inicio de la aplicación
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            messageManager = new MessageManager(mainPage);
            checkLog();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Pone la configuración por defecto
            FontSizeManager.Instance.UpdateFontSizes(1);
            ThemeManager.Instance.UpdateThemeColors(0);
            LocalizationResourceManager.Instance.SetCulture(new CultureInfo("es"));
        }


        /// <summary>
        /// Método para ir al Login
        /// </summary>
        private void goLogin_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Login());
        }

        /// <summary>
        /// Método para ir a la página principal de la aplicación
        /// </summary>
        private void goInicio_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DashBoardPublicBooks());
        }

        /// <summary>
        /// Método que comprueba si se había logeado el usuario y si es asi redirige al usuario
        /// </summary>
        private async void checkLog() {
            try
            {
                var credentials = await logManager.ReadUserCredentialsAsync();
                if (credentials != null) {
                    var auth = await App.AuthClient.SignInWithEmailAndPasswordAsync(credentials.Value.Email, credentials.Value.Password);
                    var firebaseToken = auth.User.Uid;

                    await CheckUser(firebaseToken);
                }
            }
            catch (Exception ex)
            {
                // Mostrar el error al usuario
                await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnErrorCreden"].ToString() ?? "Error al leer credenciales: ") +ex.Message);
            }
        }

        /// <summary>
        /// Método que redirige al usuario a la página principal comprobando su rol
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
                    //Pone la configuración del usuario
                    var config = await firebaseDatabaseManager.GetConfigUserAsync(userId);

                    if (config != null)
                    {
                        int fontSize = 1;
                        switch (config.fontSize)
                        {
                            case "Pequeña":
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
                    else
                    {
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
}
