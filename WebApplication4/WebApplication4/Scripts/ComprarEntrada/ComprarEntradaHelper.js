function calculaPromo() {
    var idBanco = parseInt($('#idBanco').val());
    var tipoTarjeta = parseInt($('#idTipoTarjeta').val());
    var eventosBTV = document.getElementsByName('eventosCarrito');
    var subtotalBTV = document.getElementsByName('subtotalCarrito');
    var arrRazon = [];
    var eventosCarritos = [];
    var promocionCarritos = [];
    var descuentoCarritos = [];
    var subtotalCarritos = [];
    for (var i = 0; i < eventosBTV.length ; i++) {
        var codEvento = eventosBTV[i].value;
        eventosCarritos.push(codEvento);
        subtotalCarritos.push(subtotalBTV[i].value);
        $.ajax({
            url: '/Services/Descuento/',
            data: { codEvento: codEvento, idBanco: idBanco, tipoTarjeta: tipoTarjeta },
            datatype: "json",
            async: false,
            success: function (data) {
                var obj = $.parseJSON(data);
                //si tenemos data, escribimos denuevo el valor
                var razon = "";
                if (parseInt(obj.Descuento) == -1) {
                    razon = "- No cuenta con promocion";
                } else {
                    razon = "- Descuento de " + obj.Descuento + "%.";
                }
                arrRazon.push(razon);
                descuentoCarritos.push(obj.Descuento);
                promocionCarritos.push(obj.IdPromocion);
            },
            error: function () {
                alert("Error :(");
            }
        });
    }
    //relleno denuevo el esto :v
    $('#listaEventoPromocion').empty();
    var descuentoTotal = 0;
    for (var i = 0; i < arrRazon.length; i++) {
        var inputEventos = document.createElement("input");
        inputEventos.setAttribute("type", "hidden");
        inputEventos.setAttribute("id", "eventosCarrito");
        inputEventos.setAttribute("name", "eventosCarrito");
        inputEventos.setAttribute("value", eventosCarritos[i]);

        var inputPromociones = document.createElement("input");
        inputPromociones.setAttribute("type", "hidden");
        inputPromociones.setAttribute("id", "promocionesCarrito");
        inputPromociones.setAttribute("name", "promocionesCarrito");
        inputPromociones.setAttribute("value", promocionCarritos[i]);

        var inputSubtotal = document.createElement("input");
        inputSubtotal.setAttribute("type", "hidden");
        inputSubtotal.setAttribute("id", "subtotalCarrito");
        inputSubtotal.setAttribute("name", "subtotalCarrito");
        var subtotalconDescuento = 0;
        if (descuentoCarritos[i] != -1) {
            subtotalconDescuento += parseInt(subtotalCarritos[i]) * descuentoCarritos[i] / 100;
        }
        descuentoTotal += subtotalconDescuento;
        inputSubtotal.setAttribute("value", subtotalCarritos[i]);

        var razonP = document.createElement("p");
        razonP.setAttribute("style", "margin-left:2em");
        var texto = document.createTextNode(arrRazon[i]);
        razonP.appendChild(texto);
        $('#listaEventoPromocion').append(inputEventos);
        $('#listaEventoPromocion').append(inputPromociones);
        $('#listaEventoPromocion').append(inputSubtotal);
        $('#listaEventoPromocion').append(razonP);
    }
    //actualizamos el nuevo total
    var importe = parseFloat($('#Importe').val());
    var montoPagar = importe - descuentoTotal;
    $('#Descuento').val(descuentoTotal);
    $('#MontoPagar').val(montoPagar);
}

function llenaArreglo() {
    var nombreLista1 = "idEventos";
    var nombreLista2 = "idPromociones";
    var promocionesBTV = document.getElementsByName('promocionesCarrito');
    var eventosBTV = document.getElementsByName('eventosCarrito');
    for (var i = 0; i < promocionesBTV.length; i++) {
        var idProm = promocionesBTV[i].value;
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista2 + "[" + (i) + "]' value='" + idProm + "'>");
    }
    for (var i = 0; i < eventosBTV.length; i++) {
        var idEvento = eventosBTV[i].value;
        $("#formPost").prepend("<input type='hidden' name='" + nombreLista1 + "[" + (i) + "]' value='" + idEvento + "'>");
    }
    return true;
}