
using Grpc.Core;
using Server.Backend.Secure;
using Server.Database;
using Server.Models;
using System.Text;

namespace Server.Services.Authorization
{
    public class Authorization : AuthorizeService.AuthorizeServiceBase
    {
        private readonly ILogger<Authorization> _logger;

        public Authorization(ILogger<Authorization> logger)
        {
            _logger = logger;
        }

        // По-хорошему авторизация должна быть только по токену 
        // Этот метод будет временным, пока не сделаю хранение токена в локальном хранилище андройда 

        public override Task<UserResponse> SingIn(SingInRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Sing In Request");

            var user_response = Users.GetUserByLoginPassword(request.Login, request.Password, _logger).Result;
            

            if(user_response.Code == 200)
            {
                string user_token = TokenMaker.GetUserToken(request.Login, request.Password);
                user_response.Token = Google.Protobuf.ByteString.CopyFrom(Encoding.UTF8.GetBytes(user_token));
            }


            return Task.FromResult(user_response);
        }

        public override Task<UserResponse> SingInByToken(SingInByTokenRequest request, ServerCallContext context)
        {
            return base.SingInByToken(request, context);
        }
    }
}
