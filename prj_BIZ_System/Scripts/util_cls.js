var util_cls = {};

util_cls.form = {};
util_cls.json = {};

util_cls.form.row2form = function (id , form_id) {
    var jsonStr = $("#rowdata_" + id).text();
    var jsonObj = JSON.parse(jsonStr);
    var $form = $('#' + form_id);
    for (var x in jsonObj) {
        if ($form.find('input[name="' + x + '"]').size() > 0) {
            $form.find('input[name="' + x + '"]').val(jsonObj[x]);
        } else if ($form.find('select[name="' + x + '"]').size() > 0) {
            $form.find('select[name="' + x + '"]').val(jsonObj[x]);
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

util_cls.json.getValueFromJsonStr = function (jsonstr, key) {
    var result = "";
    if (jsonstr != null && jsonstr != '') {
        var jsonObj = JSON.parse(jsonstr);
        result = jsonObj[key];
    }
    return result;
}