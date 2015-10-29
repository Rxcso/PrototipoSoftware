function myFunction(x) {
    if (document.getElementsByClassName("trselected").length > 0) {
        var element = document.getElementsByClassName("trselected");
        //element.item[0].cells[0].innerText;
        element[0].className = "";
    }
    //var tabla = document.getElementById("mytable");
    //alert(x);
    //var y = x.getCells();
    var elem = document.getElementById("idCategoria");
    elem.value = x.cells[0].innerHTML;
    //alert(x.cells[0].innerHTML);
    //ViewBag.P = x.cells[0].innerHTML;
    x.className = "trselected";

    //<a class="btn btn-primary" href="javascript:document.getElementsByClassName('trselected').submit()">Borrar<span class="glyphicon glyphicon-pencil"></span></a>
    //idProm = x.codp;
}
function borrar() {
    var categoria = $('#idCategoria').val();
    var el = document.getElementById(categoria);
    var row = $(el).closest('tr');
    //alert(evento);
    //alert(promocion);
    if (categoria != "" && categoria != null) {
        $.ajax({
            url: '/Categoria/Delete/',
            data: { categoria: categoria },
        success: function (data) {
            //alert("Eliminado");
            $('#eliminar').modal('hide');
            row.remove();
            window.location.href = '/Categoria/Index';
        },
        error: function () {
            alert("Error :(");
        }
    });
} else {
        alert("Seleccione una Categoria ");
        evento = "";
    }
}
function redirigeCategoria() {
    var categoria = $('#idCategoria').val();
    if (categoria != "") {
        window.location.href = '/Categoria/Edit?categoria=' + categoria;
    } else {
        alert("Seleccione una Categoria");
        evento = "";
    }
    //$.post('/Regalo/Edit/' + regalo);
}    
function modificar() {
    var categoria = $('#idCategoria').val();
    //var el = document.getElementById(regalo);
    //var row = $(el).closest('tr');
    //alert(evento);
    //alert(promocion);
    $.ajax({
        url: '/Categoria/Edit/',
        data: { categoria: categoria },
    success: function (data) {
        //alert("Eliminado");
        //row.remove();
    },
    error: function () {
        alert("Error :(");
    }
});
}
function closewindowEli() {
    $('#eliminar').modal('hide');
}
function closewindowAct() {
    $('#activar').modal('hide');
}

function activarArbol() {
    var categoria = $('#idCategoria').val();
    var el = document.getElementById(categoria);
    var row = $(el).closest('tr');
    //alert(evento);
    //alert(promocion);            
    if (categoria != "" && categoria != null) {
        $.ajax({
            url: '/Categoria/ActivateTree/',
            data: { categoria: categoria },
            success: function (data) {
                //alert("Eliminado");
                $('#opcionActivar').modal('hide');
                $('#activar').modal('hide');
                row.add();
                window.location.href = '/Categoria/Index';
            },
            error: function () {
                alert("Error :(");
            }
        });
    } else {
        alert("Seleccione una Categoria ");
        evento = "";
    }
}
function activarRegistro() {
    var categoria = $('#idCategoria').val();    
    var el = document.getElementById(categoria);
    var row = $(el).closest('tr');
    //alert(evento);
    //alert(promocion);            
    if (categoria != "" && categoria != null) {
        $.ajax({
            url: '/Categoria/Activate/',
            data: { categoria: categoria },
            success: function (data) {
                //alert("Eliminado");
                $('#opcionActivar').modal('hide');
                $('#activar').modal('hide');
                row.add();
                window.location.href = '/Categoria/Index';
            },
            error: function () {
                alert("Error :(");
            }
        });
    } else {
        alert("Seleccione una Categoria ");
    evento = "";
    }
}

function busca() {
    var categoria = $('#idBusq2').val();

    $.ajax({
        url: '/Categoria/Search2/',
        data: { categoria: categoria },
        success: function (data) {
            window.location.href = '/Categoria/Index';

        },
        error: function () {
            alert("Error :(");
        }
    });


}