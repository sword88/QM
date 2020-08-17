﻿
(function ($, window, document) {
    $(function () {
        var cronExpression = {
            init: function () {

            },
            submit: function () {
                $.ajax({
                    url: "/Task/NextFireTime",
                    type: "post",
                    dataType: "json",
                    data: { "CronExpression": $("#cron").val() },
                    success: function (data) {
                        if (data.result) {
                            var result = eval("(" + data.msg + ")");
                            var strHTML = "";
                            for (var i = 0; i < result.length; i++) {
                                strHTML += result[i] + "\r\n";
                            }

                            $("#command").html(strHTML);
                        } else {
                            $("#command").html("");
                        }
                    }
                });
            },
            generate: function () {
                var v_input = $("input[name^='v_']");
                var boxs = $("div.box");
                var item = [];
                v_input.each(function () {
                    var obj = this;
                    item.push($(obj).val());
                });

                var currentIndex = 0;
                $("#menu1 li").each(function (i, li) {
                    if ($(li).hasClass("hover")) {
                        currentIndex = i;
                        return false;
                    }
                });

                if (currentIndex == 0)
                {
                    $("#menu1 li").each(function (i, li) {
                        if ($(li).hasClass("active")) {
                            currentIndex = i;
                            return false;
                        }
                    });
                }


                var start = currentIndex > 3 ? 3 : currentIndex;
                for (var i = start; i >= 0 ; i--) {
                    if (item[currentIndex] != "*" && item[i - 1] == "*") {
                        item[i - 1] = "0";
                        $(v_input[i - 1]).val("0");
                        //// 把的之前的选项变成指定，并默认第一个
                        var box = boxs[i - 1];
                        $(box).find(".choose").attr("checked", true);
                        $(box).find("div[class$='_list']").children().eq(0).attr("checked", true);
                    }
                }

                $("#cron").val(item.join(" "));
            },
            save: function () {
                $("input[name='qm_cron']").val($("#cron").val());
                $("#modal-form").removeClass("in");
                $("#modal-form").addClass("out");
            },
            everyTime: function (name) {
                var item = $("input[name=v_" + name + "]");
                item.val("*");
                cron.generate();
            },
            nochoose: function (name) {
                var val = "?";
                if (name == "year")
                    val = "";
                var item = $("input[name=v_" + name + "]");
                item.val(val);
                cron.generate();
            },
            cycle: function (name) {
                var ns = $("#" + name + "_chooser_cycle").parent().find(".numberspinner");
                var start = ns.eq(0).val();
                var end = ns.eq(1).val();
                var item = $("input[name=v_" + name + "]");
                item.val(start + "-" + end);

                // 如果是周，则把日 设置为不指定
                if (name == "weekday") {
                    $("#day_chooser_nochoose").attr("checked", true);
                    var item = $("input[name=v_day]").val("?");
                }
                if (name == "day") {
                    $("#weekday_chooser_nochoose").attr("checked", true);
                    var item = $("input[name=v_weekday]").val("?");
                }

                cron.generate();
            },
            start: function (name) {
                var ns = $("#" + name + "_chooser_start").parent().find(".numberspinner");

                var start = ns.eq(0).val();
                var end = ns.eq(1).val();
                var val = "";
                if (name == "weekday") {
                    val = start + "#" + end;

                    $("#day_chooser_nochoose").attr("checked", true);
                    var item = $("input[name=v_day]").val("?");
                }
                else {
                    if (name == "day") {
                        $("#weekday_chooser_nochoose").attr("checked", true);
                        var item = $("input[name=v_weekday]").val("?");
                    }
                    val = start + "/" + end;
                }

                var item = $("input[name=v_" + name + "]");
                item.val(val);
                cron.generate();
            }
            ,
            choose: function (name) {
                var checked = $("#" + name + "_chooser_choose").prop("checked");
                var checklist = $("." + name + "_list").children();
                if (checked) {
                    if ($(checklist).filter(":checked").length == 0) {
                        $(checklist.eq(0)).attr("checked", true);
                    }
                    checklist.eq(0).change();
                }
            },
            checklist: function (name) {
                var appoint = $("#" + name + "_chooser_choose").prop("checked");
                var checklist = $("." + name + "_list").children();
                if (appoint) {
                    var vals = [];
                    checklist.each(function () {
                        if (this.checked) {
                            vals.push(this.value);
                        }
                    });
                    var val = "?";
                    if (vals.length > 0 && vals.length < checklist.length) {
                        val = vals.join(",");

                        // 如果是周，则把日 设置为不指定
                        if (name == "weekday") {
                            $("#day_chooser_nochoose").attr("checked", true);
                            var item = $("input[name=v_day]").val("?");
                        }
                        if (name == "day") {
                            $("#weekday_chooser_nochoose").attr("checked", true);
                            var item = $("input[name=v_weekday]").val("?");
                        }

                    } else if (vals.length == checklist.length) {
                        val = "*";
                    }
                    var item = $("input[name=v_" + name + "]");
                    item.val(val);
                    cron.generate();
                }
            }
        };

        window.cron = cronExpression;
    });
})(jQuery, window);

$(function () {
    $("#generate,#submit,#cronsave").on("click", function (e) {
        e.preventDefault();
        var the = $(this);
        if (the.is("#generate")) {
            cron.generate();
            cron.submit();
        }
        if (the.is("#submit")) {
            cron.submit();
        }
        if (the.is("#cronsave")) {
            cron.save();
        }
    });

    $(".numberspinner").blur(function () {
        var the = $(this).parent().find("input[type=radio]");
        var ischecked = the.prop("checked");
        var name = the.attr("name").split("_")[0]

        if (ischecked)
        {
            if (the.is(".every")) {
                cron.everyTime(name);
            }
            if (the.is(".nochoose")) {
                cron.nochoose(name);
            }
            if (the.is(".choose")) {
                cron.choose(name);
            }
            if (the.is(".cycle")) {
                cron.cycle(name);
            }
            if (the.is(".start")) {
                cron.start(name);
            }
        }

    });

    $(".every,.nochoose,.choose,.cycle,.start").on("click", function (e) {
        var the = $(this);
        if (the.is(".every")) {
            var name = the.attr("name").split("_")[0];
            cron.everyTime(name);
        }
        if (the.is(".nochoose")) {
            var name = the.attr("name").split("_")[0];
            cron.nochoose(name);
        }
        if (the.is(".choose")) {
            var name = the.attr("name").split("_")[0];
            cron.choose(name);
        }
        if (the.is(".cycle")) {
            var name = the.attr("name").split("_")[0];
            cron.cycle(name);
        }
        if (the.is(".start")) {
            var name = the.attr("name").split("_")[0];
            cron.start(name);
        }
    });

    $("div[class$='_list']").change(function () {
        var name = $(this).closest("div[class$='_list']").attr("class").split("_")[0];
        cron.checklist(name);
    });

    $("#menu1 li").on("click", function () {
        var currentIndex = $(this).attr("data-value");
        var tli = $("#menu1").find("li");
        var mli = $("#main1").children("div");
        for (i = 0; i < tli.length; i++) {
            tli[i].className = i == currentIndex ? "hover" : "";
            mli[i].style.display = i == currentIndex ? "block" : "none";
        }
    });


});