using Microsoft.AspNetCore.SignalR;
using SharedKernel.Realtime.Constant;
using SharedKernel.Realtime.Model;

namespace Gateway.SignalRHub;

internal class RealtimeServerHub : Hub;

internal class RealtimeServerHubContext(IHubContext<RealtimeServerHub> hub)
{
    public Task BroadcastVideoFrameCapturedAsync(VideoFrameCaptured notification, CancellationToken cancellationToken)
        => hub.Clients.All.SendAsync(MethodName.VideoFrameCaptured, notification, cancellationToken);
}