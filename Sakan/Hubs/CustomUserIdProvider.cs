using Microsoft.AspNetCore.SignalR;

namespace Sakan.Hubs
{
    public class CustomUserIdProvider:IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();
            return httpContext?.Request?.Query["userId"];
        }
    }
}
