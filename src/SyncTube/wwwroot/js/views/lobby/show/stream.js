var stream;

$(function () {
    stream = $.connection.streamHub;

    stream.client.broadcastStatus = function(command, value) {

        if (command == 'end') {
            if (player.getPlayerState() == YT.PlayerState.PLAYING) {
                player.seekTo(value, true);
                player.stopVideo();
            }
        } else if (command == 'play') {
            if (Math.abs(player.getCurrentTime() - value) > 2) {
                player.seekTo(value, true);
                player.playVideo();
            }
        } else if (command == 'pause') {
            if (Math.abs(player.getCurrentTime() - value) > 1) {
                player.seekTo(value, true);
            }
            if (player.getPlayerState() != YT.PlayerState.PAUSED) {
                player.pauseVideo();
            }
        }
   
    };

    $.connection.hub.start().done(function () {
        stream.server.createOrJoinRoom(roomId);
        stream.server.requestStatus(roomId);

        videoCallback.ended = function () {
            stream.server.setEnded(roomId);
        }
        videoCallback.playing = function (time) {
            stream.server.setPlaying(roomId, parseInt(time));
        }
        videoCallback.paused = function (time) {
            stream.server.setPaused(roomId, parseInt(time));
        }
        videoCallback.jumped = function (time) {
            if (player.getPlayerState() == YT.PlayerState.PAUSED) {
                stream.server.setPaused(roomId, parseInt(time));
            } else if (player.getPlayerState() == YT.PlayerState.PLAYING) {
                stream.server.setPlaying(roomId, parseInt(time));
            }
        }
    });
});