using Basetypes;
using Grpc.Core;
using Server.Database;

namespace Server.Services.Post
{
    public class PostWorkerService : PostWorker.PostWorkerBase
    {
        private readonly ILogger<PostWorkerService> _logger;

        public PostWorkerService(ILogger<PostWorkerService> logger)
        {
            _logger = logger;
        }

        public override Task<BaseResponse> LikePost(LikePostRequest request, ServerCallContext context)
        {
            string state = "OK";
            int code = 200;


            try
            {
                Posts.LikePost(request.FromUser, request.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Erorr!\n{ex.Message}");

                state = "Erorr\n" + ex.Message;
                code = 401;
            }

            return Task.FromResult(new BaseResponse()
            {
                State = state,
                Code = code,
            });
        }

        public override Task<BaseResponse> UnLikePost(UnLikePostRequest request, ServerCallContext context)
        {
            string state = "OK";
            int code = 200;


            try
            {
                Posts.UnLikePost(request.FromUser, request.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Erorr!\n{ex.Message}");

                state = "Erorr\n" + ex.Message;
                code = 401;
            }


            return Task.FromResult(new BaseResponse()
            {
                State = state,
                Code = code,
            });
        }
    }
}
