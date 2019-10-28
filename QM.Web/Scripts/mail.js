//邮箱检查

$(function () {

    //收件人添加
    $("#to_add").on("click", function () {
        var mail = $("#to").val();
        if (mail == "") {
            $("#to").addClass("error");
            $("#to").focus();            
        } else {
            //正则表达式判断是否为邮箱格式
            var reg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
            if (reg.test(mail)) {
                //判断是否已经添加
                if ($("#to_list > option[value='" + mail + "']").val() == undefined) {
                    $("#to_list").append("<option value='" + mail + "'>" + mail + "</option>");
                    $("#to").val("");
                    $("#to").removeClass("error");
                    if ($("#qm_to").val() == "") {
                        $("#qm_to").val(mail);
                    } else {
                        $("#qm_to").val($("#qm_to").val() + ";" + mail);
                    }
                } else {                    
                    $("#to").addClass("error");
                }

            } else {
                $("#to").addClass("error");
            }
        }
    });

    //收件人删除
    $("#to_remove").on("click", function () {
        $("#to_list").find("option:selected").each(function () {
            var rval = $(this).val();
            $("#to_list > option[value='" + rval + "']").remove();
            var nval = $("#qm_to").val();
            $("#qm_to").val($("#qm_to").val().replace(rval + ";", "").replace(rval, ""));
        });                    
    });

    //抄送人添加
    $("#cc_add").on("click", function () {
        var mail = $("#cc").val();
        if (mail == "") {
            $("#cc").addClass("error");
            $("#cc").focus();
        } else {
            //正则表达式判断是否为邮箱格式
            var reg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
            if (reg.test(mail)) {
                //判断是否已经添加
                if ($("#cc_list > option[value='" + mail + "']").val() == undefined) {
                    $("#cc_list").append("<option value='" + mail + "'>" + mail + "</option>");
                    $("#cc").val("");
                    $("#cc").removeClass("error");
                    if ($("#qm_cc").val() == "") {
                        $("#qm_cc").val(mail);
                    } else {
                        $("#qm_cc").val($("#qm_cc").val() + ";" + mail);                        
                    }
                } else {
                    $("#cc").addClass("error");
                }

            } else {
                $("#cc").addClass("error");
            }
        }
    });

    //抄送人删除
    $("#cc_remove").on("click", function () {
        $("#cc_list").find("option:selected").each(function () {
            var rval = $(this).val();
            $("#cc_list > option[value='" + rval + "']").remove();
            $("#qm_cc").val($("#qm_cc").val().replace(rval + ";", "").replace(rval, ""));
        });
    });

    //密送人添加
    $("#bcc_add").on("click", function () {
        var mail = $("#bcc").val();
        if (mail == "") {
            $("#bcc").addClass("error");
            $("#bcc").focus();
        } else {
            //正则表达式判断是否为邮箱格式
            var reg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
            if (reg.test(mail)) {
                //判断是否已经添加
                if ($("#bcc_list > option[value='" + mail + "']").val() == undefined) {
                    $("#bcc_list").append("<option value='" + mail + "'>" + mail + "</option>");
                    $("#bcc").val("");
                    $("#bcc").removeClass("error");
                    if ($("#qm_bcc").val() == "") {
                        $("#qm_bcc").val(mail);
                    } else {
                        $("#qm_bcc").val($("#qm_bcc").val() + ";" + mail);
                    }
                } else {
                    $("#bcc").addClass("error");
                }

            } else {
                $("#bcc").addClass("error");
            }
        }
    });

    //密送人删除
    $("#bcc_remove").on("click", function () {
        $("#bcc_list").find("option:selected").each(function () {
            var rval = $(this).val();
            $("#bcc_list > option[value='" + rval + "']").remove();
            $("#qm_bcc").val($("#qm_bcc").val().replace(rval + ";", "").replace(rval, ""));
        });
    });

});