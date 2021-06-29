$(document).ready(function () {

    var oTable = $('#enviosDataTable').dataTable({
        "order": [[2, "asc"]],
        "bServerSide": true,
        "sAjaxSource": "/Home/AjaxHandler",
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
            { "sWidth": "10%", "aTargets": 1 },
            { "sWidth": "22%", "aTargets": 2 },
            { "sWidth": "22%", "aTargets": 3 },
            { "sWidth": "10%", "aTargets": 4 },
            { "sWidth": "12%", "aTargets": 5 },
            { "sWidth": "12%", "aTargets": 6 },
        ],
        "aoColumns": [
            {
                "sClass": "center",
                "sName": "IdEnvio",
                "sTitle": "Acciones",
                "bSearchable": false,
                "bSortable": false,
                "mRender": function (data, type, oObj) {
                    var IdEnvio = oObj[0];
                    return '<a href=\"Envio/Details/' + IdEnvio + '\"><img border="0" alt="W3Schools" src="/Content/images/edit_icon.png" width="25" height="25" style="margin: 0px 10px"></a><a href=\"Envio/Delete/' + IdEnvio  + '\"><img border="0" alt="W3Schools" src="/Content/images/delete_icon.png" width="25" height="25"></a>';
                }
            },
            {
                "sClass": "center",
                "sName": "DescEnvio",
                "sTitle": "Envio",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sName": "DescCLiente",
                "sTitle": "Cliente",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sName": "DescTransportista",
                "sTitle": "Transportista",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sClass": "center",
                "sName": "DescEstado",
                "sTitle": "Estado",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sClass": "center",
                "sName": "FechaCarga",
                "sTitle": "Carga",
                "bSortable": true,
                "bSearchable": false,
            },
            {
                "sClass": "center",
                "sName": "FechaEnvio",
                "sTitle": "Envio",
                "bSortable": true,
                "bSearchable": false,
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
                    columns: [1, 2, 3, 4, 5, 6]
                }
            },
            {
                extend: 'excelHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-file-excel-o"></i> Excel',
                title: 'Envios',
                footer: true,
                exportOptions: { modifier: { page: 'all', search: 'none' },
                columns: [1, 2, 3, 4, 5, 6] }
            },
            {
                extend: 'print',
                title: 'Envios',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-print"></i> Print',
                exportOptions: {
                    modifier: { page: 'all', search: 'none' },
                    columns: [1, 2, 3, 4, 5, 6]
                }
            }
        ]
    });
});