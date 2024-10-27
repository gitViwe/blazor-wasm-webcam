using SharedKernel.Realtime.Model;

namespace SharedKernel.Realtime.Client;

public interface IRealtimeClient : IDisposable, IAsyncDisposable
{
    Task ConnectAsync();
    event VideoFrameCapturedEventHandler? OnVideoFrameCapturedAsync;
}