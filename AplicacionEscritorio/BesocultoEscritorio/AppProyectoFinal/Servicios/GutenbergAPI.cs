using AppProyectoFinal.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace AppProyectoFinal.Servicios
{
    public class GutenbergAPI
    {
        private readonly HttpClient httpClient;

        public GutenbergAPI(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task LoadGutembergBooks(int currentPage, ObservableCollection<ModelPdf> gutenbergBooks, ObservableCollection<ModelPdf> rawBooks, Func<string, Task<(string description, string publishedDate, int pageCount)>> getGoogleBookInfo)
        {
            try
            {
                var threads = new List<Thread>();

                for (int i = 1; i <= 20; i++)
                {
                    int bookId = (currentPage - 1) * 20 + i;
                    var thread = new Thread(async () =>
                    {
                        try
                        {
                            var response = await httpClient.GetStringAsync($"https://gutendex.com/books/?ids={bookId}");
                            var jsonObject = JsonNode.Parse(response);
                            var resultsArray = jsonObject["results"]?.AsArray();

                            if (resultsArray != null && resultsArray.Count > 0)
                            {
                                var bookObject = resultsArray[0];
                                var formats = bookObject["formats"];
                                string imageUrl = formats?["image/jpeg"]?.ToString() ?? "";
                                string epubUrl = formats?["application/epub+zip"]?.ToString() ?? "";

                                var authors = bookObject["authors"]?.AsArray();
                                string author = authors != null && authors.Count > 0 ? authors[0]["name"]?.ToString() : "Unknown";

                                var bookshelves = bookObject["bookshelves"]?.AsArray();
                                var categories = new List<string>();

                                // Iterar por cada elemento en el JsonArray
                                foreach (var item in bookshelves)
                                {
                                    var category = item.ToString();
                                    if (category.StartsWith("Browsing: "))
                                    {
                                        category = category.Substring("Browsing: ".Length);
                                    }
                                    categories.Add(category);
                                    break; // Rompe el ciclo después de agregar la primera categoría
                                }

                                // Unir las categorías en un solo string
                                string categoryId = string.Join("", categories);


                                string id = bookObject["id"].ToString();
                                string title = bookObject["title"].ToString();
                                long viewsCount = bookObject["download_count"]?.GetValue<long>() ?? 0;

                                // Llama a la función de Google API para obtener información adicional del libro
                                var (description, publishedDate, pageCount) = await getGoogleBookInfo(title);

                                // Crea la instancia de ModelPdf con los datos obtenidos
                                var book = new ModelPdf
                                {
                                    author = author,
                                    categoryId = categoryId,
                                    description = description,
                                    id = id,
                                    imagenUrl = imageUrl,
                                    pagecount = pageCount,
                                    timestamp = YearToMillis(publishedDate),
                                    title = title,
                                    uid = "Project Gutenberg",
                                    url = epubUrl,
                                    viewsCount = viewsCount
                                };

                                // Agrega el libro a la colección de manera segura para hilos
                                lock (gutenbergBooks)
                                {
                                    gutenbergBooks.Add(book);
                                    rawBooks.Add(book);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error loading book {bookId}: {ex.Message}");
                        }
                    });
                    // Inicia el hilo y lo agrega a la lista
                    thread.Start();
                    threads.Add(thread);
                }

                // Espera a que todos los hilos finalicen
                foreach (var thread in threads)
                {
                    thread.Join();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading books: {ex.Message}");
            }
        }

        private long YearToMillis(string year)
        {
            // Configurar la cultura en inglés para consistencia
            var culture = new CultureInfo("en-US");

            try
            {
                // Caso 1: Si el input es solo un año (ej: "1920")
                if (year.Length == 4 && int.TryParse(year, out int yearNum))
                {
                    return new DateTimeOffset(yearNum, 1, 1, 0, 0, 0, TimeSpan.Zero)
                        .ToUnixTimeMilliseconds();
                }
                // Caso 2: Si el input es año y mes (ej: "2010-08")
                else if (year.Length == 7 && Regex.IsMatch(year, @"^\d{4}-\d{2}$"))
                {
                    if (DateTime.TryParseExact(year, "yyyy-MM",
                        culture,
                        DateTimeStyles.None,
                        out DateTime date))
                    {
                        return new DateTimeOffset(date).ToUnixTimeMilliseconds();
                    }
                }
                // Caso 3: Si el input es una fecha completa (ej: "2022-05-28")
                else if (year.Length == 10 && Regex.IsMatch(year, @"^\d{4}-\d{2}-\d{2}$"))
                {
                    if (DateTime.TryParseExact(year, "yyyy-MM-dd",
                        culture,
                        DateTimeStyles.None,
                        out DateTime date))
                    {
                        return new DateTimeOffset(date).ToUnixTimeMilliseconds();
                    }
                }

                throw new ArgumentException("Formato no soportado");
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error al procesar la entrada: {ex.Message}");
            }
        }
    }
}