
$(function () {
    $.datepicker.setDefaults($.datepicker.regional['es']);

    $(".fecha").datepicker({
        format: "dd-mm-yy",
        altFormat: "yy-mm-dd",
        minDate: -30,
        maxDate: "+1M +10D"
    });

    $("#fech_ini").datepicker("option", "altField", "#altfech_ini");
    $("#fech_fin").datepicker("option", "altField", "#altfech_fin");   

    
});
