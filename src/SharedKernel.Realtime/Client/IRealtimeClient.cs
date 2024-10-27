using SharedKernel.Realtime.Model;
using SharedKernel.Realtime.Server;

namespace SharedKernel.Realtime.Client;

public interface IRealtimeClient : IRealtimeServer, IDisposable, IAsyncDisposable
{
    Task ConnectAsync();
    event VideoFrameCapturedEventHandler? OnVideoFrameCapturedAsync;
}