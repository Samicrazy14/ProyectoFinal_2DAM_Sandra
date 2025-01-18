namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Clase para manejar los datos de las categorías
    /// </summary>
    public class ModelCategory
    {
        //Atributos
        public string Id { get; set; } = "";
        public string Category { get; set; } = "";
        public long Timestamp { get; set; } = 0;
        public string Uid { get; set; } = "";

        //Constructores
        public ModelCategory() { }

        /// <summary>
        /// Constructor de Categoría con datos
        /// <list type="number">
        /// <item><param name="id">Identificador de la categoría</param></item>
        /// <item><param name="category">Nombre de la categoría</param></item>
        /// <item><param name="timestamp">Fecha de creación de la categoría</param></item>
        /// <item><param name="uid">Identificador del usuario que creó la categoría</param></item>
        /// </list>
        /// </summary>
        public ModelCategory(string id, string category, long timestamp, string uid)
        {
            Id = id;
            Category = category;
            Timestamp = timestamp;
            Uid = uid;
        }
    }
}
