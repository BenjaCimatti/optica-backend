$(document).ready(function () {
    var oTable = $('#enviosCompletadosDataTable').dataTable({
        "scrollY": 400,
        "order": [[2, "asc"]],
        "bServerSide": true,
        "sAjaxSource": "/Envios/AjaxHandlerCompletados",
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
            { "sWidth": "7%", "aTargets": 0 },
            { "sWidth": "10%", "aTargets": 1 },
            { "sWidth": "20%", "aTargets": 2 },
            { "sWidth": "20%", "aTargets": 3 },
            { "sWidth": "10%", "aTargets": 4 },
            { "sWidth": "13%", "aTargets": 5 },
            { "sWidth": "20%", "aTargets": 6 },
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
                    return '<a href="#" class="btn btn-primary ver-envio" onclick=VerEnvio(' + IdEnvio + '); idEnvio="' + IdEnvio + '"><img border="0" alt="Editar" src="/Content/images/view_icon.png" width="25" height="25" ></a>';
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
                "sClass": "text-center",
                "sName": "FechaEnvio",
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
                title: 'EnviosCompletados',
                footer: true,
                exportOptions: { modifier: { page: 'all', search: 'none' },
                    columns: [1, 2, 3, 4, 5, 6]
                }
            },
            {
                extend: 'print',
                title: 'EnviosCompletados',
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

function VerEnvio(IdEnvio) {
    var url = "/Envios/Completado";
    var id = IdEnvio;
    $('#modal-container').load(url + '/' + id);
    $('#modal-container').modal('show');
}