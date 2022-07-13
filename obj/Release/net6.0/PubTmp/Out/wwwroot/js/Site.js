$(function () {
    var createRoomBtn = $(".createRoomBtn");
    var createRoomModal = $("#createRoomModal");
    var closeRoomModal = $("#closeRoomModal");
    createRoomBtn.on("click", function () {
        createRoomModal.addClass("active");
    });
    closeRoomModal.on("click", function () {
        createRoomModal.removeClass("active");
    });
    var menuswitch = $("#switch");
    var menu1 = $("#menu1");
    var menu2 = $("#menu2");
    menuswitch.on("change", function () {
        if (menuswitch.prop("checked")) {
            menu2.addClass("show").removeClass("hide");
            menu1.addClass("hide").removeClass("show");
            createRoomBtn.addClass("hide");
        } else {
            menu1.addClass("show").removeClass("hide");
            menu2.addClass("hide").removeClass("show");
            createRoomBtn.removeClass("hide");
        }
    })
});