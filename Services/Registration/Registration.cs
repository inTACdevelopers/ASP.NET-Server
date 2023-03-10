using Grpc.Core;
using Microsoft.Extensions.Logging;
using Server.Database;

namespace Server.Services.Registration
{
    public class Registration : RegistrationService.RegistrationServiceBase
    {
        private readonly ILogger<Registration> _logger;

        public Registration(ILogger<Registration> logger)
        {
            _logger = logger;
        }

        public override async  Task<SingUpResponse> SingUp(SingUpRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Registration request");

            int update_status = await Users.RegNewUser(request);

            string state = "OK";
            int code = 200;

            if(update_status == 1)
            {
               
                state = "BD ERROR: user or company already exists";
                _logger.LogError(state);
                code = 401;
            }

            return  await Task.FromResult(new SingUpResponse
            {
                Code = code,
                State = state,

            });
        }
    }
}
