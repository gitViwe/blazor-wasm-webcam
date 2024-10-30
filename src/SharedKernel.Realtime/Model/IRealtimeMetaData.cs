namespace SharedKernel.Realtime.Model;

public interface IRealtimeMetaData
{
    string Id { get; }
    string? DisplayName { get; }
}

public class RealtimeMetaData : IRealtimeMetaData
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string? DisplayName { get; init; }
}