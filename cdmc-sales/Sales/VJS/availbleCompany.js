
var tempProjectId = undefined;
var tempCRMID = undefined;

var currentCompanyNameCN = undefined;
var currentCompanyNameEN = undefined;
    function onCompanyEdit(currentCompanyId) {
        currentCompanyNameCN = $('#currentCompanyNameCN').html().replace(/(^\s*)|(\s*$)/g, '');
        currentCompanyNameEN = $('#currentCompanyNameEN').html().replace(/(^\s*)|(\s*$)/g, '');        
        $.post('GetEditCompany', { companyId: currentCompanyId }, function (result) {
            $('.companyEdit-wrapper').html(result);
            var window = $('#EditCompany').data('tWindow');
            window.center().open();

            while ($('.dialogue-editcompany form').length > 1) {
                $('.dialogue-editcompany form').last().remove();
            }
        });
    }

    function onCompanyDetail(currentCompanyId) {
        $.post('GetDetailCompany', { companyId: currentCompanyId }, function (result) {
            $('.companyDetail-wrapper').html(result);
            var window = $('#DetailCompany').data('tWindow');
            window.center().open();

            while ($('.dialogue-detailcompany form').length > 1) {
                $('.dialogue-detailcompany form').last().remove();
            }
        });
    }

    function Editcompany() {

        var hasError = false;
        $('.dialogue-editcompany form input').removeClass('fieldError');
        $('.dialogue-editcompany form select').removeClass('fieldError');

        String.prototype.isEmpty = function () { return /^\s*$/.test(this); }

        if ($('.dialogue-editcompany form #DistrictNumberId option:selected').val().isEmpty()) {
//            if ($('.dialogue-editcompany form #ZipCode').val().isEmpty()) {
//                $('.dialogue-editcompany form #ZipCode').addClass('fieldError');
//                hasError = true;
//            }
//            if ($('.dialogue-editcompany form #Address').val().isEmpty()) {
//                $('.dialogue-editcompany form #Address').addClass('fieldError');
//                hasError = true;
//            }
            if ($('.dialogue-editcompany form #Name_CN').val().isEmpty()) {
                $('.dialogue-editcompany form #Name_CN').addClass('fieldError');
                hasError = true;
            }
        }
        else {
            if ($('.dialogue-editcompany form #Name_EN').val().isEmpty()) {
                $('.dialogue-editcompany form #Name_EN').addClass('fieldError');
                hasError = true;
            }
        }

        var telephone = $('.dialogue-editcompany form #Phone');
        if (telephone.val().isEmpty() || !IsTelephone(telephone.val())) {
            telephone.addClass('fieldError');
            hasError = true;
        }

        var fax = $('.dialogue-editcompany form #Fax');
        if ((!fax.val().isEmpty()) && (!IsTelephone(fax.val()))) {
            fax.addClass('fieldError');
            hasError = true;
        }

        if ($('.dialogue-editcompany form #IndustryId').val().isEmpty()) {
            $('.dialogue-editcompany form #IndustryId').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-editcompany form #TypeId').val().isEmpty()) {
            $('.dialogue-editcompany form #TypeId').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-editcompany form #ProgressId').val().isEmpty()) {
            $('.dialogue-editcompany form #ProgressId').addClass('fieldError');
            hasError = true;
        }      

        if ($('.dialogue-editcompany form input[type=checkbox][name=Categories]:checked').length == 0) {
            $('fieldset#categories').addClass('fieldError');
            hasError = true;
        } else {
            $('fieldset#categories').removeClass('fieldError');
        }

        if (hasError) {
            return;
        }

        var projectid = $('#CurrentProjectId').val();
        var companyNameCN = $('.dialogue-editcompany form #Name_CN').val();
        var companyNameEN = $('.dialogue-editcompany form #Name_EN').val();
        $.post('CheckCompanyExist', { projectid:projectid, beforeUpdateCN: currentCompanyNameCN, afterUpdateCN: companyNameCN, beforeUpdateEN: currentCompanyNameEN, afterUpdateEN: companyNameEN }, function (result) {
            if (result.length > 0) {
                alert(result);
            } else {
                var query = $('.dialogue-editcompany form').serializeArray();
                $.post('EditCompany', query, function (result) {
                    if (result.length > 0) {
                        alert(result);
                    } else {
                        $('#EditCompany').data('tWindow').close();
                        alert('公司更新成功');
                        var Indexes = $('.dialogue-editcompany form #CompanRelationshipId').val();
                        RefreshCrmList(Indexes);
                        onCompanyChanging(Indexes);
                    }
                });
            }
        });       
    }

    function CancelEditcompany() {
        var window = $('#EditCompany').data('tWindow');
        window.close();
    }

    var currentLeadID = undefined;
    function onLeadEdit(leadId) {
        currentLeadID = leadId;      
        $.post('GetEditLead', { leadId: leadId }, function (result) {
            $('.leadedit-wrapper').html(result);
            $('#EditLead').data('tWindow').center().open();
            while ($('.dialogue-editlead form').length > 1) {
                $('.dialogue-editlead form').last().remove();
            }
        });
    }

    function CancelEditLead() {
        var window = $('#EditLead').data('tWindow');
        window.close();
    }

    function UpdateLead() {

        var haserror = false;

        $('.dialogue-editlead form input').removeClass('fieldError');
        $('.dialogue-editlead form select').removeClass('fieldError');

        $('.dialogue-editlead form #Msg').html('');
        var msg = '';

        var $namecn = $('.dialogue-editlead form #Name_CN');
        var $nameen = $('.dialogue-editlead form #Name_EN');
        if ($namecn.val().length == 0 && $nameen.val().length == 0) {
            $namecn.addClass('fieldError');
            $nameen.addClass('fieldError');
            msg += '客户中英文名必需至少填一个.';
            haserror = true;
        }


        $gender = $('.dialogue-editlead form #Gender');
        if ($gender.val().length == 0) {
            $gender.addClass('fieldError');
             haserror = true;
         }
      
         $title = $('.dialogue-editlead form #Title');
         if ($title.val().length == 0) {
             $title.addClass('fieldError');
             haserror = true;
         }

         var telephone = $('.dialogue-editlead form #Telephone');
         var cellPhone = $('.dialogue-editlead form #CellPhone');
         if ((cellPhone.val().length == 0) && (telephone.val().length == 0)) {
             telephone.addClass('fieldError');
             cellPhone.addClass('fieldError');
             msg += '<br />客户直线移动电话必需至少填一个.';
             haserror = true;
         } else {
             if (telephone.val().length != 0 && !IsTelephone(telephone.val())) {
                 telephone.addClass('fieldError');
                 haserror = true;
             }
             if (cellPhone.val().length != 0 && !IsTelephone(cellPhone.val())) {
                 cellPhone.addClass('fieldError');
                 haserror = true;
             }
         }

         var personalPhone = $('.dialogue-editlead form #PersonalPhone ');
         if (personalPhone.val().length != 0 && !IsTelephone(personalPhone.val())) {
             personalPhone.addClass('fieldError');
             haserror = true;
         }

         var personalCellPhone = $('.dialogue-editlead form #PersonalCellPhone ');
         if (personalCellPhone.val().length != 0 && !IsTelephone(personalCellPhone.val())) {
             personalCellPhone.addClass('fieldError');
             haserror = true;
         }

         var personelEmail = $('.dialogue-editlead form #PersonelEmail');
         if (personelEmail.val().length != 0 && !validateEmail(personelEmail.val())) {
             personelEmail.addClass('fieldError');
             haserror = true;
         }

         var workingEmail = $('.dialogue-editlead form #WorkingEmail');
         if (workingEmail.val().length != 0 && !validateEmail(workingEmail.val())) {
             workingEmail.addClass('fieldError');
             haserror = true;
         }

         if (haserror) {
             if (msg != '')
                 $('.dialogue-editlead form #Msg').html(msg);
             return;
         }

        var query = $('.dialogue-editlead form').serializeArray();
        $.post('EditLead' , query, function (result) {
            if (result.length > 0) {
                alert(result);
            } else {
                $('#EditLead').data('tWindow').close();
                alert('Lead更新成功');
                selectedVal = $('#CRMID').val() + "," + $('.dialogue-editlead form #LeadId').val();
                RefreshCrmList(selectedVal);
                onCompanyChanging(selectedVal);
            }
        });
    }

    function onLeadAdd(currentCompanyId) {
        $.post('GetAddLead', { companyId: currentCompanyId }, function (result) {
            $('.leadadd-wrapper').html(result);
            var window = $('#AddLead').data('tWindow');
            window.center().open();
        });
    }

    var currentLeadCallID = undefined;
    function EditLeadCall(leadCallID) {
        currentLeadCallID = leadCallID;
        $.post('GetEditLeadCall', { leadCallId: leadCallID }, function (result) {
            $('.calledit-wrapper').html(result);
            var window = $('#EditCall').data('tWindow');
            window.center().open();
            while ($('.dialogue-editcall form').length > 1) {
                $('.dialogue-editcall form').last().remove();
            }
        });
    }
    function CancelEditLead() {
        var window = $('#EditCall').data('tWindow');
        window.close();
    }
    function onCallAdd(currentLeadId) {
        currentCompanyRelationshipId = $('#CRMID').val();
        currentProjectId = $('#ProjectID').val().replace(/(^\s*)|(\s*$)/g, '');
        if (currentProjectId.length == 0) {
            currentProjectId = null;
        }
        $.post('GetAddCall', { leadId: currentLeadId, companyRelationId: currentCompanyRelationshipId, projectId: currentProjectId }, function (result) {
            $('.calladd-wrapper').html(result);
            var window = $('#AddCall').data('tWindow');
            window.center().open();
        });
    }
    function AddCall() {
        if ($('.dialogue-addcall form #ProjectId').val() == 'null') {
            $('.dialogue-addcall form #ProjectId').val('');
        }

        if ($('.dialogue-addcall form').valid()) {
            var haserror = false;
            var $calltype = $('.dialogue-addcall form #CallTypeId');
            if ($calltype.val().length == 0) {
                $calltype.addClass('fieldError');
                haserror = true;
            }

            if (haserror) {
                return;
            }

            var query = $('.dialogue-addcall form').serializeArray();
            $.post('AddCall' , query, function (result) {
                if (result.length > 0) {
                    alert('LeadCall新增失败');
                } else {
                    $('#AddCall').data('tWindow').close();
                    alert('LeadCall新增成功');

                    var selectedVal = undefined;
                    if (tempCRMID == undefined) {
                        selectedVal = $('#CRMID').val() + "," + $('.dialogue-addcall form #LeadId').val();
                    } else {
                        selectedVal = tempCRMID + "," + $('.dialogue-addcall form #LeadId').val();
                        tempCRMID = undefined;
                        tempProjectId = undefined;
                    }
                    RefreshCrmList(selectedVal);
                    onCompanyChanging(selectedVal);
                }
            });
        }
    }

    function CancelAddCall() {
        if (tempCRMID != undefined) {
            RefreshCrmList(tempCRMID);
            onCompanyChanging(tempCRMID);
            tempCRMID = undefined;
            tempProjectId = undefined;
        }
        var window = $('#AddCall').data('tWindow');
        window.close();
    }
    function UpdateCall() {
        if ($('.calledit-wrapper form').valid()) {
            var query = $('.calledit-wrapper form').serializeArray();
            $.post('EditLeadCall' , query, function (result) {
                if (result.length > 0) {
                    alert(result);
                } else {
                    $('#' + currentLeadCallID + '_CallType').html($('.dialogue-editcall form #CallTypeId option:selected').first().text());
                    $('#' + currentLeadCallID + '_CallBackDate').html($('.dialogue-editcall form #CallBackDate').val());
                    $('#' + currentLeadCallID + '_CallDate').html($('.dialogue-editcall form #CallDate').val());
                    $('#' + currentLeadCallID + '_Result').html($('.dialogue-editcall form #Result').val());
                    $('#EditCall').data('tWindow').close();
                    alert('LeadCall更新成功');                   
                }
            });
        }
    }
    function CancelEditCall() {
        var window = $('#EditCall').data('tWindow');
        window.close();
    }
    function AddLead() {
      
            var haserror = false;

            $('.dialogue-addlead form input').removeClass('fieldError');
            $('.dialogue-addlead form select').removeClass('fieldError');
            $('.dialogue-addlead form #Msg').html('');
            var msg = '';

            var $namecn = $('.dialogue-addlead form #Name_CN');
            var $nameen = $('.dialogue-addlead form #Name_EN');
            if ($namecn.val().length == 0 && $nameen.val().length == 0) {
                $namecn.addClass('fieldError');
                $nameen.addClass('fieldError');
                msg += '客户中英文名必需至少填一个.';
                haserror = true;
            }

            $gender = $('.dialogue-addlead form #Gender');
            if ($gender.val().length == 0) {
                $gender.addClass('fieldError');
                haserror = true;
            }
          
            $title = $('.dialogue-addlead form #Title');
            if ($title.val().length == 0) {
                $title.addClass('fieldError');
                haserror = true;
            }

            var telephone = $('.dialogue-addlead form #Telephone');
            var cellPhone = $('.dialogue-addlead form #CellPhone');
            if ((cellPhone.val().length == 0) && (telephone.val().length == 0)) {
                telephone.addClass('fieldError');
                cellPhone.addClass('fieldError');
                msg += '<br />客户直线移动电话必需至少填一个.';
                haserror = true;
            } else {
                if (telephone.val().length != 0 && !IsTelephone(telephone.val())) {
                    telephone.addClass('fieldError');
                    haserror = true;
                }
                if (cellPhone.val().length != 0 && !IsTelephone(cellPhone.val())) {
                    cellPhone.addClass('fieldError');
                    haserror = true;
                }
            }

            var personalPhone = $('.dialogue-addlead form #PersonalPhone ');
            if (personalPhone.val().length != 0 && !IsTelephone(personalPhone.val())) {
                personalPhone.addClass('fieldError');
                haserror = true;
            }

            var personalCellPhone = $('.dialogue-addlead form #PersonalCellPhone ');
            if (personalCellPhone.val().length != 0 && !IsTelephone(personalCellPhone.val())) {
                personalCellPhone.addClass('fieldError');
                haserror = true;
            }

            var personelEmail = $('.dialogue-addlead form #PersonelEmail');
            if (personelEmail.val().length != 0 && !validateEmail(personelEmail.val())) {
                personelEmail.addClass('fieldError');
                haserror = true;
            }

            var workingEmail = $('.dialogue-addlead form #WorkingEmail');
            if (workingEmail.val().length != 0 && !validateEmail(workingEmail.val())) {
                workingEmail.addClass('fieldError');
                haserror = true;
            }

            if (haserror) {
                if (msg != '')
                    $('.dialogue-addlead form #Msg').html(msg);
                return;
            }
                   
            var query = $('.dialogue-addlead form').serializeArray();
            $.post('AddLead', query, function (result) {
                if (result.length > 0) {
                    $('#AddLead').data('tWindow').close();

                    if (confirm('Lead新增成功,是否新增相应Call')) {
                        if (tempCRMID != undefined) {
                            $('#CRMID').val(tempCRMID);
                        }
                        onCallAdd(result);
                    } else {
                        var selectedVal = undefined;
                        if (tempCRMID == undefined) {
                            selectedVal = $('#CRMID').val() + "," + result;
                        } else {
                            selectedVal = tempCRMID + "," + result;
                            tempCRMID = undefined;
                            tempProjectId = undefined;
                        }

                        RefreshCrmList(selectedVal);
                        onCompanyChanging(selectedVal);
                    }

                } else {
                    alert('Lead新增失败');
                }
            });
      
    }
    function CancelAddLead() {
        if (tempCRMID != undefined) {
            RefreshCrmList(tempCRMID);
            onCompanyChanging(tempCRMID);
            tempCRMID = undefined;
            tempProjectId = undefined;
        }

        var window = $('#AddLead').data('tWindow');
        window.close();
    }

    function HandleEmptySpan() {
        $('span.datainfo').each(function () {
            if ($(this).html().replace(/(^\s*)|(\s*$)/g, '').length == 0) {
                $(this).parent().hide();
            } else {
                $(this).parent().show();
            }
        });
    }

    $(function () {
        HandleEmptySpan();
    });

    function RefreshCrmList(selectedVal) {
        var f = GetFilters();
        f.selectedVal = selectedVal;
        $.post('_RefreshCrmList', f, function (result) {
            $('#mainnavigationcontainer').html(result);
            initial();
        });
    }
    function CurrentProjectChange(currentProjectId) {
        $.get('GetCatagories', { currentProjectId: currentProjectId }, function (result) {
            $('#f-category').empty();
            $("<option value=''>不指定</option>").appendTo("#f-category")
            $.each(result, function (index) {
                $("<option value='" + result[index].value + "'>" + result[index].text + "</option>").appendTo("#f-category");
            });
        });
    }

    function GetFilters() {
        var id = $('#CurrentProjectId').val();
        var companyprogress = $('#companyprogress').val();
        var companyassigndate = $('#companyassigndate').val();
        var leadprogress = $('#leadprogress').val();
        var dealprogress = $('#dealprogress').val();
        var callprogress = $('#callprogress').val();
        var categoryId = $('#f-category').val();
        var fuzzyQuery = $('#f-fuzzy').val();
        var unfold = $('#unfoldCompany').val();
        //var companyassigndate = $('#companyassigndate').val();
        return { CompanyProgress: companyprogress, CompanyAssignDate: companyassigndate, LeadProgress: leadprogress, DealProgress: dealprogress, CallProgress: callprogress, ProjectId: id, FuzzyQuery: fuzzyQuery, Unfold: unfold, CategoryId: categoryId };
    }
    function initial() {

        //分组字体标注
        $('#customView').find('>ul >li >div >span').css("fontSize", "23px").css("font-weigh", "bold").css('line-height', '28px');

        $('#customView').find('>ul >li >ul >li >div >span').css("fontSize", "15px").css("font-weigh", "bold");
        $('#navigationView').find('>ul >li >div >span').css("fontSize", "15px").css("font-weigh", "bold");

        //lead状态颜色标注
        $('#mainnavigationcontainer span').each(function () {
            var $this = $(this);
            var spantext = $this.text();
            if (spantext.indexOf(',&6*') > 0)//Call-Backed
            {
                $this.css('color', '#0000C6');
                spantext = spantext.replace(",&6*", "");
            }
            else if (spantext.indexOf(',&2*') > 0)//blowed
            {
                $this.css('color', '#000000');
                spantext = spantext.replace(",&2*", "");
            }
            else if (spantext.indexOf(',&4*') > 0)//pitch
            {
                $this.css('color', '#336666');
                spantext = spantext.replace(",&4*", "");
            }
            else if (spantext.indexOf(',&0*') > 0)//no call
            {
                $this.css('color', 'red');
                spantext = spantext.replace(",&0*", "");
            }
            else if (spantext.indexOf(',&9*') > 0)//close
            {
                $this.css('color', '#009393');
                spantext = spantext.replace(",&9*", "");
            }
            else if (spantext.indexOf(',&7*') > 0)//Waiting for Approval
            {
                $this.css('color', '#9F35FF');
                spantext = spantext.replace(",&7*", "");
            }
            else if (spantext.indexOf(',&8*') > 0)//Qualified Decision
            {
                $this.css('color', '#FF00CC');
                spantext = spantext.replace(",&8*", "");
            }
            $this.text(spantext);
        });

        //生成关联菜单子菜单
        var ul = $(".regroup ul");
        ul.children().remove();
        var customli = $('#customView >ul >li');
        customli.each(function () {
            var t = $(this).find('>div >span').text();
            var v = $(this).find('>div >input').val();
            ul.append('<li class="grouptarget">' + t + '<input type="hidden" value=' + v + '></li>');
        });

        //添加分组
        $('#group-open-button')
                .click(function (e) {
                    e.preventDefault();
                    gw().center().open();
                });


        $('#navigationView').find('>ul >li').jeegoocontext('contextmenu', {
            widthOverflowOffset: 0,
            heightOverflowOffset: 3,
            submenuLeftOffset: -4,
            submenuTopOffset: -5,
            onSelect: contextMenuSelected,
            onShow: generateContextMenu
        });

        //        $('#customView').find('>ul >li').jeegoocontext('groupcontextmenu', {
        //            widthOverflowOffset: 0,
        //            heightOverflowOffset: 3,
        //            submenuLeftOffset: -4,
        //            submenuTopOffset: -5,
        //            onSelect: groupContextMenuSelected
        //        });

        $('#customView').find('>ul >li >ul >li').jeegoocontext('favorscontextmenu', {
            widthOverflowOffset: 0,
            heightOverflowOffset: 3,
            submenuLeftOffset: -4,
            submenuTopOffset: -5,
            onSelect: contextMenuSelected,
            onShow: generateContextMenu
        });
    }

    //生成动态子菜单
    function generateContextMenu(e, context) {
        //更新context菜单公司名
        var crmtext = $(context).find('>div >span.t-in').text();
        crmtext = crmtext.split(',')[0];
        $(' .regrouptext').text("移动" + crmtext + "到");

    }

    //分组管理
    function groupContextMenuSelected(e, context) {
        var groupid = $(context).find('input').val();
        var targetid = $(e.currentTarget).attr('id');
        if (targetid == 'groupmanagement') {
            e.preventDefault();
            gw().center().open();
        }


    }

    //公司关联菜单选中/自定义组公司关联选中
    function contextMenuSelected(e, context) {
        var $target = $(e.target);

        //copy到新分组
        if ($target.hasClass('grouptarget')) {
            var crmid = $(context).find('>div >input').val();
            var groupid = $(e.currentTarget).find('input').val();
            var f = GetFilters();
            $.post('_CopyCrmToGroup', { crmid: crmid, groupid: groupid, filters: f }, function (result) {
                $('#mainnavigationcontainer').html(result);
                initial();
            });
        }

        //释放回公司池
        if ($target.hasClass('blowed')) {
            var crmid = $(context).find('>div >input').val();
            var f = GetFilters();
            $.post('_CrmBlowed', { crmid: crmid, filters: f }, function (result) {
                $('#mainnavigationcontainer').html(result);
                initial();
            });
        }


        //释放回公司池
        if ($target.hasClass('remove')) {
            var crmid = $(context).find('>div >input').val();
            var groupid = $(e.currentTarget).find('input').val();
            $.post('_CrmRemove', { crmid: crmid, filters: GetFilters() }, function (result) {
                $('#mainnavigationcontainer').html(result);
                initial();
            });
        }
    }

    //获取主树
    function nv() {
        return $('#navigationView').data('tTreeView');
    }

    //获取分组窗口
    function gw() {
        return $('#groupListWindow').data('tWindow');
    }
    //提交分组更改
    function onGroupListWindowSubmit(e) {
        $.post('_RefreshCrmList', null, function (result) {
            $('#mainnavigationcontainer').html(result);
            initial();
        });
    }

    //var count=0
    //公司选择 
    function onIndexSelect(e) {
        var indexs = nv().getItemValue(e.item);
        if (e.item.innerHTML.indexOf("分组") < 0) {
            $.post('_SelectedList', { indexs: indexs, filters: GetFilters() }, function (result) {
                $('#crmdetailscontainer').html(result);
                //count++;

                //$('#crmdetailscontainer').html('<p>' + count + '</p>');
            });
        }
    }

    function onCompanyChanging(indexs) {
        $.post('_SelectedList', { indexs: indexs, filters: GetFilters() }, function (result) {
            $('#crmdetailscontainer').html(result);
        });
    }

    //展开所有leads
    function expandRow(grid, row) {
        grid.expandRow(row);
    }

    function onRowDataBound(e) {
        var grid = $(this).data('tGrid');
        expandRow(grid, e.row);
    }

    function resetquery() {
        $('#companyprogress').val($('#companyprogress option').get(0).value);
//        $('#companyassigndate').val($('#companyassigndate option').get(0).value);
        $('#leadprogress').val($('#leadprogress option').get(0).value);
        $('#dealprogress').val($('#dealprogress option').get(0).value);
        $('#callprogress').val($('#callprogress option').get(0).value);
        $('#f-category').val($('#f-category option').get(0).value);
        $('#f-fuzzy').val("");
        $('#unfoldCompany').val($('#unfoldCompany option').get(0).value);
    }

    function GetAddCompany() {
        var currentProjectId = $('#CurrentProjectId').val();
        $.post('GetAddCompany', { projectId: currentProjectId }, function (result) {
            $('.companyAdd-wrapper').html(result);
            var window = $('#AddCompany').data('tWindow');
            window.center().open();
        });
    }
    function Addcompany() {
        var hasError = false;
        $('.dialogue-addcompany form input').removeClass('fieldError');
        $('.dialogue-addcompany form select').removeClass('fieldError');



        String.prototype.isEmpty = function () { return /^\s*$/.test(this); }

        if ($('.dialogue-addcompany form #DistrictNumberId option:selected').val().isEmpty()) {
//            if ($('.dialogue-addcompany form #ZipCode').val().isEmpty()) {
//                $('.dialogue-addcompany form #ZipCode').addClass('fieldError');
//                hasError = true;
//            }
//            if ($('.dialogue-addcompany form #Address').val().isEmpty()) {
//                $('.dialogue-addcompany form #Address').addClass('fieldError');
//                hasError = true;
//            }
            if ($('.dialogue-addcompany form #Name_CN').val().isEmpty()) {
                $('.dialogue-addcompany form #Name_CN').addClass('fieldError');
                hasError = true;
            }
        }
        else {
            if ($('.dialogue-addcompany form #Name_EN').val().isEmpty()) {
                $('.dialogue-addcompany form #Name_EN').addClass('fieldError');
                hasError = true;
            }
        }

        var telephone=$('.dialogue-addcompany form #Phone');
        if (telephone.val().isEmpty() || !IsTelephone(telephone.val())) {
            telephone.addClass('fieldError');
            hasError = true;
        }

        var fax = $('.dialogue-addcompany form #Fax');
        if ((!fax.val().isEmpty()) && (!IsTelephone(fax.val()))) {
            fax.addClass('fieldError');
            hasError = true;
        }
//            if ($('.dialogue-addcompany form #Fax').val().isEmpty()) {
//                $('.dialogue-addcompany form #Fax').addClass('fieldError');
//                hasError = true;
//            }
//        if ($('.dialogue-addcompany form #WebSite').val().isEmpty()) {
//            $('.dialogue-addcompany form #WebSite').addClass('fieldError');
//            hasError = true;
//        }
        if ($('.dialogue-addcompany form #IndustryId').val().isEmpty()) {
            $('.dialogue-addcompany form #IndustryId').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-addcompany form #TypeId').val().isEmpty()) {
            $('.dialogue-addcompany form #TypeId').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-addcompany form #ProgressId').val().isEmpty()) {
            $('.dialogue-addcompany form #ProgressId').addClass('fieldError');
            hasError = true;
        }      
    
   
        if ($('.dialogue-addcompany form input[type=checkbox][name=Categories]:checked').length == 0) {
            $('fieldset#categories').addClass('fieldError');
            hasError = true;
        } else {
            $('fieldset#categories').removeClass('fieldError');
        }

        if (hasError) {
            return;
        }
        var projectid = $('#CurrentProjectId').val();
        var companyNameCN = $('.dialogue-addcompany #Name_CN').val();
        var companyNameEN = $('.dialogue-addcompany form #Name_EN').val();
        $.post('CheckCompanyExist', { projectid:projectid,beforeUpdateCN: null, afterUpdateCN: companyNameCN, beforeUpdateEN: null, afterUpdateEN: companyNameEN }, function (result) {
            if (result.length > 0) {
                alert(result);
            } else {
                var query = $('.dialogue-addcompany form').serializeArray();
                $.post('AddCompany', query, function (result) {
                    if (result.companyRelationshipId != null) {
                        $('#AddCompany').data('tWindow').close();
                        if (confirm('公司新增成功,是否要往新增公司里面增加Lead')) {
                            tempProjectId = result.projectId;
                            tempCRMID = result.companyRelationshipId;

                            onLeadAdd(result.companyId);
                        }
                        else {
                            RefreshCrmList(result.companyRelationshipId);
                            onCompanyChanging(result.companyRelationshipId);
                        }
                    } else {
                        alert('公司新增失败');
                    }
                });
            }
        });
    }

    function CancelAddcompany() {
        var window = $('#AddCompany').data('tWindow');
        window.close();
    }

    function GetQuickEntry() {
        var currentProjectId = $('#CurrentProjectId').val();
        $.post('GetQuickEntry', { projectId: currentProjectId }, function (result) {
            $('.quickEntry-wrapper').html(result);
            var window = $('#QuickEntry').data('tWindow');
            window.center().open();
        });
    }

    function QuickEntry() {
        var hasError = false;
        $('.dialogue-addcompany form input').removeClass('fieldError');
        $('.dialogue-addcompany form select').removeClass('fieldError');
        $('.dialogue-addcompany form #Msg').html('');

        String.prototype.isEmpty = function () { return /^\s*$/.test(this); }

        var msg = '';

        // validation for company
        if ($('.dialogue-addcompany form #DistrictNumberId option:selected').val().isEmpty()) {
            if ($('.dialogue-addcompany form #Name_CN').val().isEmpty()) {
                $('.dialogue-addcompany form #Name_CN').addClass('fieldError');
                hasError = true;
            }
        }
        else {
            if ($('.dialogue-addcompany form #Name_EN').val().isEmpty()) {
                $('.dialogue-addcompany form #Name_EN').addClass('fieldError');
                hasError = true;
            }
        }

        var telephone = $('.dialogue-addcompany form #Phone');
        if (telephone.val().isEmpty() || !IsTelephone(telephone.val())) {
            telephone.addClass('fieldError');
            hasError = true;
        }

        if ($('.dialogue-addcompany form #IndustryId').val().isEmpty()) {
            $('.dialogue-addcompany form #IndustryId').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-addcompany form #TypeId').val().isEmpty()) {
            $('.dialogue-addcompany form #TypeId').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-addcompany form #ProgressId').val().isEmpty()) {
            $('.dialogue-addcompany form #ProgressId').addClass('fieldError');
            hasError = true;
        }

        if ($('.dialogue-addcompany form input[type=checkbox][name=Categories]:checked').length == 0) {
            $('fieldset#categories').addClass('fieldError');
            hasError = true;
        } else {
            $('fieldset#categories').removeClass('fieldError');
        }
        // end validation for company

        // validation for lead
        var $namecn = $('.dialogue-addcompany form #LeadName_CN');
        var $nameen = $('.dialogue-addcompany form #LeadName_EN');
        if ($namecn.val().length == 0 && $nameen.val().length == 0) {
            $namecn.addClass('fieldError');
            $nameen.addClass('fieldError');
            msg += '客户信息中客户中英文名必需至少填一个.';
            hasError = true;
        }

        $gender = $('.dialogue-addcompany form #Gender');
        if ($gender.val().length == 0) {
            $gender.addClass('fieldError');
            hasError = true;
        }

        $title = $('.dialogue-addcompany form #Title');
        if ($title.val().length == 0) {
            $title.addClass('fieldError');
            hasError = true;
        }

        var telephone = $('.dialogue-addcompany form #Telephone');
        var cellPhone = $('.dialogue-addcompany form #CellPhone');
        if ((cellPhone.val().length == 0) && (telephone.val().length == 0)) {
            msg += '<br />客户信息中客户直线移动电话必需至少填一个.';
            telephone.addClass('fieldError');
            cellPhone.addClass('fieldError');
            hasError = true;
        } else {
            if (telephone.val().length != 0 && !IsTelephone(telephone.val())) {
                telephone.addClass('fieldError');
                hasError = true;
            }
            if (cellPhone.val().length != 0 && !IsTelephone(cellPhone.val())) {
                cellPhone.addClass('fieldError');
                hasError = true;
            }
        }
        // end validation for lead

        // validation for leadcall

        var callFirst = $('.dialogue-addcompany form #CallDate');
        var callBack = $('.dialogue-addcompany form #CallBackDate');
        if (callBack.val().length != 0 && callFirst.val().length != 0) {
            if (Date.parse(callFirst.val()) >= Date.parse(callBack.val())) {
                msg += '<br />通话信息中回打时间不能早于致电时间.';
                callFirst.addClass('fieldError');
                callBack.addClass('fieldError');
                hasError = true;
            }
        }

        var $calltype = $('.dialogue-addcompany form #CallTypeId');
        if ($calltype.val().length == 0) {
            $calltype.addClass('fieldError');
            hasError = true;
        }

        // end validation for leadcall

        if (hasError) {
            if (msg != '')
                $('.dialogue-addcompany form #Msg').html(msg);
            return;
        } else {
            var projectid = $('#CurrentProjectId').val();
            var companyNameCN = $('.dialogue-addcompany form #Name_CN').val();
            var companyNameEN = $('.dialogue-addcompany form #Name_EN').val();
            $.post('CheckCompanyExist', { projectid: projectid, beforeUpdateCN: null, afterUpdateCN: companyNameCN, beforeUpdateEN: null, afterUpdateEN: companyNameEN }, function (result) {
                if (result.length > 0) {
                    alert(result);
                } else {
                    var query = $('.dialogue-addcompany form').serializeArray();
                    $.post('QuickEntry', query, function (result) {
                        if ((result.companyRelationshipId != null) && (result.leadId != null) && (result.leadCallId != null)) {
                            $('#QuickEntry').data('tWindow').close();
                            RefreshCrmList(result.companyRelationshipId);
                            onCompanyChanging(result.companyRelationshipId);
                        } else {
                            alert('快捷录入失败');
                        }
                    });
                }
            });
        }
    }

    function CancelQuickEntry() {
        var window = $('#QuickEntry').data('tWindow');
        window.close();
    }

    function onQuickAddDeal(currentProjectId, currentCRMId) {
        $.post('GetQuickAddDeal', { projectId: currentProjectId, CRMId: currentCRMId }, function (result) {
            $('.quickdeal-wrapper').html(result);
            var window = $('#QuickAddDeal').data('tWindow');
            window.center().open();
        });
    }

    function QuickDeal() {
        var hasError = false;
        $('.dialogue-quickdeal form input').removeClass('fieldError');
        $('.dialogue-quickdeal form select').removeClass('fieldError');

        String.prototype.isEmpty = function () { return /^\s*$/.test(this); }

        // validation for deal
        if ($('.dialogue-quickdeal form #PackageID').val().isEmpty()) {
            $('.dialogue-quickdeal form #PackageID').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-quickdeal form #CompanyRelationshipID').val().isEmpty()) {
            $('.dialogue-quickdeal form #CompanyRelationshipID').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-quickdeal form #ExpectedPaymentDate').val().isEmpty()) {
            $('.dialogue-quickdeal form #ExpectedPaymentDate').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-quickdeal form #Payment').val().isEmpty() || $('.dialogue-quickdeal form #Payment').val() <= 0) {
            $('.dialogue-quickdeal form #Payment').addClass('fieldError');
            hasError = true;
        }
        if ($('.dialogue-quickdeal form #Committer').val().isEmpty()) {
            $('.dialogue-quickdeal form #Committer').addClass('fieldError');
            hasError = true;
        }
        var contact = $('.dialogue-quickdeal form #CommitterContect');
        if (contact.val().length != 0 && !IsTelephone(contact.val())) {
            contact.addClass('fieldError');
            hasError = true;
        }
        var commiterEmail = $('.dialogue-quickdeal form #CommitterEmail');
        if (commiterEmail.val().isEmpty()) {
            commiterEmail.addClass('fieldError');
            hasError = true;
        } else {
            if (!validateEmail(commiterEmail.val())) {
                commiterEmail.addClass('fieldError');
                hasError = true;
            }
        }
        if ($('.dialogue-quickdeal form #PaymentDetail').val().isEmpty()) {
            $('.dialogue-quickdeal form #PaymentDetail').addClass('fieldError');
            hasError = true;
        }
        // end validation for deal

        // validation for participant
        //if ($('.dialogue-quickdeal form #Name').val().isEmpty()) {
        //    $('.dialogue-quickdeal form #Name').addClass('fieldError');
        //    hasError = true;
        //}
        //if ($('.dialogue-quickdeal form #ParticipantTypeID').val().isEmpty()) {
        //    $('.dialogue-quickdeal form #ParticipantTypeID').addClass('fieldError');
        //    hasError = true;
        //}
        // end validation for participant
        
        if (hasError) {
            return;
        }

        if ($('.quickdeal-wrapper form').valid()) {
            var query = $('.quickdeal-wrapper form').serializeArray();

            var grid = $('.quickdeal-wrapper form #pList').data('tGrid');
            var data = grid.data;
            if (data.length <= 0) {
                if (confirm("您尚未填写参会人信息，请确认是否要提交？")) {
                    $.post('QuickAddDeal', query, function (result) {
                        if ((result.dealId != null) && (result.companyRelationshipId != null) && (result.dealCode != null)) {
                            alert("您已经顺利提交出单，出单号为:" + result.dealCode)
                            $('#QuickAddDeal').data('tWindow').close();
                            RefreshCrmList(result.companyRelationshipId);
                            onCompanyChanging(result.companyRelationshipId);
                        } else {
                            alert('快捷出单失败')
                        }
                    });
                } else {
                    return;
                }
            } else {
                $.post('QuickAddDeal', query, function (result) {
                    if ((result.dealId != null) && (result.companyRelationshipId != null) && (result.dealCode != null)) {
                        alert("您已经顺利提交出单，出单号为:" + result.dealCode)
                        $('#QuickAddDeal').data('tWindow').close();
                        RefreshCrmList(result.companyRelationshipId);
                        onCompanyChanging(result.companyRelationshipId);
                    } else {
                        alert('快捷出单失败')
                    }
                });
            }
        }
    }

    function CancelQuickDeal() {
        var window = $('#QuickAddDeal').data('tWindow');
        window.close();
    }

    function GetBulkEntry() {
        var currentProjectId = $('#CurrentProjectId').val();
        $.post('GetBulkEntry', { projectId: currentProjectId }, function (result) {
            $('.bulkEntry-wrapper').html(result);
            var window = $('#BulkEntry').data('tWindow');
            window.center().open();
        });
    }

    function BulkEntry() {
        var hasError = false;
        $('.dialogue-addcompany form input').removeClass('fieldError');
        $('.dialogue-addcompany form select').removeClass('fieldError');
        $('.dialogue-addcompany form #Msg').html('');

        String.prototype.isEmpty = function () { return /^\s*$/.test(this); }

        // validation for company
        if ($('.dialogue-addcompany form #DistrictNumberId option:selected').val().isEmpty()) {
            if ($('.dialogue-addcompany form #Name_CN').val().isEmpty()) {
                $('.dialogue-addcompany form #Name_CN').addClass('fieldError');
                hasError = true;
            }
        }
        else {
            if ($('.dialogue-addcompany form #Name_EN').val().isEmpty()) {
                $('.dialogue-addcompany form #Name_EN').addClass('fieldError');
                hasError = true;
            }
        }

        var telephone = $('.dialogue-addcompany form #Phone');
        if (telephone.val().isEmpty() || !IsTelephone(telephone.val())) {
            telephone.addClass('fieldError');
            hasError = true;
        } else {
            telephone.removeClass('fieldError');
        }

        if ($('.dialogue-addcompany form #IndustryId').val().isEmpty()) {
            $('.dialogue-addcompany form #IndustryId').addClass('fieldError');
            hasError = true;
        } else {
            $('.dialogue-addcompany form #IndustryId').removeClass('fieldError');
        }

        if ($('.dialogue-addcompany form #TypeId').val().isEmpty()) {
            $('.dialogue-addcompany form #TypeId').addClass('fieldError');
            hasError = true;
        } else {
            $('.dialogue-addcompany form #TypeId').removeClass('fieldError');
        }

        if ($('.dialogue-addcompany form #ProgressId').val().isEmpty()) {
            $('.dialogue-addcompany form #ProgressId').addClass('fieldError');
            hasError = true;
        } else {
            $('.dialogue-addcompany form #ProgressId').removeClass('fieldError');
        }

        if ($('.dialogue-addcompany form input[type=checkbox][name=Categories]:checked').length == 0) {
            $('fieldset#categories').addClass('fieldError');
            hasError = true;
        } else {
            $('fieldset#categories').removeClass('fieldError');
        }
        // end validation for company
        for (var i = 0; i < 10; i++) {
            $(".leadsTable tbody tr#row_" + i).each(function () {
                var namecn = $(this).find("input[name='LeadName_CN']");
                var nameen = $(this).find("input[name='LeadName_EN']")
                var gender = $(this).find("select[name='Gender']");
                var title = $(this).find("input[name='Title']");
                var telephone = $(this).find("input[name='Telephone']");
                var cellphone = $(this).find("input[name='CellPhone']");

                if ((namecn.val().length == 0) && (nameen.val().length == 0)
                    && (title.val().length == 0) && (telephone.val().length == 0)
                    && (cellphone.val().length == 0)) {
                } else {
                    if (namecn.val().length == 0 && nameen.val().length == 0) {
                        namecn.addClass('fieldError');
                        nameen.addClass('fieldError');
                        hasError = true;
                    }
                    if (gender.val().length == 0) {
                        gender.addClass('fieldError');
                        hasError = true;
                    }
                    if (title.val().length == 0) {
                        title.addClass('fieldError');
                        hasError = true;
                    }
                    if ((cellphone.val().length == 0) && (telephone.val().length == 0)) {
                        telephone.addClass('fieldError');
                        cellphone.addClass('fieldError');
                        hasError = true;
                    } else {
                        if (telephone.val().length != 0 && !IsTelephone(telephone.val())) {
                            telephone.addClass('fieldError');
                            hasError = true;
                        }
                        if (cellphone.val().length != 0 && !IsTelephone(cellphone.val())) {
                            cellphone.addClass('fieldError');
                            hasError = true;
                        }
                    } 
                }
            });
        }
        if (hasError) {
            return;
        } else {
            var projectid = $('#CurrentProjectId').val();
            var companyNameCN = $('.dialogue-addcompany form #Name_CN').val();
            var companyNameEN = $('.dialogue-addcompany form #Name_EN').val();
            $.post('CheckCompanyExist', { projectid: projectid, beforeUpdateCN: null, afterUpdateCN: companyNameCN, beforeUpdateEN: null, afterUpdateEN: companyNameEN }, function (result) {
                if (result.length > 0) {
                    alert(result);
                } else {
                    var query = $('.dialogue-addcompany form').serializeArray();
                    $.post('BulkEntry', query, function (result) {
                        if (result.companyRelationshipId != null) {
                            $('#BulkEntry').data('tWindow').close();
                            RefreshCrmList(result.companyRelationshipId);
                            onCompanyChanging(result.companyRelationshipId);
                        } else {
                            alert('批量录入失败');
                        }
                    });
                }
            });
        }
    }

    function CancelBulkEntry() {
        var window = $('#BulkEntry').data('tWindow');
        window.close();
    }
