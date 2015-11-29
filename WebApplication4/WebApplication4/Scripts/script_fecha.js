
$(function () {
    $.datepicker.setDefaults($.datepicker.regional['es']);

    $(".fecha").datepicker({
        format: "dd-mm-yy",
        altFormat: "yy-mm-dd",
        minDate: "-30D",
        maxDate: "+12M",
        showAnim: "fade",
        changeYear: true
    });

    $("#fech_ini").datepicker("option", "altField", "#altfech_ini");
    $("#fech_fin").datepicker("option", "altField", "#altfech_fin");


    $(".fecha2").datepicker({
        format: "dd-mm",
        maxDate: "+0D",
        showAnim: "fade",
        changeYear: true
    });


    //para index 2

    $(".fechar").datepicker({
        dateFormat: "yy/mm/dd",
        //altFormat: "yy-mm-dd",        
        showAnim: "fade",
        changeYear: true
    });


});
