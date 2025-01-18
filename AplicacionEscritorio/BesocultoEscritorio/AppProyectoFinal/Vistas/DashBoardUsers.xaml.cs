using AppProyectoFinal.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppProyectoFinal.Vistas;

public partial class DashBoardUsers : ContentPage, INotifyPropertyChanged
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
    public ObservableCollection<ModelUser> AllUsers { get; set; }
    public ObservableCollection<ModelUser> rawUsers{ get; set; }


    private bool _isUserToggled = false;
    public bool isUserToggled
    {
        get => _isUserToggled;
        set
        {
            if (_isUserToggled != value)
            {
                _isUserToggled = value;
                OnPropertyChanged(nameof(isUserToggled));
            }
        }
    }
    public new event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public DashBoardUsers()
	{
		InitializeComponent();

        // Inicializa colecciones y dependencias
        AllUsers = new ObservableCollection<ModelUser>();
        rawUsers = new ObservableCollection<ModelUser>();
        httpClient = new HttpClient();
        messageManager = new MessageManager(mainPage, mainlayout);
        BindingContext = this;

        #if WINDOWS
          Microsoft.Maui.Handlers.SwitchHandler.Mapper.AppendToMapping("NoLabel", (handler, View) =>
          {
              handler.PlatformView.OnContent = null;
              handler.PlatformView.OffContent = null;

              // remove the padding around the switch as well
              // handler.PlatformView.MinWidth = 0;
          });
        #endif
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de gestión de usuarios para administrador");
        AllUsersCollection.SelectedItem = null;
        UpdateUserCollection();
    }


    /// <summary>
    /// Metodo que filtra los libros por título usando el texto introducido en la barra de busqueda.
    /// </summary>
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = RemoveDiacritics(e.NewTextValue?.ToLower() ?? string.Empty);
        AllUsers.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var user in rawUsers)
            {
                AllUsers.Add(user);
            }
        }
        else
        {
            // Filtra los elementos de rawBooks por el título
            foreach (var user in rawUsers.Where(b => RemoveDiacritics(b.Name.ToLower()).Contains(searchText)))
            {
                AllUsers.Add(user);
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
    /// Carga todos los usuarios de Firebase y los añade a la colección AllUsers.
    /// </summary>
    private async Task LoadUsers()
    {
        try
        {
            var users = await firebaseDatabaseManager.GetAllCollectionAsync<ModelUser>("Users");

            foreach (var user in users)
            {
                try
                {
                    var userData = user.Object;

                    // Saltar usuarios con UserType igual a "admin"
                    if (userData.UserType.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string name = userData.Name ?? string.Empty;
                    string profileImage = userData.ProfileImage.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(profileImage))
                    {
                        userData.CoverImage = ImageSource.FromUri(new Uri(profileImage));
                    }
                    else
                    {
                        userData.CoverImage = ImageSource.FromFile("ic_person_notnight.png");
                    }

                    AllUsers.Add(userData);
                    rawUsers.Add(userData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar usuario: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar usuarios de firebase: {ex.Message}");
        }
    }

    /// <summary>
    /// Método para eliminar un usuario
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnDeleteUserClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton imageButton && imageButton.BindingContext is ModelUser selectedUser)
        {
            await DisplayAlert(
                LocalizationResourceManager.Instance["txtDeleteUser"].ToString() ?? "Borrar Usuario",
                (LocalizationResourceManager.Instance["txtAskDeleteUser"].ToString() ?? "¿Estás seguro de que desea eliminar al usuario: ") + selectedUser.Email +"?",
                LocalizationResourceManager.Instance["txtConfirmar"].ToString() ?? "Confirmar",
                LocalizationResourceManager.Instance["btCancelar"].ToString() ?? "Cancelar"
            ).ContinueWith(async (task) =>
            {
                if (task.Result)
                {
                    // Acción para Confirmar
                    await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtDeletingUser"].ToString() ?? "Eliminando usuario...");
                    await firebaseDatabaseManager.DeleteItemAsync("Users", selectedUser.Uid);
                    UpdateUserCollection();
                }
                // En caso de Cancelar, no es necesario hacer nada, ya que simplemente se cierra el diálogo.
            });
        }
    }

    /// <summary>
    /// Método para cambiar el rol de un usuario
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnOptionsUserClicked(object sender, EventArgs e)
    {

        if (sender is ImageButton imageButton && imageButton.BindingContext is ModelUser selectedUser)
        {
            var modalPage = new UserModal(selectedUser);
            modalPage.OnResult += ModalPage_OnResult;
            await Navigation.PushModalAsync(modalPage);
        }
    }

    /// <summary>
    /// Método que espera el resultado del modal del cambio de rol de un usuario
    /// <list type="number">
    /// <item><param name="user">Usuario del evento</param></item>
    /// <item><param name="result">Resultado del modal</param></item>
    /// </list>
    /// </summary>
    private async void ModalPage_OnResult(ModelUser user, string result)
    {
        if (result == "Aceptar")
        {
            await firebaseDatabaseManager.UpdateUserTypeAsync("Users", user.Uid, "author");
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtUpdateRol"].ToString() ?? "Rol del usuario actualizado");
        }
        else if (result == "Denegar")
        {
            await firebaseDatabaseManager.UpdateUserTypeAsync("Users", user.Uid, "user");
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtUpdateRol"].ToString() ?? "Rol del usuario actualizado");
        }
    }

    /// <summary>
    /// Alterna la visualización de todos los usarios o pendientes
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void OnSwitchUsersToggled(object sender, ToggledEventArgs e)
    {
        isUserToggled = e.Value;
        UpdateUserCollection();
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
    /// Metodo para actualizar la collecion de usuarios tras un cambio.
    /// </summary>
    private async void UpdateUserCollection()
    {
        if (App.AuthClient.User != null)
        {
            ModelUser user = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", App.AuthClient.User.Uid);
            userLabel.Text = user.Email;
        }
        if (isUserToggled)
        {
            searchBarId.Text = "";
            AllUsers.Clear();
            try
            {
                var users = await firebaseDatabaseManager.GetAllCollectionAsync<ModelUser>("Users");

                foreach (var usuario in users)
                {
                    try
                    {
                        var userData = usuario.Object;
                        if (userData.UserType == "pending user")
                        {
                            string name = userData.Name ?? string.Empty;
                            string profileImage = userData.ProfileImage.ToString() ?? string.Empty;

                            if (!string.IsNullOrEmpty(profileImage))
                            {
                                userData.CoverImage = ImageSource.FromUri(new Uri(profileImage));
                            }
                            else
                            {
                                userData.CoverImage = ImageSource.FromFile("ic_person_notnight.png");
                            }

                            AllUsers.Add(userData);
                            rawUsers.Add(userData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar usuario: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar usuarios de firebase: {ex.Message}");
            }
        }
        else
        {
            AllUsers.Clear();
            await LoadUsers();
        }
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