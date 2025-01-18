using System.Globalization;
using System.Reactive;

namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Clase para manejar los datos de usuarios
    /// </summary>
    public class ModelUser
    {
        //Atributos
        public string Uid { get; set; } = "";
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string ProfileImage { get; set; } = "";
        public long Timestamp { get; set; } = 0;
        public string UserType { get; set; } = "";

        //Atributos calculados
        public string FormattedDate => FormatTimestamp(Timestamp);
        //Carga de imagen
        public ImageSource CoverImage { get; set; }

        //Constructores
        public ModelUser() { }

        /// <summary>
        /// Constructor de Usuario con datos
        /// <list type="number">
        /// <item><param name="uid">Identificador del usuario</param></item>
        /// <item><param name="email">Correo del usuario</param></item>
        /// <item><param name="name">Nombre del usuario</param></item>
        /// <item><param name="profileImage">Url de firestore con la imagen del usuario</param></item>
        /// <item><param name="timestamp">Fecha de creación del usuario</param></item>
        /// <item><param name="userType">Rol del usuario</param></item>
        /// </list>
        /// </summary>
        public ModelUser(string uid, string email, string name, string profileImage, long timestamp, string userType)
        {
            Uid = uid;
            Email = email;
            Name = name;
            ProfileImage = profileImage;
            Timestamp = timestamp;
            UserType = userType;
        }

        /// <summary>
        /// Método para formatear la fecha de creación
        /// </summary>
        /// <list type="number">
        /// <item><param name="timestamp">Fecha de creación del usuario</param></item>
        /// </list>
        /// <returns>Devuelve la fecha formateada</returns>
        public static string FormatTimestamp(long timestamp)
        {
            var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
            return dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
