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
        alert("Fecha de inicio incorrecta: " + fechaInicio + ". Ingrese una fecha valida (Rango de fecha: " + date.getDate() + "/" + (date.getMonth() + 1) + "/" + dateYear + " - " + (dateYear + 20) + ").");
    }
    if (malFinal) {
        alert("Fecha de fin incorrecta: " + fechaFin + ". Ingrese una fecha valida (Rango de fecha: " + date.getDate() + "/" + (date.getMonth() + 1) + "/" + dateYear + " - " + (dateYear + 20) + ").");
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
function nuevoBloque() {
    var fechaInicio = $("#fechaInicioInput").val();
    var fechaFin = $("#fechaFinInput").val();
    //tiene que poner las fechas si o si
    if (fechaInicio && fechaFin) {
        //verificar que sean fechas validas
        if (validarFechaInicioYFin(fechaInicio, fechaFin)) {
            //verificar que la fecha de inicio sea menor que la fecha fin
            if (validarRangoFechas(fechaInicio, fechaFin)) {
                var rowId = $("#histBloque").val();
                var table = document.getElementById("bloqueDeTiempo");
                var row = table.insertRow();
                row.setAttribute("data-fechaInicio", fechaInicio);
                row.setAttribute("data-fechaFin", fechaFin);
                row.id = rowId;
                var cell0 = row.insertCell(0);
                cell0.innerHTML = parseInt(row.id) + 1;
                var cell1 = row.insertCell(1);//desde
                cell1.innerHTML = fechaInicio;
                var cell2 = row.insertCell(2);//hasta
                cell2.innerHTML = fechaFin;
                var cell3 = row.insertCell(3);
                cell3.align = "center";
                cell3.innerHTML = '<input type="radio" name="groupBloquedeVenta" value="' + (parseInt(row.id)) + '">';

                $("#fechaInicioInput").val('');
                $("#fechaFinInput").val('');

                $("#fechaInicioInput").focus();
                $("#histBloque").val(parseInt(row.id) + 1);
            } else {
                alert("La fecha de fin debe ser posterior a al fecha inicio");
            }
        }
    } else {
        alert("Campos vacios. Ingrese datos nuevamente.");
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
    };
    for (var i = 1; i < tableBTV.rows.length; i++) {
        var fechaInicio = tableBTV.rows[i].getAttribute("data-fechaInicio");
        var fechaFin = tableBTV.rows[i].getAttribute("data-fechaFin");
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].fechaInicio' value='" + fechaInicio + "'>");
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].fechaFin' value='" + fechaFin + "'>");
    }
    return true;
}