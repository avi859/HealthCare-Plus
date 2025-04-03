using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace AngularAuthApi.Services
{
    public class NewsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "cba2d9b66b7f95078416f6bba25eddd5";

        public NewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NewsArticle>> GetHealthNewsAsync()
        {
            string url = $"http://api.mediastack.com/v1/news?access_key={_apiKey}&categories=health&languages=en";
            var response = await _httpClient.GetStringAsync(url);

            var newsResponse = JsonSerializer.Deserialize<NewsApiResponse>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return newsResponse?.Data ?? new List<NewsArticle>();
        }
    }

    // Define response model
    public class NewsApiResponse
    {
        public List<NewsArticle>? Data { get; set; }
    }

    public class NewsArticle
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Image { get; set; }
        public string? Published_at { get; set; }
    }
}
