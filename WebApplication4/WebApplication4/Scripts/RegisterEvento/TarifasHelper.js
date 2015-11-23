function agregaFila() {
    var nZonas = parseInt($('#nZonas').val());
    var table = document.getElementById("tarifario").getElementsByTagName('tbody')[0];
    var row = table.insertRow();
    row.id = nZonas;
    row.setAttribute("data-zona", "0");
    var nCeldas = parseInt($('#nHeaders').val());
    var cell0 = row.insertCell(0);
    cell0.innerHTML = (nZonas + 1);
    var cell1 = row.insertCell(1);
    cell1.innerHTML = '<input type="text" class="form-control" required/>';
    for (var i = 2; i < nCeldas; i++) {
        var cell = row.insertCell(i);
        cell.innerHTML = '<input type="number" class="form-control" name="cantidad" min = "0" required/>';
    }
    var cell = row.insertCell();
    cell.innerHTML = '<input type="radio" name="groupTarifas" value="' + (parseInt(row.id)) + '">';
    cell.align = "center";
    $('#nZonas').val((nZonas + 1));
}
function quitarFila() {
    var fila = $('input[name="groupTarifas"]:checked').val();
    var row = document.getElementById(parseInt(fila));
    row.parentNode.removeChild(row);
}
function agregaTarifas() {
    var tableBTV = document.getElementById("tarifario");
    if (tableBTV.rows.length == 1) { alert("Tabla vacía. Ingrese datos."); return false; }
    var nCeldas = parseInt($('#nHeaders').val());
    var nombreLista = "ListaZEM";
    var nombreSubLista = "ListaTarifas";
    for (var i = 1, row; row = tableBTV.rows[i]; i++) {
        var nombreZona = row.cells[1].children[0].value;
        var aforo = row.cells[nCeldas - 1].children[0].value;
        var zona = row.getAttribute("data-zona");
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].Nombre' value='" + nombreZona + "'>");
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].Aforo' value='" + aforo + "'>");
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "].Id' value='" + zona + "'>");
        for (var j = 2; j < nCeldas - 1; j++) {
            var precio = row.cells[j].children[0].value;
            $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i - 1) + "]." + nombreSubLista + "[" + (j - 2) + "].Precio' value='" + precio + "'>");
        }
    }
}