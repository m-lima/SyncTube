 $(function () {
	stream = $.connection.streamHub;

	stream.client.broadcastLobbyStatus = function (roomsStatuses) {
		var rooms = JSON.parse(roomsStatuses);
		for (i = 0; i < rooms.length; i++) {

			var roomid = rooms[i].RoomId;
			var percent = rooms[i].Time;
			var status = rooms[i].CurrentStatus;

			var viewCounter = $('#' + roomid + 'view')[0];
			var duration = $('#' + roomid + 'duration')[0];
			var progress = $('#' + roomid + '.progress-bar')[0];
			var progressTooltip = $('#' + roomid + '.progress')[0];
			var icon = $('#' + roomid + '.text-success')[0];

			var durationValue = duration.innerHTML;
			durationValue = durationValue.substr(10, durationValue.length - 12);
			durationValue = parseInt(durationValue);
			if (durationValue > 0) {
				percent = (percent * 100 / durationValue);
			} else {
				percent = 0;
			}

			viewCounter.innerHTML = "Viewers: " + rooms[i].UserCount;
			progress.attributes['aria-valuenow'] = percent;
			progress.style['width'] = percent + '%';
			progressTooltip.title = rooms[i].Time + "s elapsed";

			if (status == "play") {
				icon.className = "text-success glyphicon glyphicon-play";
			} else if (status == "pause") {
				icon.className = "text-success glyphicon glyphicon-pause";
			} else {
				icon.className = "text-success glyphicon glyphicon-stop";
			}
		}
	};

	$.connection.hub.start().done(function () {
		stream.server.joinLobby();
		stream.server.requestLobbyStatus();
	});
});