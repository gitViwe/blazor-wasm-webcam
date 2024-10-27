namespace SharedKernel.Realtime.Model;

public record VideoFrameCaptured(byte[] ImageData);
public delegate Task VideoFrameCapturedEventHandler(object sender, VideoFrameCaptured args);