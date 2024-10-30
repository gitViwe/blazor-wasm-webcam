
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedKernel.Realtime.Client;
using SharedKernel.Realtime.Model;

namespace Client.Pages;

public partial class Home
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Inject] public required IRealtimeClient RealtimeClient { get; set; }

    private string? _connectionId;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            RealtimeClient.OnConnectionChanged += (_, id) =>
            {
                _connectionId = id;
                StateHasChanged();
            };
            await RealtimeClient.ConnectAsync(InvokableCaptureFrame);
            await JsRuntime.InvokeVoidAsync("StartVideo", "videoElement");
        }
    }

    private async Task<VideoFrameCaptured> InvokableCaptureFrame(RealtimeMetaData metaData)
    {
        var imageBase64 = await JsRuntime.InvokeAsync<string>("GetFrame", "videoElement", "canvasElement");

        return new VideoFrameCaptured(imageBase64)
        {
            Id = metaData.Id,
            DisplayName = metaData.DisplayName,
        };
    }
}