using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedKernel.Realtime.Client;
using SharedKernel.Realtime.Model;

namespace Client.Pages;

public partial class Home
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Inject] public required IRealtimeClient RealtimeClient { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RealtimeClient.ConnectAsync();
            await JsRuntime.InvokeVoidAsync("StartVideo", "videoElement");
            _ = CaptureFrameIndefinitelyAsync();
        }
    }

    private async Task CaptureFrameIndefinitelyAsync()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            var imageBase64 = await JsRuntime.InvokeAsync<string>("GetFrame", "videoElement", "canvasElement");

            _ = BroadcastFrameAsync(imageBase64);
        }
    }

    private async Task BroadcastFrameAsync(string imageBase64)
    {
        var imageData = Convert.FromBase64String(imageBase64.Split(',')[1]);
        await RealtimeClient.BroadcastVideoFrameCapturedAsync(new VideoFrameCaptured(imageData), CancellationToken.None);
    }
}