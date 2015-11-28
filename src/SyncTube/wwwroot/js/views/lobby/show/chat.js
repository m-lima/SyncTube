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
			chatWindow.append('<span class="label label-warning"><strong>' + encodedName + '</strong>:</span><br>');
		}
		chatWindow.append(encodedMsg + '<br>');
		lastName = name;
		chatWindow.scrollTop = chatWindow.scrollHeight;
	};

	chat.client.addChatMessage = function (message) {
		var encodedMsg = $('<div />').text(message).html();
		// Add the message to the page.
		var chatWindow = $('.chat-window');
		chatWindow.append('<i style="color: gray">' + encodedMsg + '</i><br>');
		chatWindow.scrollTop = chatWindow.scrollHeight;
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