using System.Globalization;

namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Clase para manejar los datos de los comentarios
    /// </summary>
    public class ModelComment
    {
        public LocalizationResourceManager LocalizationResourceManager
            => LocalizationResourceManager.Instance;

        //Atributos
        public string id { get; set; } = "";
        public string bookId { get; set; } = "";
        public string timestamp { get; set; } = "";
        public string comment { get; set; } = "";
        public string uid { get; set; } = "";

        //Atributos calculados
        public string FormattedDate => FormatTimestamp(timestamp);
        public string Name { get; set; } = LocalizationResourceManager.Instance["txtNoUser"].ToString() ?? "";

        //Carga de imagen
        public ImageSource CoverImage { get; set; }

        //Constructores
        public ModelComment() { }

        /// <summary>
        /// Constructor de Comentario con datos
        /// <list type="number">
        /// <item><param name="id">Identificador del comentario</param></item>
        /// <item><param name="bookId">Identificador del libro del comentario</param></item>
        /// <item><param name="timestamp">Fecha de creación del comentario</param></item>
        /// <item><param name="comment">Contenido del comentario</param></item>
        /// <item><param name="uid">Identificador del usuario que hizo el comentario</param></item>
        /// </list>
        /// </summary>
        public ModelComment(string id, string bookId, string timestamp, string comment, string uid)
        {
            this.id = id;
            this.bookId = bookId;
            this.timestamp = timestamp;
            this.comment = comment;
            this.uid = uid;
        }

        /// <summary>
        /// Método para formatear la fecha de creación
        /// </summary>
        /// <list type="number">
        /// <item><param name="timestampString">Fecha de creación del comentario</param></item>
        /// </list>
        /// <returns>Devuelve la fecha formateada</returns>
        public static string FormatTimestamp(string timestampString)
        {
            if (long.TryParse(timestampString, out long timestamp))
            {
                var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                return dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                return LocalizationResourceManager.Instance["txtNoFecha"].ToString() ?? "";
            }
        }
    }
}
