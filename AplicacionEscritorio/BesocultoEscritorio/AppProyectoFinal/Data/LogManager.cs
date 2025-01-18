using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace AppProyectoFinal.Data;
public class LogManager
{
    // Clave y vector de inicialización para la encriptación
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes para AES-128
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("6543210987654321");  // 16 bytes para AES-128

    /// <summary>
    /// Clase para manejar los datos de log del Usuario
    /// </summary>
    public LogManager()
    {
    }

    /// <summary>
    /// Método para almacenar en la app el usuario y entrar a la cuenta directamente la proxima vez
    /// <list type="number">
    /// <item><param name="email">Email del usuario</param></item>
    /// <item><param name="password">Password del usuario</param></item>
    /// </list>
    /// </summary>
    public async Task SaveUserCredentialsAsync(string email, string password)
    {
        string fileName = "user.txt";
        string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        string content = $"Email: {email}\nPassword: {password}";

        // Encriptar contenido
        string encryptedContent = Encrypt(content);

        // Guardar contenido encriptado en el archivo
        using (StreamWriter writer = new StreamWriter(localFilePath, false))
        {
            await writer.WriteAsync(encryptedContent);
        }
    }

    /// <summary>
    /// Método para leer los credenciales guardados del usuario si existen
    /// </summary>
    /// <returns>Devuelve la email y contraseña para logearse</returns>
    public async Task<(string Email, string Password)?> ReadUserCredentialsAsync()
    {
        string fileName = "user.txt";
        string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        if (File.Exists(localFilePath))
        {
            using (StreamReader reader = new StreamReader(localFilePath))
            {
                string encryptedContent = await reader.ReadToEndAsync();
                // Verifica que el contenido no esté vacío o corrupto
                if (!string.IsNullOrWhiteSpace(encryptedContent))
                {
                    try
                    {
                        string decryptedContent = Decrypt(encryptedContent);
                        Console.WriteLine("Contenido desencriptado: " + decryptedContent);

                        string[] lines = decryptedContent.Split('\n');
                        string email = lines[0].Replace("Email: ", "").Trim();
                        string password = lines[1].Replace("Password: ", "").Trim();

                        return (email, password);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al desencriptar: {ex.Message}");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("El archivo está vacío o tiene contenido encriptado incorrecto.");
                }
            }
        }

        return null; // Si el archivo no existe, devuelve null
    }

    /// <summary>
    /// Método para eliminar el archivo de credenciales del usuario
    /// </summary>
    public void DeleteUserCredentials()
    {
        string fileName = "user.txt";
        string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        //Pone la configuración por defecto
        FontSizeManager.Instance.UpdateFontSizes(1);
        ThemeManager.Instance.UpdateThemeColors(0);
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("es"));

        if (File.Exists(localFilePath))
        {
            File.Delete(localFilePath);
            Console.WriteLine("Archivo de credenciales eliminado.");
        }
        else
        {
            Console.WriteLine("El archivo de credenciales no existe.");
        }
    }

    /// <summary>
    /// Método para encriptar los credenciales
    /// <list type="number">
    /// <item><param name="plainText">Texto a encriptar</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve los credenciales encriptados</returns>
    private string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
                sw.Close();
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    /// <summary>
    /// Método para desencriptar los credenciales
    /// <list type="number">
    /// <item><param name="cipherText">Texto a desencriptar</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve los credenciales desencriptados</returns>
    private string Decrypt(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}