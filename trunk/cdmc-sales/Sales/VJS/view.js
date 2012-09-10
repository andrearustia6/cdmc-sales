function initialaddLeadCallSheet($luncher,$win) {

    $win.find('#editor_submit').click(function () {


        var fp = $win.find('IsFirstPitch').val();
        var sheet = $win.find('LeadCallSheetTypeID').val();
        var date = $win.find('CallBackDate').val();
        var common = $win.find('Result').val();

        var callresult = {IsFirstPitch:fp,LeadCallSheetTypeID:sheet,CallBackDate:date,Result:common};

         callresult = JSON.stringify(callresult);

        $.ajax({
            url: "/Lead/CRM_Save",
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: { callresult: callresult },
            success: function (result) {
                if (result) {

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