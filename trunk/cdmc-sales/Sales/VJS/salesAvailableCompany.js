function onAvailiableCompanyPageLoad(e) {
    addDatabindings();
    onCRMsUpdate();
    onInitialSearch();
    onSalesInputInitial();
}
function onInitialSearch() {



    $('#goSearch').unbind('click').bind('click', function (e) {
        var projectid = $('#ProjectID').val();
        var condition = $('#searchCondition').val();
        var sort = $('#Sort').val();
        var url = "/sales/JsonGetCompanys";
        
        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            data: { ProjectID: projectid, Condition: condition,Sort:sort },
            success: function (result) {
                $('#crmcontainer').html(result);
                initialBtns();
            },
            error: function (xhr, status) {
                alert(xhr.responseText);
            }
        });
    });
    
}
function onCRMsUpdate() { 
  var projectid = $('#ProjectID').val();
    var url = "/sales/JsonGetCompanys";
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'html',
        contentType: 'application/json; charset=utf-8',
        data: { ProjectID: projectid },
        success: function (result) {
            $('#crmcontainer').html(result);
            $('#companydata').html('<p></p>');
         
        },
         error: function (xhr, status) {
                alert(xhr.responseText);
            }
    });
}

function addDatabindings() {
    var tWindow = $('#salesdatawindow');
    var btns = $('.insertdata');

    btns.each(function () {
        var $this = $(this);
        $this.unbind('click').bind('click', function (e) {
            $('#FreshArea').val('companys');  
            $('#crmid').val();//设crmid为空
            if ($this.attr('id') == 'addData') {
                tWindow.find('#submittype').val('company&lead&leadcall');
                tWindow.find('fieldset').show();
            }
            if ($this.attr('id') == 'addcompanyleadData') {
                tWindow.find('#submittype').val('company&lead');

                tWindow.find('fieldset').show();
                tWindow.find('#fieldsetleadcall').hide();
            }
            if ($this.attr('id') == 'addcompanyData') {
                tWindow.find('#submittype').val('company');
                tWindow.find('fieldset').hide();
                tWindow.find('#fieldsetcompany').show();
            }

            var $win = tWindow.data('tWindow');
            tWindow.find(".t-window-content").css('height', '');
            $win.center();
            $win.open();
            tWindow.find('table').addClass("needsubmit");


        }).toggle(!tWindow.is(':visible'));


    });
   
}
//初始化公司级别以下的保存按钮
//edit company save
function initialBtns() {


    $('#companysubmit').unbind('click').bind('click', function (e) {
        $(this).attr('disabled', "true");
        var $cf = $('#companydata');
        var c = getCompanyForm($cf);
        c = getCommonData($cf, c);
        data = JSON.stringify(c);
        var crmid = $('#CRMID').val();
        var pid = $('#ProjectID').val();
        var cid = c.ID;
        var categoryarray = new Array();

        $('#categorycontainer').find("input[type='checkbox']").each(function () {
            if ($(this).is(":checked")) {
                var cid = $(this).attr('name');
                categoryarray.push(cid);
            }
        });

        var progressid = $('#ProgressID').val();

        var data = { Company: c, CRMID: crmid, Categorys: categoryarray, ProjectID: pid, ProgressID: progressid };
        data = JSON.stringify(data);
        $.ajax({
            url: "/sales/JsonSaveCompany",
            type: 'POST',
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            data: data,
            success: function (result) {
                alert('保存成功');
                freshCompany();
                $('#companysubmit').removeAttr('disabled');
            },
            error: function (xhr, status) {
                alert(xhr.responseText);
                $('#companysubmit').removeAttr('disabled');
            }
        });
    });

    $('.leadsubmit').each(function () {
        IntialLeadSavebtn($(this));
    });
}

function getCompanyForm($cf) {
    var name_EN = $cf.find("#Name_EN").val();
    var name_CH = $cf.find("#Name_CH").val();
    var contact = $cf.find("#Contact").val();
    var fax = $cf.find("#Fax").val();
    var address = $cf.find("#Address").val();
    var districtNumberID = $cf.find("#DistrictNumberID").val();
    var companyTypeID = $cf.find("#CompanyTypeID").val();
    var areaID = $cf.find("#AreaID").val();

    var districtNumberID = $cf.find("#DistrictNumberID").val();
    var business = $cf.find("#Business").val();
    var website = $cf.find("#Website").val();
    var zip = $cf.find("#ZIP").val();

//    var foreignAssetPercentage = $cf.find("#ForeignAssetPercentage").val();
    var id = $cf.find("#ID").val();

    var c = { Name_EN: name_EN,
        Name_CH: name_CH,
        Fax: fax,
        Address: address,
        DistrictNumberID: districtNumberID,
        CompanyTypeID: companyTypeID,
        ID: id,
        Contact: contact,
        ZIP: zip,
        WebSite: website,
        Business:business,
//        ForeignAssetPercentage: foreignAssetPercentage,
        AreaID: areaID
    }
        return c;
}

function getLeadForm($cf) {
    var name_EN = $cf.find("#Name_EN").val();
    var name_CH = $cf.find("#Name_CH").val();
    var contact = $cf.find("#Contact").val();
    var fax = $cf.find("#Fax").val();
    var title = $cf.find("#Title").val();
    var personalEmailAddress = $cf.find("#PersonalEmailAddress").val();
    var mobile = $cf.find("#Mobile").val();
    var gender = $cf.find("#Gender").val();
    var eMail = $cf.find("#EMail").val();
    var birthday = $cf.find("#Birthday").val();
    var address = $cf.find("#Address").val();
    var zip = $cf.find("#ZIP").val();
    var department = $cf.find("#Department").val();
    var id = $cf.find("#ID").val();
    var companyid = $cf.find("#CompanyID").val();
    var subcompanyid = $cf.find("#SubCompanyID").val();
    var c = { Name_EN: name_EN,
        Name_CH: name_CH,
        Fax: fax,
        Title: title,
        PersonalEmailAddress: personalEmailAddress,
        Mobile: mobile,
        ID: id,
        Contact: contact,
        Gender: gender,
        EMail: eMail,
        CompanyID: companyid,
        ZIP:zip,
        Birthday: birthday,
        Department: department,
        Address: address,
        SubCompanyID: subcompanyid
    };

    return c;
}
function getCallForm($cf) {
    var id = $cf.find("#ID").val();
    var leadID = $cf.find("#LeadID").val();
    var memberID = $cf.find("#MemberID").val();
    var leadCallTypeID = $cf.find("#LeadCallTypeID").val();
    var companyRelationshipID = $cf.find("#CompanyRelationshipID").val();
    var projectID = $cf.find("#ProjectID").val();
    var callBackDate = $cf.find("#CallBackDate").val();
    var callDate = $cf.find("#CallDate").val();
    var result = $cf.find("#Result").val();

    var c = { LeadID: leadID,
        MemberID: memberID,
        CompanyRelationshipID: companyRelationshipID,
        ProjectID: projectID,
        CallBackDate: callBackDate,
        CallDate: callDate,
        ID: id,
        Result: result,
        LeadID: leadID,
        LeadCallTypeID: leadCallTypeID
    };

    return c;
}

//save edit lead
function IntialLeadSavebtn($btn) {
    var $cf = $btn.closest('.leaddatacontainer');
    $btn.unbind('click').bind('click', function (e) {

        var c = getLeadForm($cf);

        c = getCommonData($cf, c);
        var cid = c.CompanyID;
        var crmid = $('#CRMID').val();
        var pid = $('#ProjectID').val();

        c = JSON.stringify(c);
        $.ajax({
            url: "/sales/JsonSaveLead",
            type: 'POST',
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            data: c,
            success: function (result) {
                alert('保存成功');
                freshCompany();
                //var lsave = $cf.find('.leadsubmit');
                //IntialLeadSavebtn(lsave);
            },
            error: function (xhr, status) {
                alert(xhr.responseText);
            }
        });
    });
}

function IntialLeadCallsSavebtn($btn) {
    var $cf = $btn.closest('.singleleadcallcontainer');
    $btn.unbind('click').bind('click', function (e) {

        var c = getCallForm($cf);
        c = getCommonData($cf, c);

        c = JSON.stringify(c);
        $.ajax({
            url: "/sales/JsonSaveLeadCall",
            type: 'POST',
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            data: c,
            success: function (result) {
                alert('保存成功');
                freshCompany();
                //$cf.html(result);
                //                var lsave = $cf.find('.leadcallssubmit');
                //                IntialLeadCallsSavebtn(lsave);
            },
            error: function (xhr, status) {
                alert(xhr.responseText);
            }
        });
    });
}

function onCompanysTreeviewNodeSelected(e) {

    var ids = $(e.item).find(".t-input").val().split('||');
    var id = ids[1];
    var pid  = $('#ProjectID').val()
    $('#CompanyID').val(id);
    var compamyrelationshipid = ids[0];
    $('#CRMID').val(compamyrelationshipid);
    $('#CompanyRelationshipID').val(compamyrelationshipid);
    freshCompany();
    
}

function freshCompany() {
        var crmid = $('#CRMID').val();
        var projectid = $('#ProjectID').val();
        var companyid = $('#CompanyID').val();

    $.ajax({
        url: '/sales/JsonCompanyInfo/',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        data: { companyid: companyid, crmid: crmid, projectid: projectid },
        success: function (result) {
            // Display the section contents.

            $('#companydata').html(result);
            initialBtns();
            intialAddLeadAndCallToExistBtn();
            intialAddLeadOnlyToExistBtn();
        },
        error: function (xhr, status) {
            alert(xhr.responseText);
        }
    });
}

//初始化+Lead & Call Button
function intialAddLeadAndCallToExistBtn() {
    var tWindow = $('#salesdatawindow');


    $('#companydata').find('.addleadandcalltoexist').unbind('click').bind('click', function (e) {
        var companyid = $(this).attr('companyid')
        $('#FreshArea').val('company&' + companyid);
        tWindow.find('#CompanyID').val(companyid);
        tWindow.find('#submittype').val('lead&leadcall');
        tWindow.find('fieldset').hide();
        tWindow.find('#fieldsetleadcall').show();
        tWindow.find('#fieldsetlead').show();

        var $win = tWindow.data('tWindow');
        tWindow.find(".t-window-content").css('height', '');
        $win.center();
        $win.open();
        tWindow.find('table').addClass("needsubmit");
    }).toggle(!tWindow.is(':visible'));
}

//初始化+Lead only Button
function intialAddLeadOnlyToExistBtn() {
    var tWindow = $('#salesdatawindow');
    var companyid = $('#CompanyID').val();
    $('#FreshArea').val('leads&' + companyid);
    $('#companydata').find('.addleadonlytoexist').unbind('click').bind('click', function (e) {
        tWindow.find('#CompanyID').val(companyid);
        tWindow.find('#submittype').val('lead');
        tWindow.find('fieldset').hide();
        tWindow.find('#fieldsetlead').show();

        var $win = tWindow.data('tWindow');
        tWindow.find(".t-window-content").css('height', '');
        $win.center();
        $win.open();
        tWindow.find('table').addClass("needsubmit");
    }).toggle(!tWindow.is(':visible'));
}

//初始化+call only Button
function intialAddCallOnlyToExistBtn() {
    var tWindow = $('#salesdatawindow');
    $('#FreshArea').val('calls');
    $('#companydata').find('.addcallonlytoexist').unbind('click').bind('click', function (e) {
        var leadid = $(this).attr('leadid')
        $('#FreshArea').val('calls&' + leadid);
        tWindow.find('#LeadID').val(leadid);
        //tWindow.find('#CRMID').val();
        tWindow.find('#submittype').val('leadcall');
        tWindow.find('fieldset').hide();
        tWindow.find('#fieldsetleadcall').show();

        var $win = tWindow.data('tWindow');
        tWindow.find(".t-window-content").css('height', '');
        $win.center();
        $win.open();
        tWindow.find('table').addClass("needsubmit");
    }).toggle(!tWindow.is(':visible'));
}

//lead展开
function onLeadExpended(e) {
    var $container = $(e.item).find("#leadCallscontainer");
    var id = $(e.item).find("input[type=hidden]").val();
    var projectid = $('#ProjectID').val();
    $.ajax({
        url: '/sales/JsonLeadCalls/',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        data: { LeadID: id, ProjectID: projectid },
        success: function (result) {
            // Display the section contents.
            $(e.item).find('#leadCallscontainer').html(result);
            intialAddCallOnlyToExistBtn();
            $(e.item).find('#leadCallscontainer').find('.leadcallssubmit').each(function () {
                IntialLeadCallsSavebtn($(this));
            });
        },
        error: function (xhr, status) {
            alert(xhr.responseText);
        }
    });
}



function getCommonData($source, c) {
    var sequence = $source.find("#Sequence").val();
    var description = $source.find("#Description").val();
    var creator = $source.find("#Creator").val();
    var createdDate = $source.find("#CreatedDate").val();
    var modifiedUser = $source.find("#ModifiedUser").val();
    var modifiedDate = $source.find("#ModifiedDate").val();
    c.Sequence = sequence;
    c.Description = description;
    c.Creator = creator;
    c.CreatedDate = createdDate;
    c.ModifiedUser = modifiedUser;
    c.ModifiedDate = modifiedDate;
    return c;
}

function onSalesInputInitial() {
    var $submit = $('#formsubmit');
    var $cancel = $('#formcancel');
    var tWindow = $('#salesdatawindow');

    var pid = $('#ProjectID').val();
    var crid = $('#CRMID').val();
    var compnayid = $('#CompanyID').val();
    var $crm = tWindow.find('#CompanyRelationshipID');
    $crm.val(crid);
    $submit.unbind('click').bind('click', function (e) {

        $submit.attr('disabled', "true");
        $cancel.attr('disabled', "true");

        var data = {};
        tWindow.find('.needsubmit').each(function () {
            var $this = $(this);
            if ($this.attr('id') == 'formcompany') {
                var company = getCompanyForm($this);
                data.Company = company;
            }
            if ($this.attr('id') == 'formlead') {
                var lead = getLeadForm($this);
                
                data.Lead = lead;
            }
            if ($this.attr('id') == 'formcall') {
                var call = getCallForm($this);
                //call.CompanyRelationshipID = crmid; //为添加情况时。把全局的crmid设到到lead
                call.ProjectID = pid; //为添加情况时。把全局的projectid设到到lead
                data.LeadCall = call;
            }
        });
       


        data.SubmitType = tWindow.find('#submittype').val();
        data.ProjectID = pid;
        data = JSON.stringify(data);
        $.ajax({
            url: '/sales/jsonaddsalesinputdata/',
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            data: data,
            dataType: 'html',
            success: function (result) {
                if (result.indexOf("has_msg") < 0) {
                    var area = $('#FreshArea').val().split('&');
                    $('#salesinputwindowcontainer').html(result);
                    //if (area[0] == 'companys') {
                        onCRMsUpdate();
                    //}
                    
                    freshCompany();
                    onSalesInputInitial();

                    $submit.removeAttr("disabled");
                    tWindow.data('tWindow').close();
                }
                else {
                    $submit.removeAttr("disabled");
                    $cancel.removeAttr("disabled");
                    $('#salesinputwindowcontainer').find('#inputmessage').html(result);
                }

            },
            error: function (xhr, status) {
                alert(xhr.responseText);
                tWindow.data('tWindow').close();
            }
        });
    });
    $cancel.unbind('click').bind('click', function (e) {
        tWindow.data('tWindow').close();
        $.ajax({
            url: '/sales/jsoncancelinput/',
            contentType: 'application/html; charset=utf-8',
            type: 'GET',
            dataType: 'html',
            success: function (result) {
                $('#salesinputwindowcontainer').html(result);
                onSalesInputInitial();

            },
            error: function (xhr, status) {
                alert(xhr.responseText);
            }
        });
    });
}

function onLeadsFresh(companyid) {
    $.ajax({
        url: '/sales/JsonRefreshLeads/',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        data: { CompanyID: companyid },
        dataType: 'html',
        success: function (result) {
            $('#leadscontainer').html(result);
            onSalesInputInitial();

        },
        error: function (xhr, status) {
            alert(xhr.responseText);
        }
    });
}

function onCompanyFresh() {
    $.ajax({
        url: '/sales/JsonRefreshLeads/',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        data: { CompanyID: companyid },
        dataType: 'html',
        success: function (result) {
            $('#leadscontainer').html(result);
            onSalesInputInitial();

        },
        error: function (xhr, status) {
            alert(xhr.responseText);
        }
    });
}

function onCallsFresh(leadid,projectid) {
    $.ajax({
        url: '/sales/JsonRefreshLeadcalls/',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        data: { LeadID: leadid, ProjectID: projectid },
        dataType: 'html',
        success: function (result) {
            $('#leadCallscontainer').html(result);
            $('#leadCallscontainer').find('.leadcallssubmit').find('.leadcallssubmit').each(function () {
                IntialLeadCallsSavebtn($(this));
            });
            onSalesInputInitial();

        },
        error: function (xhr, status) {
            alert(xhr.responseText);
        }
    });
}
