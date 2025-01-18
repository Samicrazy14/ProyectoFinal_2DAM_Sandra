using AppProyectoFinal.Data;

namespace AppProyectoFinal.Vistas;

public partial class Ayuda : ContentPage
{
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    public Ayuda()
	{
		InitializeComponent();
        BindingContext = this;
        InitializeData();
    }

    /// <summary>
    /// Configura la vista de la página en función del tipo de usuario
    /// </summary>
    private async Task InitializeData()
    {
        if (App.AuthClient.User != null)
        {
            ModelUser user = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", App.AuthClient.User.Uid);
            if (user.UserType == "admin")
            {
                adminHelp.IsVisible = true;
                userHelp.IsVisible = false;
            } else
            {
                adminHelp.IsVisible = false;
                userHelp.IsVisible = true;
            }
        }
        else
        {
            adminHelp.IsVisible = false;
            userHelp.IsVisible = true;
        }
    }

    /// <summary>
    /// Método para que se ejecuta cuando aparece la ContentPage.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de ayuda donde se abarca la navegación y funcionalidad de la app");
    }
}