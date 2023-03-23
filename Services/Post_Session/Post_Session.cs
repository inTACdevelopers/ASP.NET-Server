using Basetypes;
using Grpc.Core;
using Server.Backend.Exeptions;
using Server.Database;
using Server.Models;
using Server.Protos;
using Server.Services.Post;

namespace Server.Services.Post_Session
{

    public class Post_Session : postSessionsService.postSessionsServiceBase
    {

        private readonly ILogger<Post_Session> _logger;

        public Post_Session(ILogger<Post_Session> logger)
        {
            _logger = logger;
        }


        public override async Task<BaseResponse> CreatePostSession(CreatePostSessionRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Create new post_session Request");

            string state = "OK";
            int code = 200;

            try
            {
                await Users.CreateUserPostSession(request.UserId);
            }
            catch (DataBaseExeption ex)
            {
                _logger.LogError(ex.Message);
                state = ex.Message;
                code = 401;
            }


            return new BaseResponse() {
                State = state,
                Code = code
            };
        }

        public override async Task<BaseResponse> DropPostSession(DropSessionRequest request, ServerCallContext context)
        {

            string state = "OK";
            int code = 200;

            try
            {
                await Users.DropUserPostSession(request.UserId);
            }
            catch (DataBaseExeption ex)
            {
                _logger.LogError(ex.Message);
                state = ex.Message;
                code = 401;
            }


            return new BaseResponse()
            {
                State = state,
                Code = code
            };
        }
    }
}
