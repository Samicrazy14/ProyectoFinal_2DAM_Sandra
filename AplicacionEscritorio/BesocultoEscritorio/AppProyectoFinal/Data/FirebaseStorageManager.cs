using Firebase.Storage;
using System.IO;

namespace AppProyectoFinal.Data;
public class FirebaseStorageManager
{
    /// <summary>
    /// Clase para manejar las conexiones con Firebase Storage
    /// </summary>
    public FirebaseStorageManager()
    {
    }

    /// <summary>
    /// Método para obtener la URL de descarga de un archivo
    /// <list type="number">
    /// <item><param name="collection">Collecion a la que pertenece</param></item>
    /// <item><param name="id">Identificador del libro o usuario</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve un string con la url</returns>
    public async Task<string> GetItemDownloadUrlAsync(string collection, string id)
    {
        return await App.Storage
            .Child(collection)
            .Child(id)
            .GetDownloadUrlAsync();
    }

    /// <summary>
    /// Método para obtener los metadatos de un archivo en "Books"
    /// <list type="number">
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve un objeto de la clase FirebaseMetaData con los metadatos</returns>
    public async Task<FirebaseMetaData> GetBookMetaDataAsync(string bookId)
    {
        return await App.Storage
            .Child("Books")
            .Child(bookId)
            .GetMetaDataAsync();
    }

    /// <summary>
    /// Método para subir archivos a Storage
    /// <list type="number">
    /// <item><param name="collection">coleccion a la que se va a subir</param></item>
    /// <item><param name="uid">Identificador del archivo</param></item>
    /// <item><param name="stream">Contiene la imagen</param></item>
    /// </list>
    /// </summary>
    public async Task PushItemAsync(string collection, string uid, Stream stream)
    {
        await App.Storage
            .Child(collection)
            .Child(uid)
            .PutAsync(stream);
    }

    /// <summary>
    /// Método para eliminar archivos de Storage
    /// <list type="number">
    /// <item><param name="collection">coleccion a la que se va a subir</param></item>
    /// <item><param name="uid">Identificador del archivo</param></item>
    /// </list>
    /// </summary>
    public async Task DeleteItemAsync(string collection, string uid)
    {
        await App.Storage
            .Child(collection)
            .Child(uid)
            .DeleteAsync();
    }
}
