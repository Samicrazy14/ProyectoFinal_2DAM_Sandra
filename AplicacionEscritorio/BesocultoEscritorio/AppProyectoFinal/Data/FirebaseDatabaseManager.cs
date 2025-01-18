using Firebase.Database;
using Firebase.Database.Query;
using static System.Reflection.Metadata.BlobBuilder;

namespace AppProyectoFinal.Data;
public class FirebaseDatabaseManager
{
    /// <summary>
    /// Clase para manejar las conexiones con Firebase Database
    /// </summary>
    public FirebaseDatabaseManager()
    {
    }

    /// <summary>
    /// Método para subir datos a una colección(GutembergBooks, Users, ...)
    /// <list type="number">
    /// <item><param name="collection">Nombre de la colección</param></item>
    /// <item><param name="itemId">Identificador de los datos</param></item>
    /// <item><param name="itemData">Objeto con los datos a subir</param></item>
    /// </list>
    /// </summary>
    public async Task UpdateItemAsync(string collection, string itemId, Dictionary<string, object> itemData)
    {
        await App.Database
            .Child(collection)
            .Child(itemId)
            .PutAsync(itemData);
    }

    /// <summary>
    /// Método para recoger un elemento de una colección(Books, GutembergBooks, Users, Categories, ...) específica por su ID
    /// <list type="number">
    /// <item><param name="collection">Nombre de la colección </param></item>
    /// <item><param name="itemId">Identificador de los datos</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve objeto del tipo requerido</returns>
    public async Task<T> GetItemByIdAsync<T>(string collection, string itemId)
    {
        return await App.Database
            .Child(collection)
            .Child(itemId)
            .OnceSingleAsync<T>();
    }

    /// <summary>
    /// Método para recoger una lista de la colección
    /// <list type="number">
    /// <item><param name="collection">Nombre de la colección </param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve una lista de objetos de la colección </returns>
    public async Task<List<FirebaseObject<T>>> GetAllCollectionAsync<T>(string collection)
    {
        return (await App.Database
            .Child(collection)
            .OnceAsync<T>())
            .Reverse()
            .ToList();
    }

    /// <summary>
    /// Método para recoger una lista de libros ordenados por "viewsCount"
    /// <list type="number">
    /// <item><param name="limit">Limite de libros, por defecto 10</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve una lista de objetos ModelPdf</returns>
    public async Task<List<FirebaseObject<ModelPdf>>> GetMostViewedBooksAsync(int limit = 10)
    {
        var books = (await App.Database
                            .Child("Books")
                            .OrderBy("viewsCount")
                            .LimitToLast(limit)
                            .OnceAsync<ModelPdf>())
                            .ToList();
        return books.OrderByDescending(b => b.Object.viewsCount).ToList();
    }

    /// <summary>
    /// Método para recoger la información de un libro favorito de un usuario
    /// <list type="number">
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// <item><param name="uid">Identificador del usuario</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve un objeto ModelFav</returns>
    public async Task<ModelFav> GetFavoriteBookAsync(string bookId, string uid)
    {
        return await App.Database
            .Child("Users")
            .Child(uid)
            .Child("Favorites")
            .Child(bookId)
            .OnceSingleAsync<ModelFav>();
    }

    /// <summary>
    /// Método para recoger una lista de libros favoritos de un usuario
    /// <list type="number">
    /// <item><param name="uid">Identificador del usuario</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve una lista de objetos ModelFav</returns>
    public async Task<List<FirebaseObject<ModelFav>>> GetFavoritesBooksAsync(string uid)
    {
        return (await App.Database.Child("Users").Child(uid).Child("Favorites").OnceAsync<ModelFav>()).ToList();
    }

    /// <summary>
    /// Método para actualizar el valor del contador de vistas de un libro
    /// <list type="number">
    /// <item><param name="collection">Identificador de la colección</param></item>
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// <item><param name="viewsCount">Nuevo valor actualizado</param></item>
    /// </list>
    /// </summary>
    public async Task UpdateViewsCountAsync(string collection, string bookId, long viewsCount)
    {
        await App.Database
            .Child(collection)
            .Child(bookId)
            .PatchAsync(new { viewsCount });
    }

    /// <summary>
    /// Método para actualizar el valor del tipo de usuario de un usuario
    /// <list type="number">
    /// <item><param name="collection">Identificador de la colección</param></item>
    /// <item><param name="uid">Identificador del usuario</param></item>
    /// <item><param name="userType">Nuevo valor actualizado</param></item>
    /// </list>
    /// </summary>
    public async Task UpdateUserTypeAsync(string collection, string uid, string userType)
    {
        await App.Database
            .Child(collection)
            .Child(uid)
            .PatchAsync(new { userType });
    }

    /// <summary>
    /// Método para actualizar el valor del nombre del usuario
    /// <list type="number">
    /// <item><param name="collection">Identificador de la colección</param></item>
    /// <item><param name="uid">Identificador del usuario</param></item>
    /// <item><param name="name">Nuevo valor actualizado</param></item>
    /// </list>
    /// </summary>
    public async Task UpdateUserNameAsync(string collection, string uid, string name)
    {
        await App.Database
            .Child(collection)
            .Child(uid)
            .PatchAsync(new { name });
    }

    /// <summary>
    /// Método para actualizar el valor del nombre del usuario
    /// <list type="number">
    /// <item><param name="collection">Identificador de la colección</param></item>
    /// <item><param name="uid">Identificador del usuario</param></item>
    /// <item><param name="profileImage">Nuevo valor actualizado</param></item>
    /// </list>
    /// </summary>
    public async Task UpdateUserImageAsync(string collection, string uid, string profileImage)
    {
        await App.Database
            .Child(collection)
            .Child(uid)
            .PatchAsync(new { profileImage });
    }

    /// <summary>
    /// Método para actualizar los valores editables de un libro
    /// <list type="number">
    /// <item><param name="collection">Identificador de la colección</param></item>
    /// <item><param name="id">Identificador del libro</param></item>
    /// <item><param name="title">Nuevo valor para titulo</param></item>
    /// <item><param name="description">Nuevo valor para descripción</param></item>
    /// <item><param name="categoryId">Nuevo valor para el id de categoría</param></item>
    /// </list>
    /// </summary>
    public async Task UpdateBookDataAsync(string collection, string id, string title, string description, string categoryId)
    {
        await App.Database
                .Child(collection)
                .Child(id)
                .PatchAsync(new
                {
                    title,
                    description,
                    categoryId
                });

    }

    /// <summary>
    /// Método para recoger una lista de comentarios de un libro
    /// <list type="number">
    /// <item><param name="collection">Identificador de la colección</param></item>
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve una lista de objetos ModelComment</returns>
    public async Task<List<FirebaseObject<ModelComment>>> GetCommentsAsync(string collection, string bookId)
    {
        return (await App.Database
            .Child(collection)
            .Child(bookId)
            .Child("Comments")
            .OnceAsync<ModelComment>())
            .ToList();
    }

    /// <summary>
    /// Método para añadir un comentario
    /// <list type="number">
    /// <item><param name="collection">Identificador de la colección</param></item>
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// <item><param name="timestamp">Id del comentario/fecha de creación</param></item>
    /// <item><param name="commentData">Datos del comentario</param></item>
    /// </list>
    /// </summary>
    public async Task AddCommentAsync(string collection, string bookId, string timestamp, Dictionary<string, object> commentData)
    {
        await App.Database
            .Child(collection)
            .Child(bookId)
            .Child("Comments")
            .Child(timestamp)
            .PutAsync(commentData);
    }

    /// <summary>
    /// Método para añadir un favorito
    /// <list type="number">
    /// <item><param name="bookId">Identificador del libro</param></item>
    /// <item><param name="favData">Datos del favorito</param></item>
    /// <item><param name="uid">Identificador del usuario</param></item>
    /// </list>
    /// </summary>
    public async Task AddFavoriteAsync(string bookId, ModelFav favData, string uid)
    {
        await App.Database
            .Child("Users")
            .Child(uid)
            .Child("Favorites")
            .Child(bookId)
            .PutAsync(favData);
    }

    /// <summary>
    /// Método para eliminar un favorito o un comentario
    /// <list type="number">
    /// <item><param name="collection">Identificador del tipo de libro o usuario</param></item>
    /// <item><param name="itemId">Identificador del libro o usuario</param></item>
    /// <item><param name="subCollection">Identificador de si es comentario o favorito</param></item>
    /// <item><param name="subItemId">Identificador del comentario o libro fav</param></item>
    /// </list>
    /// </summary>
    public async Task DeleteUserActionAsync(string collection, string itemId, string subCollection, string subItemId)
    {
        await App.Database
            .Child(collection)
            .Child(itemId)
            .Child(subCollection)
            .Child(subItemId)
            .DeleteAsync();
    }

    /// <summary>
    /// Método para eliminar un item de la BBDD
    /// <list type="number">
    /// <item><param name="collection">Identificador de la coleccion</param></item>
    /// <item><param name="itemId">Identificador del item</param></item>
    /// </list>
    /// </summary>
    public async Task DeleteItemAsync(string collection, string itemId)
    {
        await App.Database
            .Child(collection)
            .Child(itemId)
            .DeleteAsync();
    }

    /// <summary>
    /// Método para recoger la configuracion de un usuario
    /// <list type="number">
    /// <item><param name="uid">Identificador del usuario</param></item>
    /// </list>
    /// </summary>
    /// <returns>Devuelve una lista de objetos ModelFav</returns>
    public async Task<ModelConfig> GetConfigUserAsync(string uid)
    {
        return await App.Database.Child("Users")
                                    .Child(uid)
                                    .Child("Config")
                                    .OnceSingleAsync<ModelConfig>();
    }

    /// <summary>
    /// Método para actualizar la configuracion de un usuario
    /// <list type="number">
    /// <item><param name="uid">Identificador del usuario</param></item>
    /// <item><param name="configData">Parametros de la configuracion</param></item>
    /// </list>
    /// </summary>
    public async Task UpdateConfigUserAsync(string uid, Dictionary<string, object> configData)
    {
        await App.Database.Child("Users").Child(uid).Child("Config").PutAsync(configData);
    }
}
