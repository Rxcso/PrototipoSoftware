function validarFechaActualOMayor(fecha) {
    var fechaS = fecha.split('-');
    var fFecha = new Date(fechaS[0], --fechaS[1], fechaS[2]);
    fFecha.setHours(0, 0, 0, 0);

    var fechaInicioEvento = $("#fechaInicioEvento").val();
    var fIEv = fechaInicioEvento.split('-');
    var fIEvento = new Date(fIEv[0], --fIEv[1], fIEv[2]);
    fIEvento.setHours(0, 0, 0, 0);

    var maxDate = new Date();
    maxDate.setHours(0, 0, 0, 0);
    maxDate.setYear(maxDate.getFullYear() + 20);

    return (fFecha <= maxDate) && (fFecha >= fIEvento);
}

function format_date(date) {
    var formato = (date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + (date.getDate())).slice(-2));
    return formato;
}

function format_date2(date) {
    date = date.split('-');
    var formato = ('0' + date[2]).slice(-2) + '/' + ('0' + date[1]).slice(-2) + '/' + date[0];
    return formato;
}
var today = new Date();

function fila() {
    var rowId = $("#histFuncion").val();
    var table = document.getElementById("bloqueFuncion").getElementsByTagName('tbody')[0];
    var row = table.insertRow();
    row.id = parseInt(rowId) + 1;
    var cell0 = row.insertCell(0);
    cell0.innerHTML = parseInt(row.id);
    var cell1 = row.insertCell(1);
    cell1.innerHTML = '<input id="fechaFuncion" class="form-control" type="date" required>';
    var cell2 = row.insertCell(2);
    cell2.innerHTML = '<input id="horaFuncion" class="form-control" type="time" required>';
    var cell3 = row.insertCell(3);
    cell3.align = "center";
    cell3.innerHTML = '<input type="radio" name="groupFunciones" value="' + (parseInt(row.id)) + '">';
    $("#histFuncion").val(parseInt(row.id));
}
function validar(fecha, hora) {
    if (fecha && hora) {
        if (validarFechaActualOMayor(fecha)) {
            return true;
        } else {
            var fechaInicioEvento = $("#fechaInicioEvento").val();
            alert("Fecha de funcion incorrecta: " + format_date2(fecha) + ". Ingrese una fecha valida (Rango de fechas: Mayor o igual a la fecha de inicio del evento: " + format_date2(fechaInicioEvento) + " hasta fechas del año " + (today.getFullYear() + 20) + ").");
        }
    } else {
        alert("Campos Vacios. Ingrese nuevamente.");
    }
    return false;
}


function agregaFuncion() {
    var tableBTV = document.getElementById("bloqueFuncion");
    if (tableBTV.rows.length > 1) {
        var row = tableBTV.rows[tableBTV.rows.length - 1];
        var fecha = row.cells[1].children[0].value;
        var hora = row.cells[2].children[0].value;
        if (validar(fecha, hora)) {
            row.cells[1].children[0].setAttribute('readonly', 'true');
            row.cells[2].children[0].setAttribute('readonly', 'true');
            fila();
        }
    } else {
        fila();
    }
}
function eliminarFuncion() {
    var fila = $('input[name="groupFunciones"]:checked').val();
    var row = document.getElementById(parseInt(fila));
    row.parentNode.removeChild(row);
}
function guardarFunciones() {
    var nombreLista = "ListaFunciones";
    var tableBTV = document.getElementById("bloqueFuncion");
    if (tableBTV.rows.length == 1) {
        alert("No hay funciones. Agrege una funcion.");
        return false;
    }
    var row = tableBTV.rows[tableBTV.rows.length - 1];
    var fecha = row.cells[1].children[0].value;
    var hora = row.cells[2].children[0].value;
    if (validar(fecha, hora)) {
        for (var i = 1; i < tableBTV.rows.length; i++) {
            var row = tableBTV.rows[i];
            var fecha = row.cells[1].children[0].value;
            var hora = row.cells[2].children[0].value;
            fecha = fecha.split('-');
            hora = hora.split(':');
            var date = fecha[2] + "-" + --fecha[1] + "-" + fecha[0] + " " + hora[0] + ":" + hora[1];
            console.log(i + ") " + date);
            $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].fechaFuncion' value='" + date + "'>");
            $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].horaInicio' value='" + date + "'>");
        }
        return true;
    }
    return false;
}