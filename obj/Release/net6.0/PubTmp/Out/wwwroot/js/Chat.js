var inputform = $("#inputform");
var chatbody = $(".chat-body");
inputform.on("submit", SendMessage);
chatbody.scrollTop(chatbody[0].scrollHeight);
$(window).resize(function () {
    chatbody.scrollTop(chatbody[0].scrollHeight);
});