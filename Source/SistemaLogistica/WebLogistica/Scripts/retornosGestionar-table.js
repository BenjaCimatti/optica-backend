$(document).ready(function () {
    var rows_selected = [];
    var oTable = $('#retornosGestionarDataTable').dataTable({
        "scrollY": 400,
        "order": [[0, "asc"]],
        "bServerSide": true,
        "sAjaxSource": "/Retornos/AjaxHandlerRetornos",
        "bProcessing": true,
        "bAutoWidth": false,
        "oLanguage": {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",
            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            },
            "oAria": {
                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
            }
        },
        "aoColumnDefs": [
            { "sWidth": "10%", "aTargets": 0 },
            { "sWidth": "40%", "aTargets": 1 },
            { "sWidth": "20%", "aTargets": 2 },
            { "sWidth": "20%", "aTargets": 3 },
            { "sWidth": "10%", "aTargets": 4 }
        ],
        "aoColumns": [
            {
                "sClass": "text-center",
                "sName": "IdEnvio",
                "sTitle": "Envio",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sClass": "text-left",
                "sName": "DescCLiente",
                "sTitle": "Cliente",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sClass": "text-left",
                "sName": "DescTransportista",
                "sTitle": "Transportista",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sClass": "text-center",
                "sName": "FechaEnvio",
                "sTitle": "Fecha Envio",
                "bSortable": true,
                "bSearchable": false,
            },
            {
                "sClass": "text-center",
                "sName": "Recibido",
                "sTitle": "Recibido",
                "bSortable": false,
                "bSearchable": false,
                "mRender": function (data, type, full, meta) {
                    return '<input type="checkbox" name="id[' + $('<div/>').text(data).html() + ']" value="' + $('<div/>').text(data).html() + '">';
                }
            }
        ],
        "dom": 'Blfrtip',
        "buttons": [
            {
                extend: 'copyHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-files-o"></i> Copy',
                exportOptions: {
                    modifier: { page: 'all', search: 'none' },
                    columns: [0, 1, 2, 3]
                }
            },
            {
                extend: 'excelHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-file-excel-o"></i> Excel',
                title: 'EnviosPendientes',
                footer: true,
                exportOptions: {
                    modifier: { page: 'all', search: 'none' },
                    columns: [0, 1, 2, 3]
                }
            },
            {
                extend: 'print',
                title: 'EnviosPendientes',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-print"></i> Print',
                exportOptions: {
                    modifier: { page: 'all', search: 'none' },
                    columns: [0, 1, 2, 3]
                }
            }
        ]
    });

    $('[data-dismiss=modal]').on('click', function (e) {
        var $t = $(this),
            target = $t[0].href || $t.data("target") || $t.parents('.modal') || [];

        $(target)
            .find("input,textarea")
            .val('')
            .end()
            .find("input[type=checkbox], input[type=radio]")
            .prop("checked", "")
            .end();
    })

    $('#recibirDevoluciones').on('click', function (e) {
        e.preventDefault();

        var data = JSON.stringify(oTable.$('input[type="checkbox"]').serializeArray());

        if (data != "[]") {

            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: '/Retornos/Recibir',
                data: data,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).done(function (response) {
                if (response == "Recibidos") {
                    BootstrapDialog.show({
                        title: 'Confirmación',
                        message: "Envios Recibidos!!",
                        buttons: [{
                            label: 'Cerrar',
                            action: function (dialogItself) {
                                dialogItself.close();
                            }
                        }]
                    });
                    var table = $('#retornosGestionarDataTable').DataTable();
                    table.ajax.reload();
                }
                else {
                    if (response == "NoData") {
                        BootstrapDialog.show({
                            title: 'Seleccion',
                            message: "Debe seleccionar envios...!!",
                            buttons: [{
                                label: 'Cerrar',
                                action: function (dialogItself) {
                                    dialogItself.close();
                                }
                            }]
                        });
                    }
                    else {
                        BootstrapDialog.show({
                            title: "Error del Sistema",
                            message: "Contacte a un Administrador!!",
                            buttons: [{
                                label: 'Cerrar',
                                action: function (dialogItself) {
                                    dialogItself.close();
                                }
                            }]
                        });
                    }
                }
            });
        }
        else {
            BootstrapDialog.show({
                title: 'Seleccion',
                message: "Debe seleccionar envios...!!",
                buttons: [{
                    label: 'Cerrar',
                    action: function (dialogItself) {
                        dialogItself.close();
                    }
                }]
            });
        }

    })
});