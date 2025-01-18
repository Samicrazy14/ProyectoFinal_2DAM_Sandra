using System.Text.Json.Nodes;

namespace AppProyectoFinal.Servicios
{
    public class GoogleBooksAPI
    {
        private readonly HttpClient httpClient;

        public GoogleBooksAPI(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<(string description, string publishedDate, int pageCount)> GetBookInfoAsync(string title)
        {
            string formattedTitle = Uri.EscapeDataString(title);
            string url = $"https://www.googleapis.com/books/v1/volumes?q={formattedTitle}";

            try
            {
                string response = await httpClient.GetStringAsync(url);
                var jsonObject = JsonNode.Parse(response);
                var items = jsonObject["items"]?.AsArray();

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var volumeInfo = item["volumeInfo"];
                        string description = volumeInfo?["description"]?.ToString() ?? "";
                        string publishedDate = volumeInfo?["publishedDate"]?.ToString() ?? "";
                        int pageCount = volumeInfo?["pageCount"]?.GetValue<int>() ?? 0;

                        if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(publishedDate) && pageCount > 0)
                        {
                            return (description, publishedDate, pageCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Google Books data: {ex.Message}");
            }

            return ("", "", 0);
        }
    }
}