$(document).ready(function () {

    $("#adv-search").on('submit', function (e) {

        e.preventDefault();


        var fechI = this['altfech_ini'].value;
        var fechF = this['altfech_fin'].value;

        /*Regla 1 para la validadcion de al menos un campo*/
        if (fechI == '' && fechF == '' && $("#idCategoria").val() == '' &&
            $("#idSubCat").val() == '' && $("#idRegion").val() == '') {
            $("#alerta").empty();
            $("#alerta").append('<strong>Ingrese al menos un campo!</strong>').show();
            return;
        } else {

            var valor = $("#alerta").children('strong');
            if (valor.length > 0) {

                valor.remove();

            }

            $("#alerta").hide();

        }
        /*Regla 2 : Validacion para las fechas*/
        if (fechF != '' && fechI != '') {


            if (new Date(fechF.replace(/(\d{4})-(\d{2})-(\d{2})/, "$2/$3/$1")) < new Date(fechI.replace(/(\d{4})-(\d{2})-(\d{2})/, "$2/$3/$1"))) {
                $("#alerta").empty();
                $("#alerta").append('<strong>Fecha hasta no puede ser menor que fecha desde</strong>').show();
                return;
            } else {
                var valor = $("#alerta").children('strong');
                if (valor.length > 0) {

                    valor.remove();

                }

                $("#alerta").hide();

            }

        }

        this.submit();


    });
});




//Obtiene subcategoria

function cambioCat() {

    fillCombo("idSubCat", $("#idCategoria").val(), "/Home/Subcategorias", "Subcategorias");

}

function fillCombo(idCombo, value, linkUrl, optlabel) {

    $("#" + idCombo).empty();
    $("#" + idCombo).append("<option value=''>" + optlabel + "</option>")


    var depId = parseInt(value);
    if (isNaN(depId)) return;
    $.ajax({
        url: linkUrl,
        type: 'POST',
        data: { idCategoria: depId },
        dataType: 'json',
        success: function (data) {
            if (isNaN(data.length)) return;

            $.each(data, function (k, v) {
                $("#" + idCombo).append("<option value='" + v.id + "'>" + v.nombre + "</option>");
            });
        },
        error: function () {
            alert("Error :(");
        }
    });
}


//Funciones para las busquedas
function busqueda() {


    $("#search").submit();
}




function busquedaAdv() {

    $("#adv-search").submit();

}
