using AppProyectoFinal.Data;
namespace AppProyectoFinal.Vistas;

public partial class UserModal : ContentPage
{
    //Atributos y Manejadores
    public delegate void ModalResultHandler(ModelUser user, string result);
    public event ModalResultHandler OnResult;

    private ModelUser _selectedUser;
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    /// <summary>
    /// Modal donde se muestran las opciones a realizar sobre un usuario
    /// </summary>
    public UserModal(ModelUser selectedUser)
    {
        InitializeComponent();
        BindingContext = this;
        _selectedUser = selectedUser;
        description.Text = LocalizationResourceManager["expChangeRol2"].ToString() +$"{selectedUser.Email}?";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce($"¿Quiere dar el rango de autor al usuario: {_selectedUser.Email}?");
    }

    /// <summary>
    /// Método para guardar la opción seleccionada y cerrar el modal
    /// </summary>
    private async void OnAcceptClicked(object sender, EventArgs e)
    {
        OnResult?.Invoke(_selectedUser, "Aceptar");
        await CloseModal();
    }

    /// <summary>
    /// Método para guardar la opción seleccionada y cerrar el modal
    /// </summary>
    private async void OnDenyClicked(object sender, EventArgs e)
    {
        OnResult?.Invoke(_selectedUser, "Denegar");
        await CloseModal();
    }

    /// <summary>
    /// Método para guardar la opción seleccionada y cerrar el modal
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        OnResult?.Invoke(_selectedUser, "Cancelar");
        await CloseModal();
    }

    /// <summary>
    /// Método para cerrar el modal
    /// </summary>
    private async Task CloseModal()
    {
        await Navigation.PopModalAsync();
    }
}
