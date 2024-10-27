namespace SharedKernel.Realtime.Model;

public record VideoFrameCaptured(string Base64String);
public delegate Task VideoFrameCapturedEventHandler(object sender, VideoFrameCaptured args);