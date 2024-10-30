using SharedKernel.Realtime.Model;

namespace SharedKernel.Realtime.Client;

public interface IRealtimeClient : IDisposable, IAsyncDisposable
{
    string? ConnectionId { get; }
    public event EventHandler<string?>? OnConnectionChanged;
    event VideoFrameCapturedEventHandler? OnVideoFrameCapturedAsync;
    Task ConnectAsync(Func<RealtimeMetaData, Task<VideoFrameCaptured>> invokeVideoFrameCapturedAsync);
}