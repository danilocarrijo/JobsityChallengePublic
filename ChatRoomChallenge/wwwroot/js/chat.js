"use strict";

const events = {
    MESSAGE: 0,
    NEWUSER: 1,
    REFRESH: 2
}

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, event) {
    switch (event) {
        case events.MESSAGE:
            addMessage(user, message);
            break;
        case events.NEWUSER:
            newUser(user, message)
        case events.REFRESH:
            refreshUserList()
            break;
    }
});

function refreshUserList() {
    $.ajax({
        url: 'Home/GetUsers',
        type: 'GET',
        dataType: 'json',
        success: function (res) {
            console.log(res);
            $('#users')
                .find('option')
                .remove()
                .end()
                .append('<option value="0">All</option>')
                .val('0')
                ;
            for (var i = 0; i < res.length; i++) {
                $("#users").append('<option value="' + res[i] + '">' + res[i] + '</option>');
            }
        }
    });
}

function newUser(user,id) {
    var o = new Option(user, id);
    $("#users").append('<option value="' + user+'">' + user + '</option>');
}

function addMessage(user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
}


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    $("#messageInput").val('');
    var signalRID = $("#users").val();
    connection.invoke("SendMessage", user, message, signalRID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
