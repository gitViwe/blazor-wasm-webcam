using Microsoft.AspNetCore.SignalR;
using SharedKernel.Realtime.Model;
using SharedKernel.Realtime.Server;

namespace Gateway.SignalRHub;

internal class RealtimeServerHub : Hub, IRealtimeServer
{
    public Task BroadcastVideoFrameCapturedAsync(VideoFrameCaptured notification, CancellationToken cancellationToken)
    {
        return Clients.All.SendAsync(nameof(VideoFrameCaptured), notification, cancellationToken);
    }
}