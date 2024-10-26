using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Client.Pages;

public partial class Home
{
    private string _imageSrc = string.Empty;
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }
    private string Base64 { get; set; }

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
            Base64 = await JsRuntime.InvokeAsync<string>("GetFrame", "videoElement", "canvasElement");
        }
    }
}