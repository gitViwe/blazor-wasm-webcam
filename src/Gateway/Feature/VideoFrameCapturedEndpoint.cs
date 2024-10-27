using Gateway.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Realtime.Model;

namespace Gateway.Feature;

internal static class VideoFrameCapturedEndpoint
{
    internal static void MapVideoFrameCapturedEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(SharedKernel.Realtime.Constant.Route.Gateway.BroadcastVideoFrameCaptured, VideoFrameCapturedAsync);
    }

    private static async Task<IResult> VideoFrameCapturedAsync(
        [FromServices] RealtimeServerHubContext hubContext,
        [FromBody] VideoFrameCaptured videoFrameCaptured,
        CancellationToken cancellation = default)
    {
        await hubContext.BroadcastVideoFrameCapturedAsync(videoFrameCaptured, cancellation);

        // get image tags in background
        
        return Results.NoContent();
    }
}