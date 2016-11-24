var util_cls = {};

util_cls.form = {};
util_cls.json = {};

util_cls.form.row2form = function (id , form_id) {
    var jsonStr = $("#rowdata_" + id).text();
    var jsonObj = JSON.parse(jsonStr);
    var $form = $('#' + form_id);
    for (var i in jsonObj) {
        if (i == "enable") {
            if (jsonObj[i] == "0") {
                $("#enable01").prop("checked", true);
            } else {
                $("#enable02").prop("checked", true);
            }
        } else {
            if ($form.find('input[name="' + i + '"]').size() > 0) {
                $form.find('input[name="' + i + '"]').val(jsonObj[i]);
            } else if ($form.find('select[name="' + i + '"]').size() > 0) {
                $form.find('select[name="' + i + '"]').val(jsonObj[i]);
            }
        }
    }
}

util_cls.form.form2row = function (id, form_id) {
    var $form = $('#' + form_id);
    var oldJsonStr = $("#rowdata_" + id).text();
    var oldJsonObj = JSON.parse(oldJsonStr);
    var newJsonStr = $form.serialize();
    var newJsonAry = $form.serializeArray();

    for (var x in oldJsonObj) {
        for (var i = 0 ; i < newJsonAry.length;i++){
            if (newJsonAry[i].name == x) {
                oldJsonObj[x] = newJsonAry[i].value;
                break;
            }
        }
    }

    $("#rowdata_" + id).text(JSON.stringify(oldJsonObj));

}

util_cls.form.row2span = function (id, form_id) {
    var jsonStr = $("#rowdata_" + id).text();
    var jsonObj = JSON.parse(jsonStr);
    var $form = $('#' + form_id);
    for (var i in jsonObj) {
        if (i == "enable") {
            if (jsonObj[i] == "0") {
                $("#enable01").prop("checked", true);
            } else {
                $("#enable02").prop("checked", true);
            }
        } else {
            if ($form.find('span[name="' + i + '"]').size() > 0) {
                $form.find('span[name="' + i + '"]').html(jsonObj[i]);
            } else if ($form.find('select[name="' + i + '"]').size() > 0) {
                $form.find('select[name="' + i + '"]').val(jsonObj[i]);
            }
        }
    }
}

util_cls.json.getValueFromJsonStr = function (jsonstr, i) {
    var result = "";
    if (jsonstr != null && jsonstr != '') {
        var jsonObj = JSON.parse(jsonstr);
        result = jsonObj[i];
    }
    return result;
}

util_cls.checkFileSize = function (id, limit_size, alertMsg) {
    var fileSize = 0;
    if (!limit_size) {
        limit_size = 10 * 1024 * 1024;
    }

    var file = document.getElementById(id);
    if ($.browser.msie) {
        var img = new Image();
        img.onload = checkFileSize();
        img.src = file.value;
    }
    else {
        fileSize = file.files.item(0).size;
        return checkFileSize();
    }
    function checkFileSize() {
        if ($.browser.msie) {
            fileSize = this.fileSize;
            if (fileSize > limit_size) {
                Message((fileSize / 1024 / 1024 ).toPrecision(4), (limit_size / 1024 / 1024 ).toPrecision(2));
            } else {
                //document.FileForm.submit();
            }

        }
        //alert((fileSize / 1024 / 1024).toPrecision(4), (limit_size / 1024 / 1024).toPrecision(2) + "MB");
        return fileSize > limit_size;
    }

    function Message(file, limit) {
        var msg = " " + file + "  " + limit + " ！"
        if (alertMsg) {
            alert(alertMsg);
        } else {
            alert(msg);
        }
    }
}