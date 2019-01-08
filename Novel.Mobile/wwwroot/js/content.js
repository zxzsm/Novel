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
    $(document).swipeLeft(function () {
        if ($("#next")) {
            $("#next")[0].click();
        }
    });
    $(document).swipeRight(function () {
        if ($("#pre")) {
            $("#pre")[0].click();
        }
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
    $.cookie("rsetting", JSON.stringify(setting), { expires: 365 * 10 });
}
