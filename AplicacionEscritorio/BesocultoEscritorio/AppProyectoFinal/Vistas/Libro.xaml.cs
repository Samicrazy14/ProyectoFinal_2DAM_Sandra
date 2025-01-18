using AppProyectoFinal.Data;
using Syncfusion.PdfToImageConverter;
using System.Collections.ObjectModel;

namespace AppProyectoFinal.Vistas;

public partial class Libro : ContentPage
{
    //Manejadores de BBDD y mensajes
    private MessageManager messageManager;
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    private FirebaseStorageManager firebaseStorageManager = new FirebaseStorageManager();
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    //Atributos
    private string bookId;
    private string type;
    private string urlEpub;
    private bool isInMyFavorite;
    private string comment;
    private ContentView commentDialog;
    public ObservableCollection<ModelComment> commentList { get; set; }

    /// <summary>
    /// Página donde se puede ver los detalles de un libro
    /// </summary>
    public Libro(string bookId, string type)
    {
        InitializeComponent();

        messageManager = new MessageManager(mainPage);

        this.bookId = bookId;
        this.type = type;

        commentList = new ObservableCollection<ModelComment>();

        //Comprobar si el usuario esta logado
        if (App.AuthClient.User != null)
        {
            CheckIsFavorite();
            btFav.IsVisible = true;
            btComment.IsVisible = true;
        }
        else 
        {
            btFav.IsVisible = false;
            btComment.IsVisible = false;
        }

        IncrementBookViewCount(bookId, type);
        LoadBookDetails(type, bookId);
        ShowComments(type);

        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de detalles del libro, consulte la información y los comentarios de la comunidad disponibles");
    }

    /// <summary>
    /// Comprueba si es un libro que el usuario tiene guardado como favorito
    /// </summary>
    private async Task CheckIsFavorite()
    {
        try
        {
            var snapshot = await firebaseDatabaseManager.GetFavoriteBookAsync(bookId, App.AuthClient.User.Uid);

            isInMyFavorite = snapshot != null;

            if (isInMyFavorite)
            {
                goFav.Source = ImageSource.FromFile("ic_favourite_filled_white.png");
                goFavLabel.Text = LocalizationResourceManager.Instance["txtFavDel"].ToString() ?? "Eliminar de favoritos";
            }
            else
            {
                goFav.Source = ImageSource.FromFile("ic_favourite_border.png");
                goFavLabel.Text = LocalizationResourceManager.Instance["txtFavAdd"].ToString() ?? "Añadir a favoritos";
            }
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnCheckFav"].ToString() ?? "Error al comprobar si es favorito: ") + ex.Message);
        }
    }

    /// <summary>
    /// Aumenta el valor de visualizaciones del libro
    /// <list type="number">
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// <item><param name="type">Tipo del libro</param></item>
    /// </list>
    /// </summary>
    public async Task IncrementBookViewCount(string bookId, string type)
    {
        try
        {
            var book = await firebaseDatabaseManager.GetItemByIdAsync<ModelPdf>(type, bookId);
            if (book == null)
            {
                return;
            }

            // Incrementar viewsCount
            long newViewsCount = book.viewsCount + 1;

            // Actualizar solo el campo viewsCount en la base de datos
            await firebaseDatabaseManager.UpdateViewsCountAsync(type, bookId, newViewsCount);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar viewsCount: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Carga la información del libro
    /// <list type="number">
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// <item><param name="type">Tipo del libro</param></item>
    /// </list>
    /// </summary>
    public async Task LoadBookDetails(string type, string bookId)
    {
        try
        {
            // Leer los datos del libro una sola vez
            var snapshot = await firebaseDatabaseManager.GetItemByIdAsync<ModelPdf>(type, bookId);

            if (snapshot == null)
            {
                return;
            }

            // Obtener los valores del libro
            var categoryId = snapshot.categoryId.ToString() ?? string.Empty;
            var description = snapshot.description.ToString() ?? string.Empty;
            var timestamp = snapshot.timestamp.ToString() ?? "0";
            var title = snapshot.title.ToString() ?? string.Empty;
            var url = snapshot.url.ToString() ?? string.Empty;
            var viewsCount = snapshot.viewsCount.ToString() ?? "0";
            var date = snapshot.FormattedDate;
            var pagesCount = snapshot.pagecount.ToString() ?? "~";

            if (type != "GutembergBooks")
            {
                await LoadCategoryForBook(categoryId);

                await LoadPdfImage(bookId);
            }
            else
            {
                CategoryLabel.Text = categoryId;

                var imageUrl = snapshot.imagenUrl.ToString() ?? string.Empty;
                urlEpub = url;

                try
                {
                    // Cargar la imagen usando ImageSource
                    coverImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                }
                catch (Exception)
                {
                    coverImage.Source = "default_book_cover.png"; // Ruta de la imagen predeterminada
                }
            }

            // Asignar los valores a los elementos visuales
            TitleLabel.Text = title;
            DescriptionLabel.Text = description;
            ViewsCountLabel.Text = viewsCount;
            DateLabel.Text = date;
            PagesLabel.Text = pagesCount;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar los detalles del libro: {ex.Message}");
        }
    }

    /// <summary>
    /// Carga la información de la categoría correspondiente al libro
    /// <list type="number">
    /// <item><param name="categoryId">Identificador de la categoría del libro</param></item>
    /// </list>
    /// </summary>
    private async Task LoadCategoryForBook(string categoryId)
    {
        try
        {
            var categoryData = await firebaseDatabaseManager.GetItemByIdAsync<ModelCategory>("Categories", categoryId);

            if (categoryData != null)
            {
                CategoryLabel.Text = categoryData.Category;
            }
            else
            {
                CategoryLabel.Text = LocalizationResourceManager.Instance["txtNoCategoria"].ToString() ?? "Categoría desconocida";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar categoría {categoryId}: {ex.Message}");
            CategoryLabel.Text = LocalizationResourceManager.Instance["txtNoCategoria"].ToString() ?? "Categoría desconocida";
        }
    }

    /// <summary>
    /// Carga la portada del libro
    /// <list type="number">
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// </list>
    /// </summary>
    public async Task LoadPdfImage(string bookId)
    {
        try
        {
            // Ubicación de la imagen en el directorio de caché
            string tempImagePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, $"cover_{bookId}.png");
            if (File.Exists(tempImagePath))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // Intentar cargar la imagen desde el archivo
                    coverImage.Source = ImageSource.FromFile(tempImagePath);
                });
                return;
            }

            // Obtener la URL de descarga desde Firebase Storage
            var downloadUrl = await firebaseStorageManager.GetItemDownloadUrlAsync("Books", bookId);

            using (var httpClient = new HttpClient())
            {
                byte[] pdfBytes = await httpClient.GetByteArrayAsync(downloadUrl);

                // Verificar que tenemos datos del PDF
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    throw new Exception("No se pudieron descargar los datos del PDF");
                }

                using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
                {
                    // Configurar el convertidor con opciones específicas
                    PdfToImageConverter converter = new PdfToImageConverter();


                    converter.Load(pdfStream);

                    // Convertir específicamente a PNG
                    using (Stream imageStream = converter.Convert(0, false, false))
                    {

                        // Asegurarnos de que estamos al inicio del stream
                        imageStream.Position = 0;

                        using (FileStream fileStream = File.Create(tempImagePath))
                        {
                            await imageStream.CopyToAsync(fileStream);
                        }

                        // Verificar que el archivo existe y tiene tamaño
                        if (File.Exists(tempImagePath))
                        {
                            var fileInfo = new FileInfo(tempImagePath);
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                // Intentar cargar la imagen desde el archivo
                                coverImage.Source = ImageSource.FromFile(tempImagePath);
                            });
                        }
                        else
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                Console.WriteLine("No se pudo crear el archivo de imagen");
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Console.WriteLine($"Error: {ex.Message}");
            });
        }

    }

    /// <summary>
    /// Navegación hacia la lectura del libro
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void goLectura_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Lectura(bookId, type, null));
    }

    /// <summary>
    /// Carga los comentarios del libro
    /// <list type="number">
    /// <item><param name="type">Tipo del libro</param></item>
    /// </list>
    /// </summary>
    private async Task ShowComments(string type)
    {
        try
        {
            var coments = await firebaseDatabaseManager.GetCommentsAsync(type, bookId);

            foreach (var coment in coments)
            {
                try
                {
                    var comentData = coment.Object;

                    // Asegurarse de que los campos críticos no sean nulos
                    comentData.comment = string.IsNullOrEmpty(comentData.comment) ? "" : comentData.comment;

                    await LoadUserDetailsAsync(comentData);

                    commentList.Add(comentData);
                }
                catch (Exception ex)
                {
                    // Log del error para este comentario específico
                    Console.WriteLine($"Error al procesar comentario: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            // Log del error para este libro
            Console.WriteLine($"Error al procesar libro: {ex.Message}");
        }
    }

    /// <summary>
    /// Carga los datos asociados al usuario que ha hecho un comentario
    /// <list type="number">
    /// <item><param name="model">Objeto con la información del comentario</param></item>
    /// </list>
    /// </summary>
    private async Task LoadUserDetailsAsync(ModelComment model)
    {
        string uid = model.uid;
        try
        {
            var snapshot = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", uid);

            if (snapshot != null)
            {
                string name = snapshot.Name ?? string.Empty;
                string profileImage = snapshot.ProfileImage.ToString() ?? string.Empty;

                model.Name = name;

                if (!string.IsNullOrEmpty(profileImage))
                {
                    model.CoverImage = ImageSource.FromUri(new Uri(profileImage));
                }
                else
                {
                    model.CoverImage = ImageSource.FromFile("ic_person_notnight.png");
                }
            }
        }
        catch (Exception ex)
        {
            model.CoverImage = ImageSource.FromFile("ic_person_notnight.png");
        }
    }

    /// <summary>
    /// Navegación hacia el diálogo de creación de comentarios
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void goComment_Clicked(object sender, EventArgs e)
    {
        if (App.AuthClient.User == null)
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtNoLogin"].ToString() ?? "No estas logeado");
        }
        else
        {
            AddCommentDialogAsync();
        }
    }

    /// <summary>
    /// Crea el diálogo para introducir y añadir comentarios
    /// </summary>
    public async void AddCommentDialogAsync()
    {
        string comentary = await DisplayPromptAsync(
           LocalizationResourceManager.Instance["msnNewComment"].ToString() ?? "Nuevo Comentario",
           LocalizationResourceManager.Instance["txtComent"].ToString() ?? "Introduce un comentario:",
           placeholder: LocalizationResourceManager.Instance["phComent"].ToString() ?? "Escribe tu comentario...",
           maxLength: 500,
           keyboard: Keyboard.Text
        );

        if (!string.IsNullOrWhiteSpace(comentary))
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtAddComment"].ToString() ?? "Añadiendo comentario");

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var hashMap = new Dictionary<string, object>
            {
                ["bookId"] = bookId,
                ["comment"] = comentary,
                ["id"] = timestamp,
                ["timestamp"] = timestamp,
                ["uid"] = App.AuthClient.User.Uid
            };

            try
            {
                await firebaseDatabaseManager.AddCommentAsync(type, bookId, timestamp, hashMap);
                await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnComentAdded"].ToString() ?? "Comentario añadido");
                commentList.Clear();
                await ShowComments(type);
            }
            catch (Exception e)
            {
                await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnFailComment"].ToString() ?? "Fallo al añadir el comentario: ") + e.Message);
            }
        }
    }

    /// <summary>
    /// Metodo para el control de eliminar comentarios
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void OnCommentSelected(object sender, SelectionChangedEventArgs e) {
        // Obtiene el elemento seleccionado (book)
        var selectedComment = e.CurrentSelection.FirstOrDefault() as ModelComment;
        if (selectedComment == null || App.AuthClient.User == null) return;

        var user = await firebaseDatabaseManager.GetItemByIdAsync<ModelUser>("Users", App.AuthClient.User.Uid);

        if (user != null && (App.AuthClient.User.Uid == selectedComment.uid|| user.UserType == "admin"))
        {
            DeleteCommentDialog(selectedComment);
        }
        CommentsCollection.SelectedItem = null;
    }

    /// <summary>
    /// Elimina el comentario de la BBDD
    /// <list type="number">
    /// <item><param name="comment">Comentario a eliminar</param></item>
    /// </list>
    /// </summary>
    private void DeleteCommentDialog(ModelComment comment)
    {
        var bookId = comment.bookId;
        var commentId = comment.id;

        // Mostrar el diálogo de confirmación
        DisplayAlert(LocalizationResourceManager.Instance["msnDelComment"].ToString() ?? "Eliminar comentario",
            LocalizationResourceManager.Instance["msnConfirmDelComment"].ToString() ?? "¿Estás seguro de que deseas eliminar este mensaje?",
            LocalizationResourceManager.Instance["txtEliminar"].ToString() ?? "Eliminar",
            LocalizationResourceManager.Instance["btCancelar"].ToString() ?? "Cancelar")
            .ContinueWith(async t =>
            {
                if (t.Result)
                {
                    try
                    {
                        await firebaseDatabaseManager.DeleteUserActionAsync(type, bookId, "Comments", commentId);
                        await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnComentDeleted"].ToString() ?? "Comentario eliminado");
                        commentList.Clear();
                        await ShowComments(type);
                    }
                    catch (Exception ex)
                    {
                        await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnFailDelComent"].ToString() ?? "Fallo al eliminar el comentario: ") + ex.Message);
                    }
                }
            });
    }

    /// <summary>
    /// Controla las opciones de eliminar/añadir a favoritos
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private async void goFav_Clicked(object sender, EventArgs e)
    {

        if (App.AuthClient.User == null)
        {
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtNoLogin"].ToString() ?? "No estas logeado");
        }
        else
        {
            if (isInMyFavorite)
            {
                await RemoveFromFavoriteAsync();
            }
            else
            {
                await AddToFavoriteAsync();
            }
        }
    }

    /// <summary>
    /// Método para añadir  el libro a favoritos guardandolo en BBDD
    /// </summary>
    public async Task AddToFavoriteAsync()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var hashMap = new ModelFav(bookId, timestamp, type);

        try
        {
            await firebaseDatabaseManager.AddFavoriteAsync(bookId, hashMap, App.AuthClient.User.Uid);
            goFav.Source = ImageSource.FromFile("ic_favourite_filled_white.png");
            goFavLabel.Text = LocalizationResourceManager.Instance["txtFavDel"].ToString() ?? "Eliminar de favoritos";
            isInMyFavorite = true;
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnAddedFav"].ToString() ?? "Añadido a favoritos");
        }
        catch (Exception e)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnFailAddedFav"].ToString() ?? "Fallo al añadir a favoritos: ") + e.Message);
        }
    }


    /// <summary>
    /// Método para eliminar el libro de favoritos y guardarlo en BBDD
    /// </summary>
    public async Task RemoveFromFavoriteAsync()
    {
        try
        {
            await firebaseDatabaseManager.DeleteUserActionAsync("Users", App.AuthClient.User.Uid, "Favorites", bookId);
            goFav.Source = ImageSource.FromFile("ic_favourite_border.png");
            goFavLabel.Text = LocalizationResourceManager.Instance["txtFavAdd"].ToString() ?? "Añadir a favoritos";
            isInMyFavorite = false;
            await messageManager.ShowMessage(LocalizationResourceManager.Instance["msnDeletedFav"].ToString() ?? "Eliminado de favoritos");
        }
        catch (Exception e)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnFailDeletedFav"].ToString() ?? "Fallo al eliminar de favoritos: ") + e.Message);
        }
    }

}