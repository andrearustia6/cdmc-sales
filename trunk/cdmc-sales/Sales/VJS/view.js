﻿function onDistributionCompanyNodeDrop(e) {
    var dropContainer = $(e.dropTarget).closest('.drop-container');
    var treeview = $('#TreeView').data('tTreeView');
    var nodeText = treeview.getItemText(e.item);
    if (dropContainer.length > 0 && dropContainer[0].innerHTML.indexOf(nodeText) == -1) {
        $('<div><strong>' + nodeText + '</strong></div>').hide().appendTo(dropContainer).slideDown('fast');
     e.preventDefault(); }
}

function initoalCompanySelect() {
    $(".charatacter, .pane").sortable({
        connectWith: ".connectedSortable"
    });

    $(".pane").droppable({
        activeClass: "ui-state-hover",
        hoverClass: "ui-state-active",
        drop: function (event, ui) {
            var $this = $(this);
            var mid = $this.closest('.pane').attr('mid');
            ui.draggable.find('input').remove();
            var c = ui.draggable.text();
            ui.draggable.append('<input name="mc" type="hidden" value="' + mid + '|' + c + '"/>');
            $this.append(ui.draggable);
            return false;
        }
    });

    $(".charatacter").droppable({
        activeClass: "ui-state-hover",
        hoverClass: "ui-state-active",
        drop: function (event, ui) {
            var $drag = ui.draggable;
            $drag.find('input').remove();
            return false;
        }
    });

    
}
function initialAddLeadCall($luncher, $win) {

    $win.find('#call_editor_submit').click(function () {


        var fp = $win.find('#IsFirstPitch').val();
        var type = $win.find('#LeadCallTypeID').val();
        var date = $win.find('#callBback').val();
        var common = $win.find('#Result').val();
        var projectid = $('#ProjectID').val();
        var leadid = $('#ID').val();
        var callresult = { IsFirstPitch: fp, LeadCallTypeID: type, CallBackDate: date, Result: common, ProjectID: projectid, LeadID: leadid };
        var typetext = $win.find("#LeadCallTypeID").find("option:selected").text();

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
                    window.location.reload(true);
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
    
    $win.find('input[type=checkbox]').click(function () {
        $win.find('input[type=checkbox]').each(function () {
            $(this).removeAttr("checked");
        });
        $(this).attr('checked', 'checked');
    });



    $win.find('#package_editor_submit').click(function () {

     
        var leadid = $('#ID').val();
        if (!leadid) {
            alert("LeadID 丢失");
            return;
        }
        var projectid = $('#ProjectID').val();
        if (!projectid) {
            alert("ProjectID 丢失");
            return;
        }
        var id = $win.find("input[type=checkbox]:checked").val();
        $.ajax({
            url: "/CRM/Save_LeadPackage?leadid=" + leadid + "&projectid=" + projectid+"&packageid="+id,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',

            error: function (jqXHR, textStatus, errorThrown) {
                alert(jqXHR.responseText);
            },
            success: function (result) {
                window.location.reload(true);
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