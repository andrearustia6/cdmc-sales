function onModifyParticipantPageLoad() {
    var $addpdiv = $('.addParticipant');
    
    initialAddParticipantBtn($addpdiv);
}

function initialAddParticipantBtn($addpdiv) {
    $addpdiv.find('#addparticipant').unbind("click").bind("click", function () {
        var p = getParticipant($addpdiv);
        $.ajax({
            url: "/sales/JsonModifyParticipant",
            type: 'POST',
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            data: p,
            success: function (result) {
                alert('保存成功');
                initialAddParticipantBtn(addpdiv);
            },
            error: function (xhr, status) {
                alert(xhr.responseText);
            }
        });
    });
}

function getParticipant($pdiv) {
    var id = $pdiv.find('#ID').val();
    var name = $pdiv.find('#Name').val();
    var title = $pdiv.find('#Title').val();
    var gender = $pdiv.find('#Gender').val();
    var participantTypeID = $pdiv.find('#ParticipantTypeID').val();
    var dealID = $pdiv.find('#DealID').val();
    var contact = $pdiv.find('#Contact').val();
    var mobile = $pdiv.find('#Mobile').val();
    var email = $pdiv.find('#Email').val();
    var projectid = $pdiv.find('#ProjectID').val();
    var p = { 
        ID: id,
        Name: name,
        Title: title,
        Gender: gender,
        ParticipantTypeID: participantTypeID,
        DealID: dealID,
        Contact: contact,
        Mobile: mobile,
        Email: email,
        ProjectID: projectid
    }

    p = JSON.stringify(p);

    return p;
   
}