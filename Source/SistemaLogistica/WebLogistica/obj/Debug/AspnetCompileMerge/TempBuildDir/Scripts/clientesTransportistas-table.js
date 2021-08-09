$(document).ready(function () {

    var oTable = $('#clientesTransportistasDataTable').dataTable({
        "scrollY": 400,
        "order": [[1, "asc"]],
        "idisplayLength": 25,
        "iDisplayStart": 0,
        "iDisplayEnd": 25,
        "aLengthMenu": [[25,100,1000], [25,100,1000]],
        "bServerSide": true,
        "sAjaxSource": "/ClientesTransportistas/AjaxHandlerClientesTransportistas",
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
            { "sWidth": "15%", "aTargets": 0 },
            { "sWidth": "35%", "aTargets": 1 },
            { "sWidth": "15%", "aTargets": 2 },
            { "sWidth": "35%", "aTargets": 3 },

        ],
        "aoColumns": [
            {
                "sName": "IdCliente",
                "sTitle": "ID Cli.",
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
                "sName": "IdTransportista",
                "sTitle": "ID Trans.",
                "bSortable": true,
                "bSearchable": true,
            },
            {
                "sName": "DescTransportista",
                "sTitle": "Transportista",
                "bSortable": true,
                "bSearchable": true,
            }
        ],
        "dom": 'Blfrtip',
        "buttons": [
            {
                extend: 'copyHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-files-o"></i> Copy',
                exportOptions: { modifier: { page: 'all', search: 'none' } }
            },
            {
                extend: 'excelHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-file-excel-o"></i> Excel',
                title: 'Cliente-Transportistas',
                footer: true,
                exportOptions: { modifier: { page: 'all', search: 'none' } }
            },
            {
                extend: 'pdfHtml5',
                title: 'Clientes-Transportistas',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-file-pdf-o"></i> Pdf',
                title: 'Cliente-Transportistas',
                exportOptions: { modifier: { page: 'all', search: 'none' } }
            },
            {
                extend: 'csvHtml5',
                title: 'Clientes-Transportistas',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-table"></i> CSV',
                title: 'Cliente-Transportistas',
                exportOptions: { modifier: { page: 'all', search: 'none' } }
            },
            {
                extend: 'print',
                title: 'Clientes-Transportistas',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-print"></i> Print',
                exportOptions: { modifier: { page: 'all', search: 'none' } }
            }
        ]
    });
});

function SwitchTransportistas() {
    var url = "/ClientesTransportistas/SwitchView";
    $('#modal-container').load(url);
    $('#modal-container').modal('show');
}