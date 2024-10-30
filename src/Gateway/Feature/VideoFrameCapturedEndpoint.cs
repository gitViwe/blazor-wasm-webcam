using Gateway.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Realtime.Model;

namespace Gateway.Feature;

internal static class VideoFrameCapturedEndpoint
{
    internal static void MapVideoFrameCapturedEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(SharedKernel.Realtime.Constant.Route.Gateway.BroadcastVideoFrameCaptured, BroadcastVideoFrameCapturedAsync)
            .WithTags("Broadcast");
        
        app.MapPost(SharedKernel.Realtime.Constant.Route.Gateway.InvokeVideoFrameCapture, InvokeVideoFrameCaptureAsync)
            .WithTags("Invoke Client")
            .Produces<VideoFrameCaptured>();
    }

    private static async Task<IResult> BroadcastVideoFrameCapturedAsync(
        [FromServices] RealtimeServerHubContext hubContext,
        [FromBody] VideoFrameCaptured videoFrameCaptured,
        CancellationToken cancellation = default)
    {
        await hubContext.BroadcastVideoFrameCapturedAsync(videoFrameCaptured, cancellation);
        
        return Results.NoContent();
    }
    
    private static async Task<IResult> InvokeVideoFrameCaptureAsync(
        [FromServices] RealtimeServerHubContext hubContext,
        [FromBody] RealtimeMetaData metaData,
        [FromRoute] string connectionId,
        CancellationToken cancellation = default)
    {
        var videoFrameCaptured = await hubContext.InvokeVideoFrameCaptureAsync(connectionId, metaData, cancellation);
        
        await hubContext.BroadcastVideoFrameCapturedAsync(videoFrameCaptured, cancellation);
        
        return Results.Ok(videoFrameCaptured);
    }
}