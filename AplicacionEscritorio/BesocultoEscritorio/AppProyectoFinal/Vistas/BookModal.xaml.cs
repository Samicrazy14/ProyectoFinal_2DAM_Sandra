using AppProyectoFinal.Data;

namespace AppProyectoFinal.Vistas;

public partial class BookModal : ContentPage
{
    //Atributos y Manejadores
    public delegate void ModalResultHandler(ModelPdf book, string result);
    public event ModalResultHandler OnResult;

    private ModelPdf _selectedBook;
    private bool _isBookToggled;

    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    /// <summary>
    /// Modal donde se muestran las opciones a realizar sobre un libro
    /// </summary>
    public BookModal(ModelPdf selectedBook, bool isBookToggled)
	{
		InitializeComponent();
        BindingContext = this;
        _selectedBook = selectedBook;
        _isBookToggled = isBookToggled;
        Upload.IsVisible = isBookToggled;
        description.Text = LocalizationResourceManager["expActionBook"].ToString() + $" {selectedBook.title}?";
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce($"¿Qué acción quiere realizar sobre el libro: {_selectedBook.title}?");
    }

    /// <summary>
    /// Método para guardar la opción seleccionada y cerrar el modal
    /// </summary>
    private async void OnEditClicked(object sender, EventArgs e)
    {
        OnResult?.Invoke(_selectedBook, "Editar");
        await CloseModal();
    }

    /// <summary>
    /// Método para guardar la opción seleccionada y cerrar el modal
    /// </summary>
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        OnResult?.Invoke(_selectedBook, "Borrar");
        await CloseModal();
    }

    /// <summary>
    /// Método para guardar la opción seleccionada y cerrar el modal
    /// </summary>
    private async void OnUploadClicked(object sender, EventArgs e)
    {
        OnResult?.Invoke(_selectedBook, "Subir");
        await CloseModal();
    }

    /// <summary>
    /// Método para guardar la opción seleccionada y cerrar el modal
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        OnResult?.Invoke(_selectedBook, "Cancelar");
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