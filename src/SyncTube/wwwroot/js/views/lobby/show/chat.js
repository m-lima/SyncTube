var roomId = $("#roomId").val();
$(function () {
	var chat = $.connection.chatHub;
	var lastName;

	chat.client.sendMessage = function (name, message) {
		if (!message || message == "") return;
		var encodedName = $('<div />').text(name).html();
		var encodedMsg = $('<div />').text(message).html();
		// Add the message to the page.
		var chatWindow = $('.chat-window');
		if (lastName != name) {
		    chatWindow.append('<span class="label label-warning" style="opacity: 0.0;"><strong>' + encodedName + '</strong>:</span><br>');
		    chatWindow.children("span").last().animate({ opacity: '1.0' }, "slow");
		}
		chatWindow.append('<p style="opacity: 0.0;">' + encodedMsg + '</p>');
		chatWindow.children("p").last().animate({ opacity: '1.0' }, "slow");
		lastName = name;
		chatWindow[0].scrollTop = chatWindow[0].scrollHeight;
	};

	chat.client.addChatMessage = function (message) {
		var encodedMsg = $('<div />').text(message).html();
		// Add the message to the page.
		var chatWindow = $('.chat-window');
		chatWindow.append('<i style="color: gray; opacity: 0.0;">' + encodedMsg + '</i><br>');
		chatWindow.children("i").last().animate({ opacity: '1.0' }, "slow");
		chatWindow[0].scrollTop = chatWindow[0].scrollHeight;
	};

	$('#message').focus();
	$.connection.hub.start().done(function () {
		chat.server.joinRoom(roomId);
		$('#sendmessage').click(function () {
			chat.server.send(roomId, $('#message').val());
			$('#message').val('').focus();
		});
	});


	submitMessage = function (event) {
		if (event.keyCode == 13) {
			chat.server.send(roomId, $('#message').val());
			$('#message').val('').focus();
			return false;
		}
	}
});