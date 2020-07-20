using Hangfire.Dashboard;

namespace KinoCMS.Server.Infrastructure
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            //return httpContext.User.Identity.IsAuthenticated;
            return true;
        }
    }
}
