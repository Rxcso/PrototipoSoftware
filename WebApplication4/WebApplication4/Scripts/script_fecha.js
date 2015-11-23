
$(function () {
    $.datepicker.setDefaults($.datepicker.regional['es']);
    $(".fecha").datepicker({
        format: "dd-mm-yy",
        altFormat : "yy-mm-dd"
    });
});
