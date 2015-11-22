function validarFechaActualOMayor(fecha) {
    var fechaS = fecha.split('-');
    var fFecha = new Date(fechaS[0], --fechaS[1], fechaS[2]);

    var today = new Date();
    today.setHours(0, 0, 0, 0);

    var maxDate = new Date();
    maxDate.setHours(0, 0, 0, 0);
    maxDate.setYear(maxDate.getFullYear() + 20);
    return (fFecha >= today) && (fFecha <= maxDate);
}

function validarFechaInicioYFin(fechaInicio, fechaFin) {
    var date = new Date();
    date.setHours(0, 0, 0, 0);
    var dateYear = date.getFullYear();

    var malInicio = 0, malFinal = 0;
    if (!validarFechaActualOMayor(fechaInicio)) {
        malInicio = 1;
    }

    if (!validarFechaActualOMayor(fechaFin)) {
        malFinal = 1;
    }

    if (malInicio) {
        alert("Fecha de inicio incorrecta: " + format_date(fechaInicio) + ". Ingrese una fecha valida (Rango de fecha: " + date.getDate() + "/" + (date.getMonth() + 1) + "/" + dateYear + " - " + (dateYear + 20) + ").");
    }
    if (malFinal) {
        alert("Fecha de fin incorrecta: " + format_date(fechaFin) + ". Ingrese una fecha valida (Rango de fecha: " + date.getDate() + "/" + (date.getMonth() + 1) + "/" + dateYear + " - " + (dateYear + 20) + ").");
    }
    if (!malInicio && !malFinal) {
        return true;
    }
    return false;
}

function validarRangoFechas(fechaInicio, fechaFin) {
    var inicio = fechaInicio.split('-');
    var fInicio = new Date(inicio[0], --inicio[1], inicio[2]);
    var fin = fechaFin.split('-');
    var fFin = new Date(fin[0], --fin[1], fin[2]);
    return fInicio <= fFin;
}

function format_date(date) {
    date = date.split('-');
    var formato = date[2] + '/' + (parseInt(--date[1]) + 1) + '/' + date[0];
    return formato;
}

function fila() {
    var rowId = $("#histBloque").val();
    var table = document.getElementById("bloqueDeTiempo").getElementsByTagName('tbody')[0];
    var row = table.insertRow();
    row.id = parseInt(rowId) + 1;
    var cell0 = row.insertCell(0);
    cell0.innerHTML = parseInt(row.id);
    var cell1 = row.insertCell(1);//desde
    cell1.innerHTML = '<input id="fechaIni" class="form-control" type="date" required>';
    var cell2 = row.insertCell(2);//hasta
    cell2.innerHTML = '<input id="fechaFin" class="form-control" type="date" required>';
    var cell3 = row.insertCell(3);
    cell3.align = "center";
    cell3.innerHTML = '<input type="radio" name="groupBloquedeVenta" value="' + (parseInt(row.id)) + '">';
    $("#histBloque").val(parseInt(row.id));
}

function validar(fechaInicio, fechaFin) {
    if (fechaInicio && fechaFin) {
        //verificar que sean fechas validas
        if (validarFechaInicioYFin(fechaInicio, fechaFin)) {
            //verificar que la fecha de inicio sea menor que la fecha fin
            if (validarRangoFechas(fechaInicio, fechaFin)) {
                return true;
            } else {
                alert("La fecha de fin debe ser posterior a al fecha inicio");
            }
        }
    } else {
        alert("Campos vacios. Ingrese datos nuevamente.");
    }
    return false;
}

function nuevoBloque() {
    var tableBTV = document.getElementById("bloqueDeTiempo");
    //valida n-1 filas
    if (tableBTV.rows.length > 1) {
        //si hay contenido
        var row = tableBTV.rows[tableBTV.rows.length - 1];
        var fechaInicio = row.cells[1].children[0].value;
        var fechaFin = row.cells[2].children[0].value;
        if (validar(fechaInicio, fechaFin)) {
            row.cells[1].children[0].setAttribute('readonly', 'true');
            row.cells[2].children[0].setAttribute('readonly', 'true');
            fila();
        }
    } else {
        //si es la primera vez se agrega la fila
        fila();
    }
}

function quitarBloque() {
    var fila = $('input[name="groupBloquedeVenta"]:checked').val();
    var row = document.getElementById(parseInt(fila));
    row.parentNode.removeChild(row);
}

function guardarBloques() {
    var nombreLista = "ListaBTM";
    var tableBTV = document.getElementById("bloqueDeTiempo");
    if (tableBTV.rows.length == 1) {
        alert("No hay bloques de tiempo de venta. Agregue un bloque.");
        return false;
    }
    //valido la ultima fila ingresada
    var row = tableBTV.rows[tableBTV.rows.length - 1];
    var fechaInicio = row.cells[1].children[0].value;
    var fechaFin = row.cells[2].children[0].value;
    if (validar(fechaInicio, fechaFin)) {
        //si todo esta correcto guardo los datos
        row.cells[1].children[0].setAttribute('readonly', 'true');
        row.cells[2].children[0].setAttribute('readonly', 'true');
        for (var i = 1; i < tableBTV.rows.length; i++) {
            var fechaInicio = tableBTV.rows[i].cells[1].children[0].value;
            var fechaFin = tableBTV.rows[i].cells[2].children[0].value;
            $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].fechaInicio' value='" + fechaInicio + "'>");
            $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].fechaFin' value='" + fechaFin + "'>");
        }
        return true;
    }
    return false;
}