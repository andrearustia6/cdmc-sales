function onCompanysTreeviewDataBinding(e) {
    $.ajax({
        url: "/sales/JsonGetCompanys",
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
       



//    $.ajax({
//        url: "/sales/GetCompanyDetails",
//        type: 'GET',
//        dataType: 'json',
//        contentType: 'application/json; charset=utf-8',
//        data: { companyid: id },
//        error: function (jqXHR, textStatus, errorThrown) {
//            alert('error');
//        },
//        success: function (result) {
//            if (result) {
//                setComany(result);
//                setLeads(result);
//            }
//        }
//    });
}
//function setLeads(result) {
//    var $lf = $('#leadsdata');
//    var leadsdata = result.Leads;
//    for (var i = 0; i < leadsdata.length; i++) {
//        $lf.load("/views/shared/LeadInfo.cshtml", function (response, status, xhr) {
//            if (status == "error") {
//                var msg = "Sorry but there was an error: "; 
//        $("#error").html(msg + xhr.status + " " + xhr.statusText); } });
//    }

//}
//function setComany(result) {
//    var c = result.Company;
//    var $cf = $('#companydata');
//    $cf.find("#Name_CH").val(c.Name_CH);
//    $cf.find("#Name_EN").val(c.Name_EN);
//    $cf.find("#Contact").val(c.Contact);
//    $cf.find("#Fax").val(c.Fax);
//    $cf.find("#Address").val(c.Address);
//    $cf.find("#DistrictNumberID").val(c.DistrictNumberID);
//    $cf.find("#CompanyTypeID").val(c.CompanyTypeID);
//    $cf.find("#ID").val(c.ID);
//    $cf.find("#AreaID").val(c.AreaID);
//    $cf.find("#ForeignAssetPercentage").val(c.ForeignAssetPercentage);
//    
//    setCommonfield($cf,c);
//}

//function setCommonfield($source,c) {
//    $source.find("#Creator").val(c.Creator);
//    $source.find("#CreatedDate").val(c.CreatedDate);
//    $source.find("#ModifiedUser").val(c.ModifiedUser);
//    $source.find("#ModifiedDate").val(c.ModifiedDate);
//    $source.find("#Description").val(c.Description);
//    $source.find("#Sequence").val(c.Sequence);
//}

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