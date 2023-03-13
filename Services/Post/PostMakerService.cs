
using Basetypes;
using Grpc.Core;
using Server.Database;

namespace Server.Services.Post
{
    public class PostMakerService : postMaker.postMakerBase
    {
        private readonly ILogger<PostMakerService> _logger;

        public PostMakerService(ILogger<PostMakerService> logger)
        {
            _logger = logger;
        }

        public override Task<BaseResponse> makePost(makePostRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Make new Post Request");


            string state = "OK";
            int code = 200;

            int result = Posts.AddNewPost(request, _logger).Result;


            if (result == 1)
            {
                state = "Erorr while adding new Post in DataBase";
                code = 401;
            }

            return Task.FromResult(new BaseResponse
            {
                State = state,
                Code = code,
            });
        }
    }
}
