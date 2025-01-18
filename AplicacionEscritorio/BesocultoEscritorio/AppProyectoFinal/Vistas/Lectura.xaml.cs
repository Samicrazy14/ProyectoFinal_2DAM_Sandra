using AppProyectoFinal.Data;
using Syncfusion.PdfToImageConverter;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using VersOne.Epub;

namespace AppProyectoFinal.Vistas;

public partial class Lectura : ContentPage, INotifyPropertyChanged
{
    //Manejadores de BBDD y mensajes
    private FirebaseDatabaseManager firebaseDatabaseManager = new FirebaseDatabaseManager();
    private FirebaseStorageManager firebaseStorageManager = new FirebaseStorageManager();
    private MessageManager messageManager;
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    //Atributos
    private string bookId;
    private string type;
    private Stream? pdfStream;
    private double _imageHeight = 900;
    private EpubBook book;
    private List<string> contentParts;
    private double scaleFactor = 1.0;
    private ObservableCollection<ImageSource> _pageImages;
    public ObservableCollection<ImageSource> PageImages
    {
        get => _pageImages;
        set
        {
            if (_pageImages != value)
            {
                _pageImages = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public double ImageHeight
    {
        get => _imageHeight;
        set
        {
            if (_imageHeight != value)
            {
                _imageHeight = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Página donde se puede leer el contenido de un libro
    /// </summary>
    public Lectura(string bookId, string type, Stream pdfStream)
	{
		InitializeComponent();

        this.bookId = bookId;
        this.type = type;
        this.pdfStream = pdfStream;
        messageManager = new MessageManager(mainPage);
        PageImages = new ObservableCollection<ImageSource>();
        BindingContext = this;

        if (type != "GutembergBooks")
        {
            epubLayout.MinimumHeightRequest = 0;
            epubLayout.HeightRequest = 0;
            if (type == "propio")
            {
                LoadUserPdfImages();
            } else {
                LoadPdfImages();
            }
        }
        else
        {
            LoadEpub();
        } 
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SemanticScreenReader.Default.Announce("Bienvenido a la página de lectura del libro, espero que lo disfrute");
    }

    /// <summary>
    /// Carga el contenido de un epub y lo muestra por pantalla
    /// </summary>
    private async void LoadEpub()
    {
        try
        {
            var snapshot = await firebaseDatabaseManager.GetItemByIdAsync<ModelPdf>(type, bookId);
            var downloadUrl = snapshot.url.ToString() ?? string.Empty;
            if (downloadUrl == null)
            {
                await messageManager.ShowMessage(LocalizationResourceManager.Instance["txtNoBook"].ToString() ?? "El libro no existe en la base de datos."); 
                return;
            }
            using (var httpClient = new HttpClient())
            {
                byte[] epubBytes = await httpClient.GetByteArrayAsync(downloadUrl);
                if (epubBytes == null || epubBytes.Length == 0)
                {
                    throw new Exception("No se pudieron descargar los datos del PDF");
                }
                string fileName = $"epub_{bookId}.epub";
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                File.WriteAllBytes(localFilePath, epubBytes);
                
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    book = await EpubReader.ReadBookAsync(localFilePath);

                    byte[] coverImage = book.CoverImage;

                    if (coverImage != null)
                    {
                        string rutaImagen = Path.Combine(FileSystem.CacheDirectory, $"{bookId}-cover.png");
                        await File.WriteAllBytesAsync(rutaImagen, coverImage);
                    }


                    string imageFilePath = Path.Combine(FileSystem.CacheDirectory, $"{bookId}-cover.png");
                    string base64Image = string.Empty;
                    if (File.Exists(imageFilePath))
                    {
                        byte[] imageBytes = File.ReadAllBytes(imageFilePath);
                        base64Image = Convert.ToBase64String(imageBytes);
                    }

                    StringBuilder htmlContent = new StringBuilder();
                    htmlContent.Append("<html><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=no\"><style>");
                    foreach (var cssFile in book.Content.Css.Local)
                    {
                        htmlContent.Append(cssFile.Content);
                    }
                    htmlContent.Append("</style></head><body>");

                    if (!string.IsNullOrEmpty(base64Image))
                    {
                        htmlContent.Append($"<img src='data:image/png;base64,{base64Image}' alt='Cover Image' style='width:100%; height:auto;' />");
                    }

                    bool isFirstItem = true;
                    string updatedHtmlContent = "";
                    foreach (var textContentFile in book.ReadingOrder)
                    {
                        if (isFirstItem)
                        {
                            isFirstItem = false;
                            continue;
                        }
                        updatedHtmlContent += textContentFile.Content;
                    }
                    updatedHtmlContent = Regex.Replace(updatedHtmlContent,
                                         @"<img\b[^>]*>",
                                         string.Empty,
                                         RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    updatedHtmlContent = Regex.Replace(updatedHtmlContent,
                                                      @"<a[^>]*>(.*?)<\/a>",
                                                      "$1",
                                                      RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    // Si el contenido es demasiado grande, dividirlo
                    const int maxLength = 1_000_000; // Máximo 1 millón de caracteres por parte
                    contentParts = SplitContent(updatedHtmlContent, maxLength);

                    // Muestra la primera parte
                    htmlContent.Append(contentParts[0]);

                    htmlContent.Append("</body></html>");
                    epubViewer.Source = new HtmlWebViewSource
                    {
                        Html = htmlContent.ToString()
                    };
                });
            }
        }
        catch (Exception ex)
        {
            await messageManager.ShowMessage((LocalizationResourceManager.Instance["msnErrorBook"].ToString() ?? "Error al cargar el libro: ") + ex.Message); 
        }

        await Task.Delay(4000);
        // Cargar las partes restantes de forma perezosa
        if (contentParts.Count > 1)
        {
            await EnableLazyLoading(contentParts.Skip(1).ToList());
        }
    }

    /// <summary>
    /// Divide el contenido en partes más pequeñas de un tamaño máximo especificado.
    /// <list type="number">
    /// <item><param name="content">contenido a dividir</param></item>
    /// <item><param name="maxLength">maximo tamaño de cada parte</param></item>
    /// </list>
    /// </summary>
    private List<string> SplitContent(string content, int maxLength)
    {
        var parts = new List<string>();
        for (int i = 0; i < content.Length; i += maxLength)
        {
            parts.Add(content.Substring(i, Math.Min(maxLength, content.Length - i)));
        }
        return parts;
    }

    /// <summary>
    /// Habilita la carga perezosa de contenido en el WebView.
    /// <list type="number">
    /// <item><param name="contentParts">partes del contenido</param></item>
    /// </list>
    /// </summary>
    private async Task EnableLazyLoading(List<string> contentParts)
    {

        for (int i = 0; i < contentParts.Count; i++)
        {
            string script = $@"
            var lazyDiv = document.createElement('div');
            lazyDiv.innerHTML = `{contentParts[i]}`;
            document.body.appendChild(lazyDiv);
        ";
            epubViewer.Eval(script);
        }
    }


    /// <summary>
    /// Carga el contenido de un pdf y lo muestra por pantalla a través de imagenes
    /// </summary>
    public async Task LoadPdfImages()
    {
        try
        {
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

                    // Obtener el número de páginas del PDF
                    int pageCount = converter.PageCount;

                    // Iterar a través de todas las páginas y generar imágenes
                    for (int i = 0; i < pageCount; i++)
                    {
                        string tempImagePath = Path.Combine(FileSystem.CacheDirectory, $"page_{bookId}_{i}.png");

                        using (Stream imageStream = converter.Convert(i, false, false))
                        {
                            // Asegurarnos de que estamos al inicio del stream
                            imageStream.Position = 0;

                            // Guardar la imagen en el directorio de caché
                            using (FileStream fileStream = File.Create(tempImagePath))
                            {
                                await imageStream.CopyToAsync(fileStream);
                            }

                            // Verificar que el archivo existe y tiene tamaño
                            if (File.Exists(tempImagePath))
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    // Cargar la imagen desde el archivo
                                    PageImages.Add(ImageSource.FromFile(tempImagePath));
                                    OnPropertyChanged(nameof(PageImages));
                                });
                            }
                            else
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    Console.WriteLine($"No se pudo crear el archivo de imagen para la página {i + 1}");
                                });
                            }
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
    /// Carga el contenido de un pdf del usuario y lo muestra por pantalla a través de imagenes
    /// </summary>
    public async Task LoadUserPdfImages()
    {
        try
        {
            using (pdfStream)
            {
                // Configurar el convertidor con opciones específicas
                PdfToImageConverter converter = new PdfToImageConverter();
                converter.Load(pdfStream);

                // Obtener el número de páginas del PDF
                int pageCount = converter.PageCount;

                // Iterar a través de todas las páginas y generar imágenes
                for (int i = 0; i < pageCount; i++)
                {
                    string tempImagePath = Path.Combine(FileSystem.CacheDirectory, $"page_{type}_{i}.png");

                    using (Stream imageStream = converter.Convert(i, false, false))
                    {
                        // Asegurarnos de que estamos al inicio del stream
                        imageStream.Position = 0;

                        // Guardar la imagen en el directorio de caché
                        using (FileStream fileStream = File.Create(tempImagePath))
                        {
                            await imageStream.CopyToAsync(fileStream);
                        }

                        // Verificar que el archivo existe y tiene tamaño
                        if (File.Exists(tempImagePath))
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                // Cargar la imagen desde el archivo
                                PageImages.Add(ImageSource.FromFile(tempImagePath));
                                OnPropertyChanged(nameof(PageImages));
                            });
                        }
                        else
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                Console.WriteLine($"No se pudo crear el archivo de imagen para la página {i + 1}");
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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        CleanupCachedImages();
    }

    /// <summary>
    /// Limpia de la cache los archivos descargados 
    /// </summary>
    private void CleanupCachedImages()
    {
        foreach (var image in PageImages)
        {
            if (image is FileImageSource fileImageSource)
            {
                string filePath = fileImageSource.File;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
        PageImages.Clear();
    }

    /// <summary>
    /// Evento de hacer zoom del contenido
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void ZoomIn_Clicked(object sender, EventArgs e)
    {
        if (type != "GutembergBooks")
        {
            ImageHeight *= 1.2; // Increase size by 20%
        }
        else
        {
            scaleFactor *= 1.2; // Increase size by 20%
            ApplyZoom();
        }
    }

    /// <summary>
    /// Evento de deshacer zoom del contenido
    /// <list type="number">
    /// <item><param name="sender">Origen del evento</param></item>
    /// <item><param name="e">Argumentos de selección</param></item>
    /// </list>
    /// </summary>
    private void ZoomOut_Clicked(object sender, EventArgs e)
    {
        if (type != "GutembergBooks")
        {
            ImageHeight /= 1.2; // Decrease size by 20%
        }
        else
        {
            scaleFactor /= 1.2; // Decrease size by 20%
            ApplyZoom();
        }
    }

    /// <summary>
    /// Aplica las opciones de zoom al webview de epub
    /// </summary>
    private void ApplyZoom()
    {
        string jsCode = $"document.body.style.zoom = '{scaleFactor * 100}%'";
        epubViewer.Eval(jsCode);
    }
}