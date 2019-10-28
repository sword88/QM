
$(document).ready(function () {

    // MetsiMenu
    $('#side-menu').metisMenu();

    // 打开右侧边栏
    $('.right-sidebar-toggle').click(function () {
        $('#right-sidebar').toggleClass('sidebar-open');
    });

    //固定菜单栏
    $(function () {
        $('.sidebar-collapse').slimScroll({
            height: '100%',
            railOpacity: 0.9,
            alwaysVisible: false
        });
    });


    // 菜单切换
    $('.navbar-minimalize').click(function () {
        $("body").toggleClass("mini-navbar");
        SmoothlyMenu();
    });


    // 侧边栏高度
    function fix_height() {
        var heightWithoutNavbar = $("body > #wrapper").height() - 61;
        $(".sidebard-panel").css("min-height", heightWithoutNavbar + "px");
    }
    fix_height();

    $(window).bind("load resize click scroll", function () {
        if (!$("body").hasClass('body-small')) {
            fix_height();
        }
    });

    //侧边栏滚动
    $(window).scroll(function () {
        if ($(window).scrollTop() > 0 && !$('body').hasClass('fixed-nav')) {
            $('#right-sidebar').addClass('sidebar-top');
        } else {
            $('#right-sidebar').removeClass('sidebar-top');
        }
    });

    $('.full-height-scroll').slimScroll({
        height: '100%'
    });

    $('#side-menu>li').click(function () {
        if ($('body').hasClass('mini-navbar')) {
            NavToggle();
        }
    });
    $('#side-menu>li li a').click(function () {
        if ($(window).width() < 769) {
            NavToggle();
        }
    });

    $('.nav-close').click(NavToggle);

    //ios浏览器兼容性处理
    if (/(iPhone|iPad|iPod|iOS)/i.test(navigator.userAgent)) {
        $('#content-main').css('overflow-y', 'auto');
    }

    //sendby
    $("#qm_sendby").change(function () {
        var sendby = $("#qm_sendby").val();

        if (sendby == "") {
            $("#mail_show").hide();
            $("#ftp_show").hide();
        } else if (sendby == "MAIL") {
            $("#mail_show").show();
            $("#ftp_show").hide();
        } else if (sendby == "FTP") {
            $("#ftp_show").show();
            $("#mail_show").hide();
        } else {
            $("#mail_show").show();
            $("#ftp_show").show();
        }
    });

    $("#qm_type").change(function () {
        var type = $("#qm_type").val();

        if (type == "SQL-FILE") {
            $("input[name='qm_dbcon']").removeAttr("disabled");
            $("input[name='qm_dbcon']").attr("placeholder", "数据库连接[userid/password@DB]");
            //$("#qm_sendby").attr("disabled", "true");
            $("#sql_show").hide();
            $("#qm_file").show();
            $("#qm_sendby").val('').trigger('change');
            $("#qm_sql").val('').hide();
        } else if (type == "SQL-EXP") {
            $("input[name='qm_dbcon']").removeAttr("disabled");
            $("input[name='qm_dbcon']").attr("placeholder", "数据库连接[DATA SOURCE=IP:Port/DBNAME;PASSWORD=****;USER ID=****]");
            //$("#qm_sendby").removeAttr("disabled");
            $("#sql_show").show();
            $("#qm_file").val('').hide();
            $("#qm_sql").show();
        } else {
            $("input[name='qm_dbcon']").attr("placeholder", "不需要输入");
            $("input[name='qm_dbcon']").val("");
            $("input[name='qm_dbcon']").attr("disabled", "true");
            //$("#qm_sendby").attr("disabled", "true");
            $("#sql_show").hide();
            $("#qm_sql").val('').hide();
            $("#qm_sendby").val('').trigger('change');
            $("#qm_file").show();
        }

    });

});

$(window).bind("load resize", function () {
    if ($(this).width() < 769) {
        $('body').addClass('mini-navbar');
        $('.navbar-static-side').fadeIn();
    }
});

function NavToggle() {
    $('.navbar-minimalize').trigger('click');
}

function SmoothlyMenu() {
    if (!$('body').hasClass('mini-navbar')) {
        $('#side-menu').hide();
        setTimeout(
            function () {
                $('#side-menu').fadeIn(500);
            }, 100);
    } else if ($('body').hasClass('fixed-sidebar')) {
        $('#side-menu').hide();
        setTimeout(
            function () {
                $('#side-menu').fadeIn(500);
            }, 300);
    } else {
        $('#side-menu').removeAttr('style');
    }
}