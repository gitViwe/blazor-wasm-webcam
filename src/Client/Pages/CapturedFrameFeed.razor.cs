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
        _imageSrc = $"data:{args.ContentType};base64," + Convert.ToBase64String(args.ImageData);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        RealtimeClient.OnVideoFrameCapturedAsync -= RealtimeClientOnOnVideoFrameCapturedAsync;
        GC.SuppressFinalize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RealtimeClient.ConnectAsync();
            RealtimeClient.OnVideoFrameCapturedAsync += RealtimeClientOnOnVideoFrameCapturedAsync;
        }
    }
}