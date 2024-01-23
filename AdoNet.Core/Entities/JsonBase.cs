using Newtonsoft.Json;

namespace AdoNet.Core.Entities
{
    public class JsonBase
    {
        readonly HttpClient httpClient=new();
        public async Task<List<Post>> GetPostsApi()
        {
            try
            {
                string apiUrl = @"https://jsonplaceholder.typicode.com/posts";
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(apiUrl);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var data = await httpResponseMessage.Content.ReadAsStringAsync();
                    var posts = JsonConvert.DeserializeObject<List<Post>>(data);
                    return posts;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Operation failed : {httpResponseMessage.StatusCode} due to {httpResponseMessage.ReasonPhrase}");
                    Console.ResetColor();
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Request error : {ex.Message}");
                Console.ResetColor();
                return null;
            }
        }
    }
}