namespace SharedKernel.Realtime.Model;

public record VideoFrameCaptured(byte[] ImageData)
{
    /// <summary>
    /// The image file type
    /// </summary>
    /// <remarks>
    /// This can be used to create a fully qualified base64 string<br/>
    /// <code>$"data:{ContentType};base64," + Convert.ToBase64String(ImageData);</code>
    /// </remarks>
    public string ContentType => "image/jpeg";
};
public delegate Task VideoFrameCapturedEventHandler(object sender, VideoFrameCaptured args);