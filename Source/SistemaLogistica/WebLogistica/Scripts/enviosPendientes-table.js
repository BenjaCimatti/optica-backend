$(document).ready(function () {
    var rows_selected = [];
    var oTable = $('#enviosPendientesDataTable').dataTable({
        "scrollY": 400,
        "order": [[2, "asc"]],
        "bServerSide": true,
        "sAjaxSource": "/Envios/AjaxHandlerPendientes",
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
            { "sWidth": "12%", "aTargets": 0 },
            { "sWidth": "10%", "aTargets": 1 },
            { "sWidth": "20%", "aTargets": 2 },
            { "sWidth": "20%", "aTargets": 3 },
            { "sWidth": "13%", "aTargets": 4 },
            { "sWidth": "20%", "aTargets": 5 },
            { "sWidth": "5%", "aTargets": 6 }
        ],
        "aoColumns": [
            {
                "sClass": "text-center",
                "sName": "IdEnvio",
                "sTitle": "Acciones",
                "bSearchable": false,
                "bSortable": false,
                "mRender": function (data, type, oObj) {
                    var IdEnvio = oObj[0];
                    return '<a href="#" class="btn btn-primary edit-envio" onclick=EditEnvio(' + IdEnvio + '); idEnvio="' + IdEnvio + '"><img border="0" alt="Editar" src="/Content/images/edit_icon.png" width="25" height="25" ></a>&nbsp;<a href="#" class="btn btn-danger" onclick=DeleteEnvio(' + IdEnvio + '); ><img border="0" alt="Borrar" src="/Content/images/delete_icon.png" width="25" height="25"></a>';
                }
            },
            {
                "sClass": "text-center",
                "sName": "DescEnvio",
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
                "sName": "FechaCarga",
                "sTitle": "Carga",
                "bSortable": true,
                "bSearchable": false,
            },
            {
                "sClass": "text-left",
                "sName": "Observaciones",
                "sTitle": "Observaciones",
                "bSortable": false,
                "bSearchable": true,
            },
            {
                "sClass": "text-center",
                "sName": "Enviar",
                "sTitle": "Enviar",
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
                    columns: [1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'excelHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-file-excel-o"></i> Excel',
                title: 'EnviosPendientes',
                footer: true,
                exportOptions: { modifier: { page: 'all', search: 'none' },
                columns: [1, 2, 3, 4, 5] }
            },
            {
                extend: 'print',
                title: 'EnviosPendientes',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-print"></i> Print',
                exportOptions: {
                    modifier: { page: 'all', search: 'none' },
                    columns: [1, 2, 3, 4, 5]
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

    $('#liberarEnvios').on('click', function (e) {
        e.preventDefault();

        var data = JSON.stringify(oTable.$('input[type="checkbox"]').serializeArray());

        if (data != "[]") {

            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: '/Envios/Liberar',
                data: data,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).done(function (response) {
                if (response == "Liberados") {
                    BootstrapDialog.show({
                        title: 'Confirmación',
                        message: "Envios Liberados!!",
                        buttons: [{
                            label: 'Cerrar',
                            action: function (dialogItself) {
                                dialogItself.close();
                            }
                        }]
                    });
                    var table = $('#enviosPendientesDataTable').DataTable();
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

function EditEnvio(IdEnvio) {
    var url = "/Envios/EditarView";
    var id = IdEnvio;
    $.get(url + '/' + id, function (data) {
        $('#modal-container').html(data);
        $('#modal-container').modal('show');
    });
}

function DeleteEnvio(IdEnvio) {
    var dialog = BootstrapDialog.confirm({
        title: 'Borrado de Envios Pendientes',
        message: "Seguro que desea borrar el Envio ID " + IdEnvio,
        closable: true,
        btnOKLabel: "Confirmar",
        cssClass: "big-title",
        callback: function (result) {
            if (result) {
                Delete(IdEnvio);
            } else {
                dialog.close();
            }
        }
    });
}

function Delete(IdEnvio) {
    var url = "/Envios/Delete";
    $.post(url, { ID: IdEnvio }, function (data) {
        if (data == "Deleted") {
            BootstrapDialog.show({
                title: 'Confirmación',
                message: "Envio Borrado!!",
                buttons: [{
                    label: 'Cerrar',
                    action: function (dialogItself) {
                        dialogItself.close();
                    }
                }]
            });
            var table = $('#enviosPendientesDataTable').DataTable();
            table.ajax.reload();
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
    });
}

function NuevoEnvio() {
    var url = "/Envios/NuevoEnvioView";
    $('#modal-container').load(url);
    $('#modal-container').modal('show');
}