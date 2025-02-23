using Microsoft.AspNetCore.Components;

namespace KeycloakExample.Web.Auth;

public class LogoutService(IHttpContextAccessor httpContextAccessor, NavigationManager navigationManager)
{
    // Be careful in a mulit user seting, as these static variables would be shared across scopes
    private static readonly Lock _lock = new();
    private static bool _isLoggingOut; 

    public void Logout()
    {
        lock (_lock)
        {
            if (_isLoggingOut) return;
            _isLoggingOut = true;
        }

        var context = httpContextAccessor.HttpContext;
        if (context != null)
        {
            // Navigate to the logout endpoint
            navigationManager.NavigateTo("/authentication/logout", true);
        }
        _isLoggingOut = false;
    }
}

