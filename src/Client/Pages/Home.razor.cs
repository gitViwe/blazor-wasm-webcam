using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedKernel.Realtime.Client;
using SharedKernel.Realtime.Constant;
using SharedKernel.Realtime.Model;

namespace Client.Pages;

public partial class Home
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Inject] public required HttpClient HttpClient { get; set; }
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
            await Task.Delay(TimeSpan.FromSeconds(1));
            var imageBase64 = await JsRuntime.InvokeAsync<string>("GetFrame", "videoElement", "canvasElement");

            _ = HttpClient.PostAsJsonAsync(Route.Gateway.BroadcastVideoFrameCaptured, new VideoFrameCaptured(imageBase64));
        }
    }
}