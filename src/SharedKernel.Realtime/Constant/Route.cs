namespace SharedKernel.Realtime.Constant;

public static class Route
{
    public static class Gateway
    {
        public const string BroadcastVideoFrameCaptured = "broadcast-video-frame-captured";
        public const string InvokeVideoFrameCapture = "invoke-video-frame-capture/{ConnectionId}";

    }
}