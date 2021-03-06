﻿function onModifyParticipantPageLoad() {
    initialButtons();
}

function initialButtons() { 
    initialAddParticipantBtn();
    initialEditParticipantBtn();
    initialDeleteParticipantBtn();
}
function initialDeleteParticipantBtn() {
    var $editdiv = $('.editparticipant');
    $editdiv.each(function () {
        var $this = $(this);
        $this.find('#deletebtn').unbind("click").bind("click", function () {

            var pid = $this.find('#ID').val();
            var did = $this.find('#DealID').val();
            id = JSON.stringify({ ParticipantID: pid, DealID: did });
            if (confirm("确认删除？")) {

                $.ajax({
                    url: "/sales/JsonDeleteParticipant",
                    type: 'POST',
                    dataType: 'html',
                    contentType: 'application/json; charset=utf-8',
                    data: id,
                    success: function (result) {
                        $('.participantContainer').html('');
                        location.reload();
                        
                        initialButtons();
                    },
                    error: function (xhr, status) {
                        alert(xhr.responseText);
                    }
                });

            }
        });
    });
}

function initialEditParticipantBtn() {

    var $editdiv = $('.editparticipant');
    $editdiv.each(function () {
        var $this = $(this);
        $this.find('#savebtn').unbind("click").bind("click", function () {
            var p = getParticipant($this);

            $.ajax({
                url: "/sales/JsonModifyParticipant",
                type: 'POST',
                dataType: 'html',
                contentType: 'application/json; charset=utf-8',
                data: p,
                success: function (result) {

                    if (result.indexOf("has_msg") < 0) {
                        alert('保存成功');
                        $('.participantContainer').html('');
                        location.reload();
                        initialButtons();
                    }
                    else {
                        $this.find('.messageparticipant').html(result);
                    }
                },
                error: function (xhr, status) {
                    alert(xhr.responseText);
                }
            });
        });
    });

}

function initialAddParticipantBtn() {
    var $addpdiv = $('.addParticipant');
    $addpdiv.find('#addparticipant').unbind("click").bind("click", function () {
        var p = getParticipant($addpdiv);
        $.ajax({
            url: "/sales/JsonModifyParticipant",
            type: 'POST',
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            data: p,
            success: function (result) {

                if (result.indexOf("has_msg") < 0) {
                    alert('保存成功');
                    $('.participantContainer').html('');
                    location.reload();
                   initialButtons();
                }
                else {
                    $('.participantContainer').find('.inputmessageaddparticipant').html(result);
                }
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
    var zip = $pdiv.find('#ZIP').val();
    var address = $pdiv.find('#Address').val();
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
        ProjectID: projectid,
        ZIP: zip,
        Address: address
    }

    p = JSON.stringify(p);

    return p;
   
}