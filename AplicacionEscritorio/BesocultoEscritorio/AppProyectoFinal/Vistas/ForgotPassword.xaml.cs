using AppProyectoFinal.Data;
using Firebase.Auth;
using System.Text.RegularExpressions;

namespace AppProyectoFinal.Vistas;

public partial class ForgotPassword : ContentPage
{
    //Atributo por defecto
    private string email = "";
    //Manejador de mensajes
    private MessageManager messageManager;
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    /// <summary>
    /// P�gina donde se puede recuperar la contrase�a si se ha perdido
    /// </summary>
    public ForgotPassword()
	{
		InitializeComponent();
        BindingContext = this;
        messageManager = new MessageManager(mainPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la p�gina de recuperaci�n de contrase�a");
    }

    /// <summary>
    /// M�todo para iniciar la recuperaci�n de contrase�a
    /// </summary>
    private async void doRecuperarPassword_Clicked(object sender, EventArgs e)
    {
        if (await ValidateData())
        {
            await RecoverPassword();
        }
    }

    /// <summary>
    /// M�todo que comprueba la validez de los datos introducidos para la recuperaci�n de contrase�a
    /// </summary>
    /// <returns>Tarea as�ncrona que devuelve un booleano con el resultado de la comprobaci�n de datos</returns>
    private async Task<bool> ValidateData()
    {
        email = emailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnEmail"].ToString() ?? "Introduce un email v�lido"); 
            return false;
        }
        return true;
    }

    /// <summary>
    /// M�todo que env�a la nueva contrase�a al correo introducido
    /// </summary>
    private async Task RecoverPassword()
    {
        try
        {
            messageManager.ShowLoading((LocalizationResourceManager.Instance["msnSendEmail"].ToString() ?? "Enviando correo a ")+ email + "...");

            await App.AuthClient.ResetEmailPasswordAsync(email);
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnSendedEmail"].ToString() ?? "Correo enviado a: ") + $"\n {email}");
            await Task.Delay(2000); // Esperar dos segundos antes de volver
            await Navigation.PopAsync();
        }
        catch (FirebaseAuthException ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnSendFail"].ToString() ?? "Fallo al enviar el correo a ") + $"{email}: {ex.Message}");
        }
        finally
        {
            messageManager.HideLoading();
        }
    }
}