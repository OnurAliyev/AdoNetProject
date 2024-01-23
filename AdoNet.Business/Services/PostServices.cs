using AdoNet.Business.Exceptions;
using AdoNet.Core.Entities;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace AdoNet.Business.Services;

public class PostServices
{
    public async Task<Post> GetByIdAsync(int id)
    {
        if (id < 0) throw new WrongIdFormatException("Wrong ID format !!");
        HttpClient client = new();
        string postUrl = $"https://jsonplaceholder.typicode.com/posts/{id}";
        HttpResponseMessage httpResponse = await client.GetAsync(postUrl);
        if (httpResponse.IsSuccessStatusCode)
        {
            var data = await httpResponse.Content.ReadAsStringAsync();
            var post = JsonConvert.DeserializeObject<Post>(data);
            return post;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Operation failed : {httpResponse.StatusCode}");
            Console.ResetColor();
            return null;
        }
    }
    public async Task AddPostToDbAsync(Post post)
    {
        string connString = @"Server=DESKTOP-E26J09P\SQLEXPRESS;Database=AdoNetDB;Trusted_Connection=true";
        using (SqlConnection conn = new(connString))
        {
            conn.Open();
            if (PostExist(conn, post.Id))
            {
                throw new AlreadyExistException("This post is already exist!");
            }
            string query = $"INSERT INTO Posts VALUES (@UserId,@id,@title,@body)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", post.UserId);
                cmd.Parameters.AddWithValue("@id", post.Id);
                cmd.Parameters.AddWithValue("@title", post.Title);
                cmd.Parameters.AddWithValue("@body", post.Body);
                int affectedRow = await cmd.ExecuteNonQueryAsync();
                if (affectedRow > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Post successfully added.");
                    Console.ResetColor();
                }
            }
        }
    }
    public bool PostExist(SqlConnection conn, int id)
    {
        string query = $"SELECT COUNT(*) FROM Posts Where id={id}";
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
    public async Task NotExistPostsDbAsync()
    {
        JsonBase json = new();
        List<Post> posts = await json.GetPostsApi();
        string connString = @"Server=DESKTOP-E26J09P\SQLEXPRESS;Database=AdoNetDB;Trusted_Connection=true";
        using (SqlConnection conn = new(connString))
        {
            conn.Open();
            string query = $"SELECT id FROM Posts";
            using (SqlCommand cmd = new(query, conn))
            {
                List<int> postsIdInDb = new();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        postsIdInDb.Add(Convert.ToInt32(reader["id"]));
                    }
                }
                var notExistPostsDb = posts.Where(apiPost => !postsIdInDb.Contains(apiPost.Id)).ToList();
                foreach (var post in notExistPostsDb)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"User Id: {post.UserId}\n" +
                                      $"Post Id: {post.Id}\n" +
                                      $"Post Title: {post.Title}\n" +
                                      $"Post Body: {post.Body}");
                    Console.ResetColor();
                }
            }
        }









    }
    public async Task<int> GetUserPostCountsAsync(int userId)
    {
        if (userId < 1 || userId > 10) throw new UserNotFoundException("User with this Id is not found!");
        string connString = @"Server=DESKTOP-E26J09P\SQLEXPRESS;Database=AdoNetDB;Trusted_Connection=true";
        int result = -1;
        using (SqlConnection conn = new(connString))
        {
            conn.Open();
            string query = $"SELECT COUNT(id) FROM Posts Where userId=@UserId";
            using (SqlCommand cmd = new(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                result = (int)cmd.ExecuteScalar();
            }
        }
        return result;
    }
}
