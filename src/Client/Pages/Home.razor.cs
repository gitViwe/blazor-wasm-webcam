using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Client.Pages;

public partial class Home
{
    private string _imageSrc = string.Empty;
    [Inject] public required IJSRuntime JsRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await JsRuntime.InvokeVoidAsync("StartVideo", "videoElement");
        _ = CaptureFramePeriodicallyAsync();
    }

    private async Task CaptureFramePeriodicallyAsync()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            _imageSrc = await JsRuntime.InvokeAsync<string>("GetFrame", "videoElement", "canvasElement");
        }
    }
}