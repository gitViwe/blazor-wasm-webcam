using Microsoft.AspNetCore.SignalR;
using SharedKernel.Realtime.Constant;
using SharedKernel.Realtime.Model;

namespace Gateway.SignalRHub;

internal class RealtimeServerHub : Hub
{
    public Task JoinGroup(string groupName)
        => Groups.AddToGroupAsync(Context.ConnectionId, groupName);
};

internal class RealtimeServerHubContext(IHubContext<RealtimeServerHub> hub)
{
    public Task BroadcastVideoFrameCapturedAsync(VideoFrameCaptured notification, CancellationToken cancellationToken)
        => hub.Clients.All.SendAsync(MethodName.VideoFrameCaptured, notification, cancellationToken);
    
    public Task<VideoFrameCaptured> InvokeVideoFrameCaptureAsync(string connectionId, RealtimeMetaData metaData, CancellationToken cancellationToken)
        => hub.Clients.Client(connectionId).InvokeAsync<VideoFrameCaptured>(MethodName.InvokeVideoFrameCapture, metaData, cancellationToken);
}