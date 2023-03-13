using Grpc.Core;
using Microsoft.Extensions.Logging;
using Server.Backend.Secure;
using Server.Database;
using System.Text;

namespace Server.Services.Registration
{
    public class Registration : RegistrationService.RegistrationServiceBase
    {
        private readonly ILogger<Registration> _logger;

        public Registration(ILogger<Registration> logger)
        {
            _logger = logger;
        }

        public override async Task<SingUpResponse> SingUp(SingUpRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Registration request");


            string state = "OK";
            int code = 200;

            string user_token = TokenMaker.GetUserToken(request.Login, request.Password);

            int update_status = await Users.RegNewUser(request, user_token, _logger);


            if (update_status == 1)
            {

                state = "BD ERROR: user or company already exists\n";
                _logger.LogError(state);
                code = 401;
            }

            return await Task.FromResult(new SingUpResponse
            {
                Code = code,
                State = state,
                Token = Google.Protobuf.ByteString.CopyFrom(Encoding.UTF8.GetBytes(user_token))
            }); 
        }
    }
}
