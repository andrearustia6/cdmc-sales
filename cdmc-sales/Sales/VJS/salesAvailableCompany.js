function onCompanysTreeviewDataBinding(e) {
    var projectid = $('#ProjectID').val();
    var url = "/sales/JsonGetCompanys/?projectid=" + projectid;
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: { name: null },
        success: function (result) {
            if (result) {
                var treeview = $('#companystreeview').data('tTreeView');
                var array = new Array();
                for (var i = 0; i < result.length; i++) {
                    array.push({ Text: result[i].Name, Value: result[i].ID })
                }
                if (e) {
                    treeview.dataBind(e.item, array);
                    initialBtns();
                }
            }
        }
    });
}


function initialBtns() {
    $('#companysubmit').click(function () {

        var $cf = $('#companydata');
        var name_EN = $cf.find("#Name_EN").val();
        var name_CH = $cf.find("#Name_CH").val();
        var contact = $cf.find("#Contact").val();
        var fax = $cf.find("#Fax").val();
        var address = $cf.find("#Address").val();
        var districtNumberID = $cf.find("#DistrictNumberID").val();
        var companyTypeID = $cf.find("#CompanyTypeID").val();
        var areaID = $cf.find("#AreaID").val();
        var foreignAssetPercentage = $cf.find("#ForeignAssetPercentage").val();
        var id = $cf.find("#ID").val();

        var c = { Name_EN: name_EN,
            Name_CH: name_CH,
            Fax: fax,
            Address: address,
            DistrictNumberID: districtNumberID,
            CompanyTypeID: companyTypeID,
            ID: id,
            Contact: contact,
            ForeignAssetPercentage: foreignAssetPercentage,
            AreaID: areaID
        };

        c = getCommonData($cf, c);

        c = JSON.stringify(c);
        $.ajax({
            url: "/sales/JsonSaveCompany",
            type: 'POST',
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            data: c,
            success: function (result) {
                alert('保存成功');
                $('#companydata').html(result);
                initialBtns();
            },
            error: function (xhr, status) {
                alert(xhr.responseText);
            }
        });
    });

    $('.leadsubmit').each(function () {
        IntialLeadSavebtn($(this));
    });
}


function IntialLeadSavebtn($btn) {
    var $cf = $btn.closest('.leaddatacontainer');
    $btn.click(function () {
        var name_EN = $cf.find("#Name_EN").val();
        var name_CH = $cf.find("#Name_CH").val();
        var contact = $cf.find("#Contact").val();
        var fax = $cf.find("#Fax").val();
        var title = $cf.find("#Title").val();
        var personalEmailAddress = $cf.find("#PersonalEmailAddress").val();
        var mobile = $cf.find("#Mobile").val();
        var gender = $cf.find("#Gender").val();
        var eMail = $cf.find("#EMail").val();
        var id = $cf.find("#ID").val();
        var companyid = $cf.find("#CompanyID").val();
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
            CompanyID: companyid
        };

        c = getCommonData($cf, c);

        c = JSON.stringify(c);
        $.ajax({
            url: "/sales/JsonSaveLead",
            type: 'POST',
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            data: c,
            success: function (result) {
                alert('保存成功');
                $cf.html(result);
                var lsave = $cf.find('.leadsubmit');
                IntialLeadSavebtn(lsave);
            },
            error: function (xhr, status) {
                alert(xhr.responseText);
            }
        });
    });
}

function IntialLeadCallsSavebtn($btn) {
    var $cf = $btn.closest('.singleleadcallcontainer');
    $btn.click(function () {
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
                $cf.html(result);
                var lsave = $cf.find('.leadcallssubmit');
                IntialLeadCallsSavebtn(lsave);
            },
            error: function (xhr, status) {
                alert(xhr.responseText);
            }
        });
    });
}


function onCompanysTreeviewNodeSelected(e) {

    var id = $(e.item).find(".t-input").val();

    $.ajax({
        url: '/sales/CompanyInfo/',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        data: { companyid: id },
        success: function (result) {
            // Display the section contents.

            $('#companydata').html(result);
            initialBtns();
        },
        error: function (xhr, status) {
            alert(xhr.responseText);
        }
    });

}


function onLeadExpended(e) {
    var $container = $(e.item).find("#leadCallscontainer");
    var id = $(e.item).find("input[type=hidden]").val();
    var projectid = $('#ProjectID').val();
    $.ajax({
        url: '/sales/LeadCalls/?projectid=' + projectid + '&leadid=' + id,
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        data: { leadid: id },
        success: function (result) {
            // Display the section contents.
            $(e.item).find('#leadCallscontainer').html(result);

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