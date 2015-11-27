
$(function () {
    $.datepicker.setDefaults($.datepicker.regional['es']);

    $(".fecha").datepicker({
        format: "dd-mm-yy",
        altFormat: "yy-mm-dd",        
        minDate: "-30D",
        maxDate: "+11M +10D"
    });

    $("#fech_ini").datepicker("option", "altField", "#altfech_ini");
    $("#fech_fin").datepicker("option", "altField", "#altfech_fin");   

     

    $(".fecha2").datepicker({
        changeYear: true,
        format: "yy-mm-dd",        
        maxDate: "+11M +10D"
    });
});
