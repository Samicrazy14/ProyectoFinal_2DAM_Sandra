using System.Globalization;

namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Clase para manejar los datos de los libros
    /// </summary>
    public class ModelPdf
    {
        public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

        //Atributos
        public string author { get; set; } = LocalizationResourceManager.Instance["txtNoAutor"].ToString() ?? "";
        public string categoryId { get; set; } = "";
        public string description { get; set; } = LocalizationResourceManager.Instance["txtNoDescripcion"].ToString() ?? "";
        public string id { get; set; } = "";
        public string imagenUrl { get; set; } = "default_book_cover.png";
        public long pagecount { get; set; } = 0;
        public long timestamp { get; set; } = 0;
        public string title { get; set; } = LocalizationResourceManager.Instance["txtNoTitulo"].ToString() ?? "";
        public string uid { get; set; } = "";
        public string url { get; set; } = "";
        public long viewsCount { get; set; } = 0;


        //Atributos calculados
        public string Category { get; set; } = LocalizationResourceManager.Instance["txtNoCategoria"].ToString() ?? "";
        public string FormattedDate => FormatTimestamp(timestamp);

        //Carga de imagen
        public ImageSource CoverImage { get; set; }

        //Constructores
        public ModelPdf() { }

        /// <summary>
        /// Constructor de libros de Firebase con datos
        /// <list type="number">
        /// <item><param name="uid">Identificador del autor del libro</param></item>
        /// <item><param name="id">Identificador del libro</param></item>
        /// <item><param name="title">Titulo del libro</param></item>
        /// <item><param name="description">Descripción del libro</param></item>
        /// <item><param name="categoryId">Identificador de la categoría del libro</param></item>
        /// <item><param name="pageCount">Numero de páginas del libro</param></item>
        /// <item><param name="url">Url del contenido del libro</param></item>
        /// <item><param name="timestamp">Fecha de creación del libro</param></item>
        /// <item><param name="viewsCount">Numero de visualizaciones del libro</param></item>
        /// </list>
        /// </summary>
        public ModelPdf(string uid, string id, string title, string description, string categoryId, long pageCount, string url, long timestamp, long viewsCount)
        {
            this.uid = uid;
            this.id = id;
            this.title = title;
            this.description = description;
            this.categoryId = categoryId;
            this.pagecount = pageCount;
            this.url = url;
            this.timestamp = timestamp;
            this.viewsCount = viewsCount;
        }

        /// <summary>
        /// Constructor de libros de Gutenberg API con datos
        /// <list type="number">
        /// <item><param name="author">Autor del libro</param></item>
        /// <item><param name="categoryId">Identificador de la categoría del libro</param></item>
        /// <item><param name="description">Descripción del libro</param></item>
        /// <item><param name="id">Identificador del libro</param></item>
        /// <item><param name="imagenUrl">Url de la portada del libro</param></item>
        /// <item><param name="pageCount">Numero de páginas del libro</param></item>
        /// <item><param name="timestamp">Fecha de creación del libro</param></item>
        /// <item><param name="title">Titulo del libro</param></item>
        /// <item><param name="uid">Identificador del autor del libro</param></item>
        /// <item><param name="url">Url del contenido del libro</param></item>
        /// <item><param name="viewsCount">Numero de visualizaciones del libro</param></item>
        /// </list>
        /// </summary>
        public ModelPdf(string author, string categoryId, string description, string id, string imagenUrl, long pageCount, long timestamp, string title, string uid, string url, long viewsCount)
        {
            this.author = author;
            this.imagenUrl = imagenUrl;
            this.pagecount = pageCount;
            this.uid = uid;
            this.id = id;
            this.title = title;
            this.description = description;
            this.categoryId = categoryId;
            this.url = url;
            this.timestamp = timestamp;
            this.viewsCount = viewsCount;
        }

        /// <summary>
        /// Método para formatear la fecha de creación
        /// </summary>
        /// <list type="number">
        /// <item><param name="timestamp">Fecha de creación del comentario</param></item>
        /// </list>
        /// <returns>Devuelve la fecha formateada</returns>
        private string FormatTimestamp(long timestamp)
        {
            var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
            return dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
