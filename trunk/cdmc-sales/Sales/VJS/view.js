function initialaddLeadCallSheet($luncher,$win) {

    $luncher.click(function () {

        $win.data('tWindow').content(getCallResultContent($win)).title('编辑框').open();
    });
}

function getCallResultContent(win) {
    var $target = $('<div></div>');
    
    var array = new Array();
    var data;
    array.push({ controltype: 'radio', fieldname: 'First Pitch?', fieldvalue: data == null ? null : data.ConditionName, optionalvalue: '是||否' });
    array.push({ controltype: 'radio', fieldname: 'Call Result?', fieldvalue: data == null ? null : data.ConditionName, optionalvalue: 'Not Pitched||Pitched||Call-Backed||Waiting for Approval||Closed||Blowed' });


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