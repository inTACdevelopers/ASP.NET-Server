using Npgsql;
using Server.Backend.Config;
using Server.Backend.Secure;
using Server.Models;
using Server.Services.Post;

namespace Server.Database
{
    public class Posts
    {
        public static Task RecalculatePostWeight()
        {
            return Task.Run(() =>
            {

            });
        }

        public static Task<int> AddNewPost(makePostRequest request, ILogger<PostMakerService> _logger)
        {
            try
            {
                // add new post EF
                using (IntacNetRuContext db = new IntacNetRuContext())
                {
                    var Post = new Post()
                    {
                        Title = request.PostTitle,
                        Description = request.PostDescription,
                        Owner = request.UserId,
                        Url = request.Url,
                        CreationDate = DateTime.UtcNow,

                    };
                    db.Posts.Add(Post);
                    db.Users.Where(user => user.Id == request.UserId).First().Posts.Add(Post);

                    db.SaveChanges();

                    // create post likes table
                    using (var conn = new NpgsqlConnection(new ConfigManager().GetConnetion()))
                    {

                        string get_last_post_id = "SELECT last_value FROM posts_id_seq --";

                        int last_post_id = 1;


                        conn.Open();

                        using (var command = new NpgsqlCommand(get_last_post_id, conn))
                        {
                            using(var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                              
                                    last_post_id = reader.GetInt32(0);
                                }
                            }
                        }

                        string post_token = TokenMaker.GetPostToken(last_post_id, request.PostTitle);

                        string create_post_likes_table = $"CREATE TABLE post_likes_{post_token} (" +
                        "user_id BIGINT PRIMARY KEY)--";


                        using (var command = new NpgsqlCommand(create_post_likes_table, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Source + "\n" + ex.Message);

                return Task.FromResult(1);
            }

        }

        public static Task<string> GetPostTitle(int post_id)
        {
            string title = String.Empty;

            using (var conn = new NpgsqlConnection(new ConfigManager().GetConnetion()))
            {
                string get_title = $"SELECT title FROM posts WHERE id = {post_id}--";
               

                conn.Open();

                using (var command = new NpgsqlCommand(get_title, conn))
                {
                   using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            title = reader.GetString(0);    
                        }
                    }
                }

               
            }

            return Task.FromResult(title);

        }


        public static Task LikePost(int from_user, int post_id)
        {
            return Task.Run(() =>
            {
                using (var conn = new NpgsqlConnection(new ConfigManager().GetConnetion()))
                {
                    string post_token = TokenMaker.GetPostToken(post_id, GetPostTitle(post_id).Result);

                    string append_like = $"INSERT INTO post_likes_{post_token} VALUES({from_user})--";

                    conn.Open();

                    using (var command = new NpgsqlCommand(append_like, conn))
                    {
                        command.ExecuteNonQueryAsync();
                    }
                }
            });
        }

        public static Task UnLikePost(int from_user, int post_id)
        {
            return Task.Run(() =>
            {
                using (var conn = new NpgsqlConnection(new ConfigManager().GetConnetion()))
                {
                    string post_token = TokenMaker.GetPostToken(post_id, GetPostTitle(post_id).Result);

                    string append_like = $"DELETE FROM post_likes_{post_token} WHERE user_id = {from_user}--";

                    conn.Open();

                    using (var command = new NpgsqlCommand(append_like, conn))
                    {
                        command.ExecuteNonQueryAsync();
                    }
                }
            });
        }
    }
}
