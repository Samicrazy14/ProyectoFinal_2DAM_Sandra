namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Clase para manejar los datos de los favoritos
    /// </summary>
    public class ModelFav
    {
        //Atributos
        public string bookId { get; set; } = "";
        public long timestamp { get; set; } = 0;
        public string type { get; set; } = "";

        //Constructores
        public ModelFav() { }

        /// <summary>
        /// Constructor de Categoría con datos
        /// <list type="number">
        /// <item><param name="bookId">Identificador del favorito</param></item>
        /// <item><param name="timestamp">Fecha de creación delfavorito</param></item>
        /// <item><param name="type">Tipo del libro favorito</param></item>
        /// </list>
        /// </summary>
        public ModelFav(string bookId, long timestamp, string type)
        {
            this.bookId = bookId;
            this.timestamp = timestamp;
            this.type = type;
        }
    }
}
