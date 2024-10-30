using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using SharedKernel.Realtime.Constant;
using SharedKernel.Realtime.Model;

namespace SharedKernel.Realtime.Client;

public class RealtimeClient(IConfiguration configuration) : IRealtimeClient
{
    public event EventHandler<string?>? OnConnectionChanged;
    public string? ConnectionId => _hubConnection?.ConnectionId;
    public event VideoFrameCapturedEventHandler? OnVideoFrameCapturedAsync;

    private HubConnection? _hubConnection;
    private readonly CancellationTokenSource _cts = new();
    private Func<RealtimeMetaData, Task<VideoFrameCaptured>>? _invokeVideoFrameCapturedAsync;

    public Task ConnectAsync(Func<RealtimeMetaData, Task<VideoFrameCaptured>> invokeVideoFrameCapturedAsync)
    {
        _invokeVideoFrameCapturedAsync = invokeVideoFrameCapturedAsync;
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(configuration["ApiGatewayBaseUrl"] + "/notification")
            .WithAutomaticReconnect(new IndefiniteRetryPolicy())
            .Build();

        _hubConnection.Closed += async _ =>
        {
            OnConnectionChanged?.Invoke(this, null);
            // Wait a bit and restart the connection again.
            await Task.Delay(5000, _cts.Token);
            await ConnectWithRetryAsync(_cts.Token);
        };
        
        _hubConnection.Reconnected += id =>
        {
            OnConnectionChanged?.Invoke(this, id);
            return Task.CompletedTask;
        };
        
        // client invokable methods
        _hubConnection.On<RealtimeMetaData, VideoFrameCaptured>(MethodName.InvokeVideoFrameCapture, async (metaData) => await InvokeAsync(metaData));

        // subscribe to messages
        _ = _hubConnection.On<VideoFrameCaptured>(MethodName.VideoFrameCaptured, InvokeAsync);

        // launch the signalR connection in the background.
        _ = ConnectWithRetryAsync(_cts.Token);

        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
    
    private async Task InvokeAsync(VideoFrameCaptured args)
    {
        if (OnVideoFrameCapturedAsync is not null)
        {
            var asyncHandlers = OnVideoFrameCapturedAsync
                .GetInvocationList()
                .Cast<VideoFrameCapturedEventHandler>()
                .Select(h => h.Invoke(this, args))
                .ToArray();

            await Task.WhenAll(asyncHandlers);
        }
    }

    private Task<VideoFrameCaptured> InvokeAsync(RealtimeMetaData args)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(_invokeVideoFrameCapturedAsync);

        return _invokeVideoFrameCapturedAsync.Invoke(args);
    }
    
    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        _ = _hubConnection ?? throw new InvalidOperationException("HubConnection can't be null.");
        
        while (true)
        {
            try
            {
                await _hubConnection.StartAsync(cancellationToken);
                OnConnectionChanged?.Invoke(this, ConnectionId);
                return;
            }
            catch when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch
            {
                await Task.Delay(5000, cancellationToken);
            }
        }
    }
}

internal class IndefiniteRetryPolicy : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext) =>
        retryContext.PreviousRetryCount switch
        {
            0 => TimeSpan.Zero,
            1 => TimeSpan.FromSeconds(2),
            2 => TimeSpan.FromSeconds(5),
            _ => TimeSpan.FromSeconds(10)
        };
}