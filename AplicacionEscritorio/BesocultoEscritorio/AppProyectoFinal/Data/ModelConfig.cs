namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Clase para manejar los datos de configuracion del usuario
    /// </summary>
    public class ModelConfig
    {
        //Atributos
        public string fontSize { get; set; } = ""; //Pequeña, Mediana y Grande
        public string language { get; set; } = ""; //es y en
        public string theme { get; set; } = ""; //light y dark

        //Constructores
        public ModelConfig() { }

        /// <summary>
        /// Constructor de Configuracion con datos
        /// <list type="number">
        /// <item><param name="fontSize">Tamaño de fuente</param></item>
        /// <item><param name="language">Lenguaje</param></item>
        /// <item><param name="theme">Tema</param></item>
        /// </list>
        /// </summary>
        public ModelConfig(string fontSize, string language, string theme)
        {
            this.fontSize = fontSize;
            this.language = language;
            this.theme = theme;
        }
    }
}
