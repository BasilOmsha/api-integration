using Microsoft.AspNetCore.SignalR;

namespace api_integration.Presenter.API.src.Hubs
{
    public class DashboardHub : Hub
    {
        private static int _viewerCount;
        private static readonly Lock _lock = new();

        //connection throttling
        private static readonly Dictionary<string, DateTime> _lastConnection = new();
        private static readonly TimeSpan _minConnectionInterval = TimeSpan.FromSeconds(2);

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var remoteIp = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            lock (_lock)
            {
                // reject rapid reconnections from same IP
                if (_lastConnection.TryGetValue(remoteIp, out var last) 
                    && DateTime.UtcNow - last < _minConnectionInterval)
                {
                    Context.Abort();
                    return Task.CompletedTask;
                }

                _lastConnection[remoteIp] = DateTime.UtcNow;
                _viewerCount++;
            }

            Clients.All.SendAsync("ViewerCount", _viewerCount);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            int count;
            lock (_lock)
            {
                _viewerCount = Math.Max(0, _viewerCount - 1);
                count = _viewerCount;
            }

            Clients.All.SendAsync("ViewerCount", count);
            return base.OnDisconnectedAsync(exception);
        }
    }
}