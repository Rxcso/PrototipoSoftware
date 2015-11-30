document.addEventListener('DOMContentLoaded', function () {
    var arr = ["efectivo", "tarjeta", "mixto"];
    var mod = parseInt(document.getElementById("modalidad").value);
    if (mod == 1) {
        loadEfectivo(arr[0], arr[1], arr[2]);
    }
    if (mod == 2) {
        loadTarjeta(arr[1], arr[0], arr[2]);
    }
    if (mod == 3) {
        if (parseInt($('#mixto input#MontoTar').val())) {
            loadDatosTarjeta();
        }
        loadMixto(arr[2], arr[0], arr[1]);
    }
}, false);

function llena1() {
    //tipo de cambio
    var tipoCambio = parseFloat($('#tipoCambioMoneda').val());
    //valor de los inputs
    var inputs = $('#efectivo input');
    var montoSoles = inputs[3].value;
    var montoDolares = inputs[4].value;
    var montoPagar = inputs[2].value;
    //chequeo a nivel de texto
    if (montoSoles == "") {
        alert("El monto en soles debe tener un valor");
        inputs[3].value = "0";
        return false;
    }
    if (montoDolares == "") {
        alert("El monto en dolares debe tener un valor");
        inputs[4].value = "0";
        return false;
    }
    //en numeros
    var montoSoles = parseFloat(montoSoles);
    var montoDolares = parseFloat(montoDolares);
    var montoPagar = parseFloat(montoPagar);
    if (montoSoles < 0) {
        alert("El monto en soles debe ser mayor o igual a 0");
        inputs[3].value = "0";
        return false;
    }
    if (montoDolares < 0) {
        alert("El monto en dolares debe ser mayor o igual a 0");
        inputs[4].value = "0";
        return false;
    }
    //la suma de los montos debe ser mayor que le monto pagar
    if (montoSoles + montoDolares * tipoCambio >= montoPagar) {
        var vuelto = montoSoles + montoDolares * tipoCambio - montoPagar;
        inputs[6].value = Math.floor(vuelto * 100) / 100;
    } else {
        alert("La suma de los montos debe ser mayor al monto a pagar");
        inputs[6].value = "0";
        return false;
    }
}

function llena3() {
    var montoefe = $("#mixto input#MontoEfe").val();
    var montoDolares = $('#mixto input#MontoDolares').val();
    var cambio = parseFloat(montoDolares) * parseFloat($('#tipoCambioMoneda').val());
    var montopagar = $("#mixto input#MontoPagar").val();
    if (parseFloat(montoefe) < 0) {
        $("#mixto input#MontoEfe").val(0);
        alert("El monto en efectivo debe ser mayor o igual a cero");
        return false;
    }
    if (parseFloat(montoDolares) < 0) {
        $('#mixto input:#MontoDolares').val(0);
        alert("El monto de efectivo en dólares debe ser mayor o igual a cero");
        return false;
    }

    montopagar = $("#mixto input#MontoPagar").val();
    if (parseFloat(montoefe) + cambio > parseFloat(montopagar)) {
        var vuelto = cambio + parseFloat(montoefe) - parseFloat(montopagar);
        $('#mixto input#Vuelto').val(Math.floor(vuelto * 100 / 100));
        $('#mixto input#MontoTar').val(0);
    } else {
        var montotar = parseFloat(montopagar) - cambio - parseFloat(montoefe);
        $("#mixto input#MontoTar").val(Math.floor(montotar * 100) / 100);
        $('#mixto input#Vuelto').val(0);
    }
    montoTar = $('#mixto input#MontoTar').val();
    if (parseFloat(montoTar) == 0) {
        document.getElementById("AnioVen").disabled = true;
        document.getElementById("Mes").disabled = true;
        document.getElementById("CodCcv").readOnly = true;
        document.getElementById("NumeroTarjeta").readOnly = true;
        document.getElementById("idTipoTarjeta").disabled = true;
        document.getElementById("idBanco").disabled = true;
    }
    else {
        document.getElementById("AnioVen").disabled = false;
        document.getElementById("Mes").disabled = false;
        document.getElementById("CodCcv").readOnly = false;
        document.getElementById("NumeroTarjeta").readOnly = false;
        document.getElementById("idTipoTarjeta").disabled = false;
        document.getElementById("idBanco").disabled = false;
    }
}

function calculaPromo() {
    var metodoSelecionado = $('input[name="TipoPago"]:checked').val();
    if (parseInt(metodoSelecionado) == 2) {
        var montoefe = $("#tarjeta input#MontoEfe").val();
        var montoDolares = $('#tarjeta input#MontoDolares').val();
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
                        //$('#dsctInicial').val(0);
                    } else {
                        razon = "- Descuento de " + obj.Descuento + "%.";
                        //$('#dsctInicial').val(0);
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
        $('#tarjeta input#Descuento').val(descuentoTotal);
        $('#tarjeta input#MontoPagar').val(montoPagar);
        var montoTar = montoPagar - parseFloat($('#tarjeta input#MontoEfe').val());
        $('#tarjeta input#MontoTar').val(montoTar);
    }
}

//escribe el arreglo
function escribeArreglo(nombreLista, id, i) {
    $("#formPost").prepend("<input type='hidden' name='" + nombreLista + "[" + (i) + "]' value='" + id + "'>");
}

//al hacer click en siguiente, debe de validar que los montos sean correctos
function verificarMontos(metodo) {
    if (metodo == 1) {
        //MontoEfe, MontoDolares
        var montoEfectivo = parseFloat($('#efectivo input#MontoEfe').val());
        var montoDolares = parseFloat($('#efectivo input#MontoDolares').val());
        var tipoCambio = parseFloat($('#tipoCambioMoneda').val());
        var montoPagar = parseFloat($('#efectivo input#MontoPagar').val());
        if (montoEfectivo + montoDolares * tipoCambio >= montoPagar) {
            return true;
        }
        alert("Monto en dolares y soles son menores que el monto a pagar");
        return false;
    }
    if (metodo == 3) {
        //MontoEfe, MontoDolares
        var montoEfectivo = parseFloat($('#mixto input#MontoEfe').val());
        var montoDolares = parseFloat($('#mixto input#MontoDolares').val());
        var tarjeta = parseFloat($('#mixto input#MontoTar').val());
        var tipoCambio = parseFloat($('#tipoCambioMoneda').val());
        var montoPagar = parseFloat($('#mixto input#MontoPagar').val());
        if (tarjeta + montoEfectivo + montoDolares * tipoCambio >= montoPagar) {
            return true;
        }
        alert("Monto en dolares y soles son menores que el monto a pagar");
        return false;
    }
    return true;
}

//llena el arreglo auxiliar para los eventos y promociones que estan guardadas
function llenaArreglo() {
    var metodoSelecionado = parseFloat($('input[name="TipoPago"]:checked').val());
    if (verificarMontos(metodoSelecionado)) {
        var nombreLista1 = "idEventos";
        var nombreLista2 = "idPromociones";
        var promocionesBTV = 1;
        var eventosBTV = 1;
        //si es un pago mixto no hay promociones - no afectan promociones
        //es pago solo en efectivo
        if (metodoSelecionado == 1) {
            promocionesBTV = document.getElementsByName('promoEfectivo');
            eventosBTV = document.getElementsByName('eventoEfectivo');
        } else {
            //es pago solo con tarjeta
            promocionesBTV = document.getElementsByName('promocionesCarrito');
            eventosBTV = document.getElementsByName('eventosCarrito');
        }
        //guardo los datos
        for (var i = 0; i < promocionesBTV.length; i++) {
            var idProm = promocionesBTV[i].value;
            var idEvento = eventosBTV[i].value;
            escribeArreglo(nombreLista2, idProm, i);
            escribeArreglo(nombreLista1, idEvento, i);
        }
        return true;
    }
    return false;
}

function desactiva(item) {
    var inputs = $('#' + item + ' input');
    for (var i = 1 ; i < inputs.length ; i++) {
        inputs[i].disabled = true;
    }
}

function loadEfectivo(id1, id2, id3) {
    var inputs = $('#' + id1 + ' input');
    inputs[1].disabled = false;
    inputs[1].readOnly = true;
    inputs[2].disabled = false;
    inputs[2].readOnly = true;
    inputs[3].disabled = false;
    inputs[4].disabled = false;
    inputs[5].disabled = false;
    inputs[5].readOnly = true;
    inputs[6].disabled = false;
    inputs[6].readOnly = true;
    desactiva(id2);
    desactiva(id3);
}

function loadDatosTarjeta() {
    document.getElementById("AnioVen").disabled = false;
    document.getElementById("Mes").disabled = false;
    document.getElementById("CodCcv").readOnly = false;
    document.getElementById("NumeroTarjeta").readOnly = false;
    document.getElementById("idTipoTarjeta").disabled = false;
    document.getElementById("idBanco").disabled = false;
}

function loadTarjeta(id1, id2, id3) {
    var inputs = $('#' + id1 + ' input');
    loadDatosTarjeta();
    inputs[1].disabled = false;
    inputs[1].readOnly = true;
    inputs[2].disabled = false;
    inputs[2].readOnly = true;
    inputs[3].disabled = false;
    inputs[3].readOnly = true;
    inputs[4].disabled = false;
    inputs[4].readOnly = true;
    inputs[5].disabled = false;
    inputs[5].readOnly = true;
    inputs[6].disabled = false;
    inputs[6].readOnly = true;
    desactiva(id2);
    desactiva(id3);
}

function loadMixto(id1, id2, id3) {
    var inputs = $('#' + id1 + ' input');
    inputs[1].disabled = false;
    inputs[1].readOnly = true;
    inputs[2].disabled = false;
    inputs[2].readOnly = true;
    inputs[3].disabled = false;
    inputs[4].disabled = false;
    inputs[5].disabled = false;
    inputs[5].readOnly = true;
    inputs[6].disabled = false;
    inputs[6].readOnly = true;
    desactiva(id2);
    desactiva(id3);
}
//funcion que cambia la seleccion de los metodos de pago
function cambioPago() {
    var metodoSelecionado = $('input[name="TipoPago"]:checked').val();
    $('#modalidad').val(metodoSelecionado);
    var arr = ["efectivo", "tarjeta", "mixto"];
    if (parseInt(metodoSelecionado) == 1) {
        loadEfectivo(arr[0], arr[1], arr[2]);
    }
    if (parseInt(metodoSelecionado) == 2) {
        loadTarjeta(arr[1], arr[0], arr[2]);
    }
    if (parseInt(metodoSelecionado) == 3) {
        loadMixto(arr[2], arr[1], arr[0]);
    }
}