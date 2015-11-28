//https://developers.google.com/youtube/iframe_api_reference?hl=en

var player;
var roomFrame;
var roomVideo;
var videoCallback = {};

videoCallback.playing = function(currentTime) {};
videoCallback.ended = function() {};
videoCallback.paused = function(currentTime) {};
videoCallback.jumped = function(currentTime) {};
videoCallback.timeChanged = function(currentTime) {};
videoCallback.buffering = function(currentTime) {};

function initYoutube(frameId, videoId) {
    var tag = document.createElement("script");

    roomFrame = frameId;
    roomVideo = videoId;

    tag.src = "https://www.youtube.com/iframe_api";
    var firstScriptTag = document.getElementsByTagName("script")[0];
    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
}

function onYouTubeIframeAPIReady() {
    player = new YT.Player(roomFrame, {
        height: "390",
        width: "640",
        videoId: roomVideo,
        playerVars: {
            start: "0"
        },
        events: {
            'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
        }
    });

    var lastTime = 0;
    setInterval(function() {
        var currentTime = player.getCurrentTime();
        if (currentTime - lastTime < 0 || currentTime - lastTime > 2) {
            videoCallback.jumped(currentTime);
        }
        lastTime = currentTime;
        videoCallback.timeChanged(currentTime);
    }, 1000);
}

function onPlayerReady(event) {
//    event.target.playVideo();
}

function onPlayerStateChange(event) {
    if (event.data == YT.PlayerState.ENDED) {
        videoCallback.ended();
    } else if (event.data == YT.PlayerState.PLAYING) {
        videoCallback.playing(player.getCurrentTime());
    } else if (event.data == YT.PlayerState.PAUSED) {
        videoCallback.paused(player.getCurrentTime());
    } else if (event.data == YT.PlayerState.BUFFERING) {
        videoCallback.buffering(player.getCurrentTime());
    } else if (event.data == YT.PlayerState.CUED) {
//        onPlayerCued();
//    } else if (event == 'JUMP') {
//        videoCallback.jumped();
    } else {
//        onPlayerUnstarted();
    }
}