using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using SharedKernel.Realtime.Constant;
using SharedKernel.Realtime.Model;

namespace SharedKernel.Realtime.Client;

public class RealtimeClient(IConfiguration configuration) : IRealtimeClient
{
    public event VideoFrameCapturedEventHandler? OnVideoFrameCapturedAsync;
    
    private HubConnection? _hubConnection;
    private readonly CancellationTokenSource _cts = new();
    
    public Task ConnectAsync()
    {
        if (_hubConnection?.State != HubConnectionState.Disconnected)
        {
            return Task.CompletedTask;
        }

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(configuration["ApiGatewayBaseUrl"] + "/notification")
            .WithAutomaticReconnect(new IndefiniteRetryPolicy())
            .Build();

        _hubConnection.Closed += async _ =>
        {
            // Wait a bit and restart the connection again.
            await Task.Delay(5000, _cts.Token);
            await ConnectWithRetryAsync(_cts.Token);
        };

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
    
    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        _ = _hubConnection ?? throw new InvalidOperationException("HubConnection can't be null.");
        
        while (true)
        {
            try
            {
                await _hubConnection.StartAsync(cancellationToken);
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