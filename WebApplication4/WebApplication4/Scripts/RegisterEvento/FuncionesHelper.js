﻿function format_time(date_obj) {
    // formats a javascript Date object into a 12h AM/PM time string
    var hour = date_obj.getHours() + 5;
    var minute = date_obj.getMinutes();
    var amPM = (hour > 11) ? "pm" : "am";
    if (hour > 12) {
        hour -= 12;
    } else if (hour == 0) {
        hour = "12";
    }
    if (minute < 10) {
        minute = "0" + minute;
    }
    return hour + ":" + minute + amPM;
}
function format_date(date) {
    var formato = (date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + (date.getDate())).slice(-2));
    return formato;
}
function agregaFuncion() {
    var fecha = $('#fechaFuncion').val();
    var date = new Date(fecha);
    var fechaF = format_date(date);
    var horaF = format_time(date);
    if (fecha) {
        var rowId = $("#histFuncion").val();
        var table = document.getElementById("bloqueFuncion");
        var row = table.insertRow();
        row.setAttribute("data-fecha", fechaF);
        row.setAttribute("data-hora", horaF);
        row.id = rowId;
        var cell0 = row.insertCell(0);
        cell0.innerHTML = parseInt(row.id) + 1;
        var cell1 = row.insertCell(1);
        cell1.innerHTML = fechaF;
        var cell2 = row.insertCell(2);
        cell2.innerHTML = horaF;
        var cell3 = row.insertCell(3);
        cell3.align = "center";
        cell3.innerHTML = '<input type="radio" name="groupFunciones" value="' + (parseInt(row.id)) + '">';

        $("#fechaFuncion").val('');
        $("#horaFuncion").val('');

        $("#fechaFuncion").focus();
        $("#histFuncion").val(parseInt(row.id) + 1);
    }
}
function eliminarFuncion() {
    var fila = $('input[name="groupFunciones"]:checked').val();
    var row = document.getElementById(parseInt(fila));
    row.parentNode.removeChild(row);
    var cantidad = parseInt($('#histFuncion').val());
    $("#histFuncion").val(cantidad - 1);
}
function guardarFunciones() {
    var nombreLista = "ListaFunciones";
    var tableBTV = document.getElementById("bloqueFuncion");
    for (var i = 1; i < tableBTV.rows.length; i++) {
        var fecha = tableBTV.rows[i].getAttribute("data-fecha");
        var hora = tableBTV.rows[i].getAttribute("data-hora");
        alert(fecha + " - " + hora);
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].fechaFuncion' value='" + fecha + "'>");
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].horaInicio' value='" + hora + "'>");
    }
}