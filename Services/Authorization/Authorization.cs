
using Grpc.Core;
using Server.Models;

namespace Server.Services.Authorization
{
    public class Authorization : AuthorizeService.AuthorizeServiceBase
    {
        private readonly ILogger<Authorization> _logger;

        public Authorization(ILogger<Authorization> logger)
        {
            _logger = logger;
        }


        public override Task<UserResponse> SingIn(SingInRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Sing In Request");

            using (IntacNetRuContext db = new IntacNetRuContext())
            {
                

                return Task.FromResult(new UserResponse()
                {
                    State = "OK",
                    Code = 200,

                });
            }
        }

        public override Task<UserResponse> SingInByToken(SingInByTokenRequest request, ServerCallContext context)
        {
            return base.SingInByToken(request, context);
        }
    }
}
