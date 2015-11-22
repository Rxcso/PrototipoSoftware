
$(function () {
    $.datepicker.setDefaults($.datepicker.regional['es']);
    $(".fecha").datepicker({
        dateFormat: "yy/mm/dd"
    });
});
