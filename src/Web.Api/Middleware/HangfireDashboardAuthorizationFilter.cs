using Hangfire.Dashboard;

namespace Web.Api.Middleware;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow all authenticated users to see the dashboard.
        // You can customize this to check for specific roles or claims.
        // e.g., return httpContext.User.IsInRole("Admin");
        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}