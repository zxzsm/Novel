//判断滚动条是否到达窗口底部
$(window).bind('scroll', function () {    //绑定滚动事件
    var scrollTopHeight = document.documentElement.scrollTop || document.body.scrollTop;
    if (scrollTopHeight >= 400) {
        $(".mp-top").show();
    }
    else {
        $(".mp-top").hide();
    }
});