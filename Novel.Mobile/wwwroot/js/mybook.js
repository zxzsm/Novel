$(function () {
    $("#mbook").click(function () {
        $("#mybook").show();
        $("#myhistory").hide();
        $("#hbook").removeClass("active");
        $("#mbook").addClass("active");
    });
    $("#hbook").click(function () {
        $("#mybook").hide();
        $("#myhistory").show();
        $("#hbook").addClass("active");
        $("#mbook").removeClass("active");

    });
});