
function onSelectOrDiselect() {
   
    var $selectall = $('#selectall');
    var $disselectall = $('#disselectall');
    $('#selectall,#disselectall').bind('click', function () {
        var $this = $(this);
        if ($this.attr('id') == 'selectall') {
            $('.selectedprojects').attr('checked', 'checked');
            $disselectall.removeAttr("checked");
            $('#selecttype').val('selectall');
        }
        else {
            $('.selectedprojects').removeAttr("checked");
            $selectall.removeAttr("checked");
            $('#selecttype').val('disselectall');
        }

    });

   
}

//callist member 下拉筛选
function onMemberSelected() {
    $('.memberselect').change(function () {
        var $this = $(this);
        var gridid = $this.attr('gridid')
        var member = $this.val();
        var grid = $('#' + gridid).data('tGrid');
        if (member) {
            grid.filterBy = "Member~eq~'" + member + "'";
            //grid.rebind(); 
            grid.ajaxRequest();
        }
        else {
            grid.filterBy = "";
            //grid.rebind(); 
            grid.ajaxRequest();
        }

    });
}

function onColorSet() {

    $('.calltype').each(function () {
        var $this = $(this);
        if ($this.html() == "Qualified Decision") {
            $this.css('background-color', 'yellow');
            return;
        }

        if ($this.html() == "Closed") {
            $this.css('background-color', '#CEFFCE');
            return;
        }

        if ($this.html() == "Blowed") {
            $this.css('background-color', '#E0E0E0');
            return;
        }

        if ($this.html() == "Waiting for Approval") {
            $this.css('background-color', '#D3A4FF');
            return;
        }
    });
}


function onCallListDataBound(e) {
    var grid = $("#Grid").data('tGrid');    
    var $exportLink = $('#export');  
    var href = $exportLink.attr('href');
    // Update the 'page' parameter with the grid's current page      
    //href = href.replace(/page=([^&]*)/, 'page=' + grid.currentPage);
    // Update the 'orderBy' parameter with the grids' current sort state  
   // href = href.replace(/orderBy=([^&]*)/, 'orderBy=' + (grid.orderBy || '~'));
    // Update the 'filter' parameter with the grids' current filtering state      
    href = href.replace(/filter=(.*)/, 'filter=' + (grid.filterBy || '~'));
       // Update the 'href' attribute    
     $exportLink.attr('href', href);
}

function onManagementTabLoad(e) {
    if (e) {
        var $t = $(e.contentElement);
        var id = $t.attr("id");
        var arrstr = id.split("-");
        var index = parseInt(arrstr[1]) - 1;
        $('#tabindex').val(index);

        $t.find('.t-numeric a').each(function () {
            var $this = $(this);
            var href = $this.attr('href');
            var sprit = href.split("&tabindex=");
            if (sprit.length == 1) {
                href += '&tabindex=' + index;
                $this.attr('href', href);
            }
        });
    }
}
function initialTargetbreakdown() {
//     $(this).closest('table').find('.result').text("￥" + totalresult);
    $(".t-input").each(function () {
        var $this = $(this);
         
         $this.change(function () {
             var $t = $(this);
             var hidden = $this.closest('tr').find('input[type=hidden]');
             var mname = hidden.attr('mname');
             hidden.val(mname + '|' + $t.val());
//             $t.closest('table').find(".pertarget").each(function () {
//                 var totalresult += parseInt($(this).attr("v"));
//                  $t.closest('table').find('.result').text("￥" + totalresult);
//             });
         });
     });

   

}
function getSliderValue(s) {
    var v = s.attr('TargetOfWeek');
    return v;
}

function initialMemberSelect() {
    $('input[type=checkbox]').click(function () { 
    });
}
function initialCharDistribution() {
    $(".charatacter, .pane").sortable({
        connectWith: ".connectedSortable"
    });

    $(".pane").droppable({
        drop: function (event, ui) {
            var $this = $(this);
            var mid = $this.closest('.pane').attr('mid');
            ui.draggable.find('input').remove();
            var c = ui.draggable.text();
            ui.draggable.append('<input name="mc" type="hidden" value="' + mid + '|' + c + '"/>');
            $this.append(ui.draggable);
            $(".cc").addClass('xiangmu_3');
            return false;
        }
    });

    $(".charatacter").droppable({
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
            url: "/Lead/Save_LeadCall",
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
            url: "/Lead/Save_LeadPackage?leadid=" + leadid + "&projectid=" + projectid + "&packageid=" + id,
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