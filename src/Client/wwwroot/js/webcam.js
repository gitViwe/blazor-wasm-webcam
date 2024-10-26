window.StartVideo = async function StartVideo(elementId) {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        
        let constraints = {
            audio: false,
            video: {
                facingMode: { exact: "environment" },
                frameRate: { ideal: 10, max: 15 }
            }
        };
        
        let stream = await navigator.mediaDevices.getUserMedia(constraints);
        
        let video = document.getElementById(elementId);

        if ("srcObject" in video) { video.srcObject = stream; }
        else { video.src = window.URL.createObjectURL(stream); }
        
        video.onloadedmetadata = function (e) {
            video.play();
        };
    }
}

window.GetFrame = function GetFrame(videoElementId, canvasElementId) {
    let video = document.getElementById(videoElementId);
    
    let canvas = document.getElementById(canvasElementId);
    //canvas.getContext('2d').drawImage(video, 0, 0, 320, 240);

    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    canvas.getContext('2d').drawImage(video, 0, 0, video.videoWidth, video.videoHeight);

    let output= canvas.toDataURL("image/jpeg");
    
    return output;
}