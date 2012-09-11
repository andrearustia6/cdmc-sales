function initialaddLeadCallSheet($luncher,$win) {

    $win.find('#editor_submit').click(function () {


        var fp = $win.find('#IsFirstPitch').val();
        var type = $win.find('#LeadCallTypeID').val();
        var date = $win.find('#callBback').val();
        var common = $win.find('#Result').val();
        var projectid = $win.find('#ProjectID').val();
        var leadid = $win.find('#LeadID').val();
        var callresult = { IsFirstPitch: fp, LeadCallTypeID: type, CallBackDate: date, Result: common, ProjectID: projectid, LeadID: leadid };

        if (!fp) {
            alert("请选择是否属于First Pitch");
            return;
        }
        if (!callresult) {
            alert("请选择致电结果");
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

    $win.find('#editor_cancel').click(function () {
        $win.data('tWindow').close();
    });

    $luncher.click(function () {
        $win.data('tWindow').title('编辑框').open();
//        $win.data('tWindow').content(getCallResultContent($win)).title('编辑框').open();
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