
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
                _logger.LogInformation("request-forbidden: session-id: {0}, {1}:{2}",context.Session.Id, context.Request.Method, context.Request.Path);
                return;
            }

            // Fall back to the default implementation.
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
