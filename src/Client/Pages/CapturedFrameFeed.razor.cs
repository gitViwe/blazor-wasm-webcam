using Microsoft.AspNetCore.Components;
using SharedKernel.Realtime.Client;
using SharedKernel.Realtime.Model;

namespace Client.Pages;

public partial class CapturedFrameFeed : IDisposable
{
    [Inject] public required IRealtimeClient RealtimeClient { get; set; }

    private string _imageSrc = string.Empty;

    private Task RealtimeClientOnOnVideoFrameCapturedAsync(object sender, VideoFrameCaptured args)
    {
        _imageSrc = args.Base64String;
        StateHasChanged();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        RealtimeClient.OnVideoFrameCapturedAsync -= RealtimeClientOnOnVideoFrameCapturedAsync;
        GC.SuppressFinalize(this);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            RealtimeClient.OnVideoFrameCapturedAsync += RealtimeClientOnOnVideoFrameCapturedAsync;
        }
    }
}