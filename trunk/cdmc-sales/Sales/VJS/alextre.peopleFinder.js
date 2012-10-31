(function ($) {
    function peopleClick($pf, $target) {
        var $li = $target.parent().parent();
        var $alreadyAdded = $('.pf-selectedresult').find('li[employeeID*="' + $li.attr('employeeID') + '"]');

        if ($target.attr('isChecked') == 'checked') {
            $target.removeAttr('isChecked');
            if ($alreadyAdded.length > 0)
                $alreadyAdded.remove();
        }
        else
            $target.attr('isChecked', 'checked');

        if ($target.attr('isChecked') == 'checked' & $alreadyAdded.length == 0) {
            var $add = $li.clone();
            $add.data('employee', $li.data('employee'));
            $add.appendTo($pf.find('.pf-selectedresult'));
            $add.find(':checkbox').click(function () {
                var $remove = $(this).parent().parent();
                $remove.remove();
                $('.pf-searchresult').find('li[employeeID="' + $remove.attr('employeeID') + '"]').find(':checkbox').removeAttr('checked').removeAttr('isChecked');
            });
        }
    }

    function InitialSearchResultList($pf, condition) {

        var condition = { "condition": condition };
        $.ajax({
            url: "/WorkFlow/PeopleFinder",
            type: 'POST',
            dataType: 'text json',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(condition),
//            dataFilter: function(d, type) {
//                var d = d.replace(/"\\\/(Date\([0-9-]+\))\\\/"/gi, 'new $1');
//               //var d = data.replace(/"\\\/(Date\(.*?\))\\\/"/gi, 'new $1');
//                return d;
//            },
            success: function (result) {
                var $searchlist = $pf.find('.pf-searchresult');
                $searchlist.children().remove();
                for (var i = 0; i < result.length; i++) {
                    var $li = generateLi(result[i].ID, result[i].Name).appendTo($searchlist);
                    $li.data('employee', result[i]);
                    var $addedcbk = $li.find('.pf-checkbox');
                    $addedcbk.click(function () {
                        peopleClick($pf, $(this));
                    });
                    var $exist = $pf.find('.pf-selectedresult').find('li[employeeID=' + result[i].id + ']');
                    if ($exist.length > 0) {
                        $addedcbk.attr('isChecked', 'checked');
                        $addedcbk.attr('Checked', 'checked');
                    }
                }
            }
        });
    }

    function generateLi(id, name) {
        return $('<li class="pf-li" employeeID="' + id + '"><label><input class="pf-checkbox" type="checkbox" />' + name + '</label></li>');
    }
    function InitialPeopleFinder($target) {
        var $pf = $('<div></div>');
        var $searchDiv = $('<div class="pf-search"><span>搜索：<input type="text" style="width: 300px" /></span></div>').appendTo($pf);
        var $searchBtn = $('<input type="button" class="pf-search-click" style="margin-left: 20px" value="开始搜索" />').appendTo($searchDiv.find('span'));
        $searchBtn.click(function () {
            var condition = $searchBtn.parent().find(':text').val();
            InitialSearchResultList($pf, condition);

        });
        var $listsDiv = $('<div style="max-height: 200px; height: 200px;"></div>').appendTo($pf);
        var $searchresultDiv = $('<div style="float: left; width: 50%"><ul  class="pf-searchresult" style="max-height: 200px; overflow:auto; height: 200px;list-style-type: none"></ul><div>').appendTo($listsDiv);
        var $selectedDiv = $('<div style="float: left; width: 50%"><ul class="pf-selectedresult" style="max-height: 200px;overflow:auto; height: 200px;list-style-type: none"></ul><div>').appendTo($listsDiv);

        InitialSearchResultList($pf, 'all');

        var $saveDiv = $('<center style="margin-right:0px;width:auto"><input type="button"value="确定"><input value="取消" type="button"/></center>').appendTo($pf);
        var array = $target.data('pf');
        if (array) {
            for (var i = 0; i < array.length; i++) {
                generateLi(array[i].id, array[i].Name).appendTo($selectedDiv.find('ul'));
                $searchresultDiv.find('li[employeeID="' + array[i].id + '"]').find(':checkbox').attr('ischecked', 'checked').attr('checked', 'checked');

            }
            $selectedDiv.find(':checkbox').attr('ischecked', 'checked').attr('checked', 'checked');
            var $sck = $selectedDiv.find(':checkbox').click(function () {
                var $remove = $(this).parent().parent();
                $remove.remove();
                $('.pf-searchresult').find('li[employeeID="' + $remove.attr('employeeID') + '"]').find(':checkbox').removeAttr('checked').removeAttr('isChecked');
            });
        }
        else {
            array = new Array();
        }
        //确定
        $saveDiv.children().first().click(function () {
            array = new Array(); //清空
            var result = "";
            $selectedDiv.find('li').each(function () {
                var $li = $(this);
                var data = $li.data('employee');
                array.push(data);
                result += $li.find('label').text() + ';';
            });

            $target.data('pf', array);
            $target.val(result);
            $('#w-peoplefinder').data('tWindow').close();
        });
        //取消
        $saveDiv.children().last().click(function () {
            $('#w-peoplefinder').data('tWindow').close();
        });

        $pf.find('.pf-searchresult').find('.pf-checkbox').each(function () {
            $(this).click(function () {
                var $t = $(this);
                peopleClick($pf, $t);
            });

        });
        var $screen = $(document);
        var w = $('#w-peoplefinder');
        w.css('left', $screen.width() / 2 - w.width() / 2).css('top', '150px');
        w.data('tWindow').content($pf).open();
    }

    function initialtartgetItem($target, settings) {
        $target.click(function () {
            InitialPeopleFinder($target);
        });
    }

    jQuery.fn.peopleFinder = function (options) {
        var settings = jQuery.extend({
            img: '/images/48.png'
        }, options);
        initialtartgetItem($(this), settings);
        return this;
    };
})(jQuery);