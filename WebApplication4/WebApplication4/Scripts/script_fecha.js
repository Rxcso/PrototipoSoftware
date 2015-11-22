
$(function () {
    $.datepicker.setDefaults($.datepicker.regional['es']);
    $(".fecha").datepicker({
        dateFormat: "dd/mm/yy"
    });
});
