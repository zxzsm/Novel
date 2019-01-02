$(document).keyup(function (event) {
    var e = event || window.event || arguments.callee.caller.arguments[0];
    if (e.keyCode == 37 && $("#pre")) {
        $("#pre")[0].click();
    }
    else if (e.keyCode == 39 && $("#next")) {
        $("#next")[0].click();
    }
});
//判断滚动条是否到达窗口底部
$(window).bind('scroll', function () {    //绑定滚动事件
    var h = $("#header").height() + 100;
    if ($(document).scrollTop() >= h) {
        $(".mp-top").show();
        $(".left-bar").css("top", "0px");
    }
    else {
        $(".left-bar").css("top", "180px");
        $(".mp-top").hide();
    }
});
function bgselected(obj) {
    $(".rbgselected").removeClass("rbgselected");
    $(obj).addClass("rbgselected");
    var bgcolor = $(obj).css("background-color");
    $("#icontent").css("background-color", bgcolor);
    rsetting("bgcolor", $(obj).attr("title"));

}
function afselected(obj, val) {
    $(".afs").removeClass("afs");
    $(obj).addClass("afs");
    $(".ncontent").css("font-family", val);
    rsetting("fontfamily", $(obj).attr("title"));
}
function cfontSize(str) {
    var fontSize = parseInt($(".sfs").html());
    if (str == "+") {
        fontSize += 2;
    } else {
        fontSize -= 2;
    }
    if (fontSize < 16) {
        fontSize = 16;
    }
    else if (fontSize > 26) {
        fontSize = 26;
    }
    $(".sfs").html(fontSize);
    $(".ncontent").css("font-size", fontSize + "px");
    rsetting("fontsize", fontSize);
}
$(function () {
    $("#setting").click(function () {
        $(".c-setting").show();
    });

    $(".close").click(function () {
        $(".c-setting").hide();
    });

});

function rsetting(name, value) {
    var setting = $.cookie("rsetting");
    if (!setting) {
        setting = {};
    }
    else {
        setting = JSON.parse(setting);
    }
    setting[name] = value;
    $.cookie("rsetting", JSON.stringify(setting), { expires: 365*10 });
}
