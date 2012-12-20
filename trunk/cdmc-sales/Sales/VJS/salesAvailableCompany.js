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
                        initialCompanysaveBtn();
                }
            }
        }
    });
}


function initialCompanysaveBtn() {
    $('.companysubmit').click(function () {

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
            ForeignAssetPercentage:foreignAssetPercentage,
            AreaID: areaID
        };

        c = getCommonData($cf,c);

        c = JSON.stringify(c);
        $.ajax({
            url: "/sales/JsonSaveCompany",
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: c,
            success: function (result) {
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
        },
        error: function (xhr, status) {
            alert(xhr.responseText);
        }
    });
       
}


function onLeadExpended(e){
    var $container = $(e.item).find("#leadCallscontainer");
    var id = $(e.item).find("input[type=hidden]").val();
    var projectid = $('#ProjectID').val();
    $.ajax({
        url: '/sales/LeadCalls/?projectid=' + projectid + '&leadid='+id,
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        data: { leadid: id },
        success: function (result) {
            // Display the section contents.
            $(e.item).find('#leadCallscontainer').html(result);
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