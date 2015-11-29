function cambioDepartamento() {
    var depId = parseInt($("#idRegion").val());
    if (!isNaN(depId)) {
        var ddProv = $("#idProv");
        ddProv.empty();
        ddProv.append($("<option></option>").val("").html("Provincia"));
        $.ajax({
            url: '/Services/Provincia/',
            data: { depId: depId },
            datatype: "json",
            success: function (data) {
                var obj = $.parseJSON(data);
                $.each(obj, function (k, v) {
                    ddProv.append($("<option></option>").val(v.IdRegion).html(v.Nombre));
                });
            },
            error: function () {
                alert("Error :(");
            }
        });
    }
    ddProv.empty();
}
function cambioCategoria() {
    var catId = parseInt($("#idCategoria").val());
    if (!isNaN(catId)) {
        var ddProv = $("#idSubCat");
        ddProv.empty();
        ddProv.append($("<option></option>").val("").html("Subcategoria"));
        $.ajax({
            url: '/Services/Subcategoria/',
            data: { catId: catId },
            datatype: "json",
            success: function (data) {
                var obj = $.parseJSON(data);
                $.each(obj, function (k, v) {
                    ddProv.append($("<option></option>").val(v.IdSCategoria).html(v.Nombre));
                });
            },
            error: function () {
                alert("Error :(");
            }
        });
    }
    ddProv.empty();
}
function llenaOrg() {
    var nomb = $('input[name="groupO"]:checked').val();
    var ids = $('input[name="groupO"]:checked').attr('id');
    $("#organizadorNombre").val(nomb);
    $('#valOrganizador').val(ids);
    $('#modalBuscarOrganizador').modal('hide');
}
function llenaLocal() {
    var nomb = $('input[name="groupL"]:checked').val();
    var idl = $('input[name="groupL"]:checked').attr('id');
    var idProv = $('input[name="groupL"]:checked').data("prov");
    var idDep = $('input[name="groupL"]:checked').data("dep");
    var nProv = $('input[name="groupL"]:checked').data("nprov");
    var nombreProv = String(nProv);
    nombreProv = nombreProv.replace('<', '');
    nombreProv = nombreProv.replace('>', '');
    $('select#idRegion').val(idDep);
    var provincia = $('select#idProv');
    provincia.append($("<option></option>").val(idProv).html(nombreProv));
    provincia.val(idProv);
    $('select#idRegion').attr('disabled', 'disabled');
    $('select#idProv').attr('disabled', 'disabled');
    $('input#idRegion').val(idDep);
    $('input#idProv').val(idProv);
    $("#localNombre").val(nomb);
    $('#valLocal').val(idl);
    $('#modalBuscarLocal').modal('hide');
    $('#Direccion').val('');
}

function cambio() {
    $('select#idRegion').removeAttr('disabled');
    $('select#idProv').removeAttr('disabled');
    $('select#idRegion').val(0)
    $('select#idProv').val(0);
    $('input#idRegion').val(0);
    $('input#idProv').val(0)
    $("#localNombre").val('');
    $('#valLocal').val('');
}
