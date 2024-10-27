using SharedKernel.Realtime.Model;

namespace SharedKernel.Realtime.Server;

public interface IRealtimeServer
{
    Task BroadcastVideoFrameCapturedAsync(VideoFrameCaptured notification, CancellationToken cancellationToken);
}