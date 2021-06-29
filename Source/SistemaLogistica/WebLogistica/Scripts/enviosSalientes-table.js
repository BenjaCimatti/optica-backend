$(document).ready(function () {
    var oTable = $('#enviosSalientesDataTable').dataTable({
        "searching": true,
        "scrollY": 400,
        "order": [[1, "asc"]],
        "idisplayLength": 300,
        "iDisplayStart": 0,
        "iDisplayEnd": 300,
        "aLengthMenu": [[300, 1000], [300, 1000]],
        "bServerSide": true,
        "sAjaxSource": "/Envios/AjaxHandlerSalientes",
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
            { "sWidth": "5%", "aTargets": 0 },
            { "sWidth": "30%", "aTargets": 1 },
            { "sWidth": "25%", "aTargets": 2 },
            { "sWidth": "10%", "aTargets": 3 },
            { "sWidth": "30%", "aTargets": 4 },
        ],
        "aoColumns": [
            {
                "sClass": "center",
                "sName": "IdEnvio",
                "sTitle": "Envio",
                "bSortable": true,
                "bSearchable": false,
            },
            {
                "sName": "DescTransportista",
                "sTitle": "Transportista",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sName": "DescCLiente",
                "sTitle": "Cliente",
                "bSortable": false,
                "bSearchable": false,
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
                "sName": "Observaciones",
                "sTitle": "Observaciones",
                "bSortable": false,
                "bSearchable": false,
            }
        ],
        "dom": 'lrtip',
        "buttons": [
            {
                extend: 'copyHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-files-o"></i> Copy',
                exportOptions: {
                    modifier: { page: 'all', search: 'none' }
                }
            },
            {
                extend: 'excelHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-file-excel-o"></i> Excel',
                title: 'Listado de Envios',
                footer: true,
                exportOptions: {
                    modifier: { page: 'all', search: 'none' }
                }
            },
            {
                extend: 'print',
                title: 'Listado de Envios',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-print"></i> Print',
                exportOptions: {
                    modifier: { page: 'all', search: 'none' }
                }
            }
        ]
    });

    $('#ddlTransportistas').on('change', function () {
        oTable.search(this.value).draw();
    });
});