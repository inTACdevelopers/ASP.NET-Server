using Server.Models;
using Server.Services.Post;

namespace Server.Database
{
    public class Posts
    {
        public static Task<int> AddNewPost(makePostRequest request, ILogger<PostMakerService> _logger)
        {
            try
            {
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
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Source + "\n" + ex.InnerException);
                return Task.FromResult(1);
            }

        }
    }
}
