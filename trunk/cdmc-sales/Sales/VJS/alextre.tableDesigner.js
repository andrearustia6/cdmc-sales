(function ($) {

    function initialRowCommon($table, fieldname, must) {
        var $v = getValueInputTd($table, fieldname);

        if (!$v) {
            var $row = $('<tr></tr>').appendTo($table);
            var $tdName = $('<td style="width:25%"></td>').appendTo($row);
            var $name = $('<p class="td-name" style="float:right">' + fieldname + ' : ' + '</p>').appendTo($tdName);
            var $v = $('<td class="td-value" style="width:85%"></td>').appendTo($row);
        }

        if (must)
            $v.attr('must', must)

        return $v;
    }

    function getValueInputTd($table, fieldname, must) {
        var $td;
        $table.find('p').each(function () {
            var v = $(this).html();
            v = $.trim(v.replace(":", ""));
            if (v == fieldname) {
                $td = $(this).parent().parent().find('.td-value');
            }
        });
        return $td;
    }

    function initialTextInputRow($table, fieldname, inputvalue, must) {
        var $tdValue = initialRowCommon($table, fieldname, must);
        var $value = $('<input type="text" style="width:95%;"/>').appendTo($tdValue);
        if (inputvalue) {
            $value.val(inputvalue);
        }
    }

    function initialNumberInputRow($table, fieldname, inputvalue, min, max, must) {
        var $tdValue = initialRowCommon($table, fieldname, must);
        var $value = $('<input type="text" style="width:95%;"/>').appendTo($tdValue);
        if (min && max && max > min)
            $value.jStepper({ minValue: min, maxValue: max });
        else
            $value.jStepper();

        $value.val(inputvalue);

    }

    function initalCheckboxInputRow($table, fieldname, inputvalue, optionalValue, must) {
        var $tdValue = initialRowCommon($table, fieldname, must);
        var ovs = optionalValue.split('||');

        for (var i = 0; i < ovs.length; i++) {
            $('<input type="checkbox" value="' + ovs[i] + '" class="checkbox">' + ovs[i] + '</input>').appendTo($tdValue).click(function () {
                var $ck = $(this);
                if ($ck.attr('isChecked') == 'checked') {
                    $ck.removeAttr('isChecked');
                }
                else {
                    $ck.attr('isChecked', 'checked');
                }
            });
        }
        if (inputvalue) {
            var ivs = inputvalue.split('||');
            for (var j = 0; j < ivs.length; j++) {
                $tdValue.find('[value="' + ivs[j] + '"]').attr('checked', 'checked').attr('isChecked', 'checked');
            }
        }
    }

    function initialDropDownList($table, fieldname, inputvalue, optionalvalue, must) {
        var $tdValue = initialRowCommon($table, fieldname, must);
        var ovs = optionalvalue.split('||');
        var $select = $('<select style="width:97%" class="select"></select>').appendTo($tdValue);
        $('<option selected="selected" value="-1">--请选择--</option>').appendTo($select);
        for (var i = 0; i < ovs.length; i++) {
            if (ovs[i]!="")
               $('<option ></option>').text(ovs[i]).val(ovs[i]).appendTo($select);
        }
        if (inputvalue) {
            $tdValue.find('option[value="' + inputvalue + '"]').attr('selected', 'selected');
        }
    }

    function initialSlideInputRow($table, fieldname, value, min, max, step, display) {
        var $tdValue = initialRowCommon($table, fieldname);
        var $value = $('<div class="slide" style="width:95%;"></div>').appendTo($tdValue).slider({
            value: value,
            min: min,
            max: max,
            step: step,
            slide: function (event, ui) {
                $value.attr('slideValue', ui.value);
                $tdValue.find('.display').html(ui.value);
            }
        });
        if (value) {
            $value.attr('slideValue', value);
            $('<div class="display"></div>').appendTo($tdValue).html(value);
        }
    }

    function initialRadioInputRow($table, fieldname, inputvalue, optionalValue) {
        var $tdValue = initialRowCommon($table, fieldname);
        var ovs = optionalValue.split('||');
        for (var i = 0; i < ovs.length; i++) {
            $('<input type="radio" value="' + ovs[i] + '" class="radio">' + ovs[i] + '</input>').appendTo($tdValue).click(function () {
                var $this = $(this);
                var parent = $this.parent();
                parent.find('input').each(function () {
                    $(this).removeAttr("checked");
                });
                $this.attr('checked', 'checked');
            });
        }
        if (inputvalue) {
            var checkedradio;
            $tdValue.find('input').each(function () {
                var $radio = $(this);
                if ($radio.val() == inputvalue) {
                    $radio.attr('checked', 'checkedradio');
                }
            })
        }
        else {
            $tdValue.find('input').first().attr('checked', 'checkedradio');
        }
    }

    function initalPeopleFinderInputRow($table, fieldname, inputvalue, fieldvaluedata) {
        var $tdValue = initialRowCommon($table, fieldname);
        var $input = $('<input type="text" style="width:95%;"/>');
        $input.data('pf', fieldvaluedata);
        $input.peopleFinder().appendTo($tdValue).val(inputvalue);
    }

    function initalDatePickerInputRow($table, fieldname, inputvalue) {
        var $tdValue = initialRowCommon($table, fieldname);
        $('<input type="text" style="width:95%;"/>').datepicker().appendTo($tdValue).val(inputvalue);
    }

    function addSubmitCanelBtn($table, $target, save, cancel) {
        var $row = $('<tr></tr>').appendTo($table);
        var $tdName = $('<td colspan="2" padding="5"></td>').appendTo($row);
        var $div = $('<div style="margin-right:10px;width:200px;padding:5px;float:right"></div>').appendTo($tdName);
        var $cancel = $('<input type="button" class="window_cancel" style="float:right;" value="取消"/>').appendTo($div);
        var $confirm = $('<input type="button" style="float:right;" value="确定"/>').appendTo($div);
        var result;
        $confirm.click(function () {
            var sucess = saveValue($table, $target);
            $target.attr('sucess', sucess);
        });
        if (save)
            $confirm.click(save);
        if (cancel)
            $cancel.click(cancel);
    }
    jQuery.fn.singletonRadioSelection = function () {
        $radio = $(this);
        $radio.click(function () {
            var $this = $(this);
            var parent = $this.parent();
            parent.find('input').each(function () {
                $(this).removeAttr("checked");
            });
            $this.attr('checked', 'checked');
        });


    }
    jQuery.fn.getTable = function (options) {
        var settings = jQuery.extend({
            save: null,
            cancel: null
        }, options);

        var $table = $('<table style="width:95%;"></table>');
        var $target = $(this);

        var av = $target.data('av');
        var i = 0;
        for (i; i < av.length; i++) {
            var current = av[i];
            if (current.controltype == 'text') {
                initialTextInputRow($table, current.fieldname, current.fieldvalue, current.must);
            }
            else if (current.controltype == 'number') {
                initialNumberInputRow($table, current.fieldname, current.fieldvalue, current.min, current.max, current.must);
            } else if (current.controltype == 'radio') {
                initialRadioInputRow($table, current.fieldname, current.fieldvalue, current.optionalvalue);
            } else if (current.controltype == 'slide') {
                initialSlideInputRow($table, current.fieldname, current.fieldvalue, current.min, current.max);
            } else if (current.controltype == 'checkbox') {
                initalCheckboxInputRow($table, current.fieldname, current.fieldvalue, current.optionalvalue, current.must);
            } else if (current.controltype == 'peoplefinder') {
                initalPeopleFinderInputRow($table, current.fieldname, current.fieldvalue, current.fieldvaluedata);
            } else if (current.controltype == 'dropdownlist') {
                initialDropDownList($table, current.fieldname, current.fieldvalue, current.optionalvalue, current.must);
            } else if (current.controltype == 'datepicker') {
                initalDatePickerInputRow($table, current.fieldname, current.fieldvalue);
            }
        }
        addSubmitCanelBtn($table, $target, settings.save, settings.cancel);
        return $table;
    }

    function checkValue($table) {
        var flag = true;
        $table.find('.td-value').each(function () {
            var $tdvalue = $(this);
            if ($tdvalue.attr('must') == 'true') {
                var $v;
                $tdvalue.find(':text').each(function () {
                    $v = $(this).val();
                    if (!$v || $.trim($v) == '') {
                        var $title = $tdvalue.parent().first('td').find('p').html();
                        $title = $.trim($title.replace(":", ""));
                        alert($title + ' 不能为空，请填入信息');
                        $(this).focus();
                        flag = false;
                        return flag;
                    }
                });
                if (flag == false)
                    return flag;
                $tdvalue.find('textarea').each(function () {
                    $v = $(this).val();
                    if (!$v || $.trim($v) == '') {
                        var $title = $tdvalue.parent().first('td').find('p').html();
                        $title = $.trim($title.replace(":", ""));
                        alert($title + ' 不能为空，请填入信息');
                        $(this).focus();
                        flag = false;
                        return flag;
                    }
                });
                if (flag == false)
                    return flag;
                $tdvalue.find('select :selected').each(function () {
                    v = $(this).val();
                    if (v == '-1') {
                        var $title = $tdvalue.parent().first('td').find('p').html();
                        $title = $.trim($title.replace(":", ""));
                        alert($title + '的值未选择，请选择');
                        $(this).focus();
                        flag = false;
                        return flag;
                    }
                });
                if (flag == false)
                    return flag;

                var chs = $tdvalue.find('.checkbox');
                if (chs.length > 0) {
                    chs = $tdvalue.find('.checkbox[isChecked="checked"]');
                    if (chs.length == 0) {
                        var $title = $tdvalue.parent().first('td').find('p').html();
                        $title = $.trim($title.replace(":", ""));
                        alert($title + '的值未选择，请选择');
                        flag = false;
                        return flag;
                    }
                }
            }
        });
        return flag;
    }

    function saveValue($table, $target) {
        if (!checkValue($table)) { return false; }
        var av = $target.data('av');
        for (var i = 0; i < av.length; i++) {
            var $td = getValueInputTd($table, av[i].fieldname);
            if (av[i].controltype == 'text' || av[i].controltype == 'number') {
                var v = $td.find('input').val();
                av[i].fieldvalue = v;
            }
            else if (av[i].controltype == 'radio') {
                var $checked;
                $td.find('.radio').each(function () {
                    if ($(this).attr('checked') == 'checked') {
                        $checked = $(this);
                        var v = $checked.val();
                        av[i].fieldvalue = v;
                    }
                });
            }
            else if (av[i].controltype == 'checkbox') {
                if (av[i].fieldvaluereference != null) {
                    var conditions = new Array();

                    $td.find('.checkbox').each(function () {
                        if ($(this).attr('isChecked') == 'checked') {
                            var conditionname = $(this).val();
                            var refertarget = $(av[i].fieldvaluereference + "[uniquename=" + conditionname + "]");
                            conditions.push(refertarget.data('lidata'));
                        }
                    });
                    av[i].fieldvalue = conditions;
                }
                else {
                    var value = '||';
                    $td.find('.checkbox').each(function () {
                        if ($(this).attr('isChecked') == 'checked') {
                            var v = $(this).val();
                            value = value + v + '||';
                        }
                    });
                    av[i].fieldvalue = value;
                }
            }
            else if (av[i].controltype == 'slide') {
                $td.find('.slide').each(function () {
                    av[i].fieldvalue = $(this).attr('slideValue');
                });
            }
            else if (av[i].controltype == 'dropdownlist') {
                $td.find(':selected').each(function () {
                    if ($(this).val() != '-1') {
                        var selectedvalue = $(this).val();

                        if (av[i].fieldvaluereference != null) {
                            var test = $(av[i].fieldvaluereference);
                            var refertarget = $(av[i].fieldvaluereference + "[uniquename=" + selectedvalue + "]");
                            av[i].fieldvalue = refertarget.data('lidata');
                        }
                        else
                            av[i].fieldvalue = selectedvalue;

                    }

                    //保存有引用关系的数据

                });
            }
            else if (av[i].controltype == 'peoplefinder') {
                var v = $td.find('input').val();
                var data = $td.find('input').data('pf');
                av[i].fieldvalue = v;
                av[i].fieldvaluedata = data;
            }
            else if (av[i].controltype == 'datepicker') {
                var v = $td.find('input').val();
                av[i].fieldvalue = v;
            }
        }
        $target.data('av', av);
        return true;
    }
})(jQuery);