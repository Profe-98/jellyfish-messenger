
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace JellyFishBackend.Middleware
{
    public class AuthorizationResultMiddleware : IAuthorizationMiddlewareResultHandler
    {
        private readonly ILogger<AuthorizationResultMiddleware> _logger;
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        public AuthorizationResultMiddleware(ILogger<AuthorizationResultMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task HandleAsync(RequestDelegate next,HttpContext context,AuthorizationPolicy policy,PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                string requiredClaims = null;
                authorizeResult.AuthorizationFailure?.FailureReasons?.ToList()?.ForEach(x => requiredClaims += x.Message);
                _logger.LogInformation("request-forbidden: conn-id: {0}, {1}:{2}, {3}",context.Connection.Id, context.Request.Method, context.Request.Path,requiredClaims);
                return;
            }

            // Fall back to the default implementation.
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
