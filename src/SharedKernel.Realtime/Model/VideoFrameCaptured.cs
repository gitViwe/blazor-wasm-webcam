namespace SharedKernel.Realtime.Model;

public record VideoFrameCaptured(string Base64String) : IRealtimeMetaData
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string? DisplayName { get; init; }
}

public delegate Task VideoFrameCapturedEventHandler(object sender, VideoFrameCaptured args);