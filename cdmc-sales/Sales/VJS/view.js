function initialAddLeadCall($luncher, $win) {

    $win.find('#call_editor_submit').click(function () {


        var fp = $win.find('#IsFirstPitch').val();
        var type = $win.find('#LeadCallTypeID').val();
        var date = $win.find('#callBback').val();
        var common = $win.find('#Result').val();
        var projectid = $win.find('#ProjectID').val();
        var leadid = $win.find('#LeadID').val();
        var callresult = { IsFirstPitch: fp, LeadCallTypeID: type, CallBackDate: date, Result: common, ProjectID: projectid, LeadID: leadid };
        var typetext = $win.find("#LeadCallTypeID").find("option:selected").text();
        //var sv = $("select[selected='selected']").val();

        if (!fp) {
            alert("请选择是否属于First Pitch");
            return;
        }
        if (!type) {
            alert("请选择致电结果");
            return;
        }
        if (typetext == "Call-Backed" && date == "") {
            alert("请输入回访时间");
            return;
        }

        if (!leadid) {
            alert("LeadID 丢失");
            return;
        }

        callresult = JSON.stringify(callresult);

        $.ajax({
            url: "/CRM/Save_LeadCall",
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: callresult,
            error: function (jqXHR, textStatus, errorThrown) {
                alert(jqXHR.responseText);
            },
            success: function (result) {
                if (result) {
                    window.location.reload(true);

                    // $win.data('tWindow').close();
                }
            }
        });

    });

    $win.find('#call_editor_cancel').click(function () {
        $win.data('tWindow').close();
    });

    $luncher.click(function () {
        $win.data('tWindow').title('编辑框').open();
    });
}

function initialSetLeadPackage($luncher, $win) {

    $win.find('#package_editor_submit').click(function () {

        if (!leadid) {
            alert("LeadID 丢失");
            return;
        }

       // callresult = JSON.stringify(callresult);

        $.ajax({
            url: "/CRM/Save_LeadCall",
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: callresult,
            error: function (jqXHR, textStatus, errorThrown) {
                alert(jqXHR.responseText);
            },
            success: function (result) {
                if (result) {
                    window.location.reload(true);

                    // $win.data('tWindow').close();
                }
            }
        });

    });

    $win.find('#package_editor_cancel').click(function () {
        $win.data('tWindow').close();
    });

    $luncher.click(function () {
        $win.data('tWindow').title('编辑框').open();
    });
}

function getCallResultContent(win) {
    var $target = $('<div></div>');
    


    $target.data('av', array);
    var $table = $target.getTable({
        save: function () {
            
        },
        cancel: function () {
            win.data('tWindow').close();
        }
    });
       return $table;
}