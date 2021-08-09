$(document).ready(function () {
    var currentDate = new Date()
    var day = currentDate.getDate()
    var month = currentDate.getMonth() + 1
    var year = currentDate.getFullYear()
    var d = day + "-" + month + "-" + year;
    var oTable = $('#enviosSalientesDataTable').dataTable({
        "searching": true,
        "paging": false,
        "scrollY": 350,
        "order": false,
        "idisplayLength": 300,
        "iDisplayStart": 0,
        "iDisplayEnd": 300,
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
            { "sWidth": "20%", "aTargets": 1 },
            { "sWidth": "40%", "aTargets": 2 },
            { "sWidth": "10%", "aTargets": 3 },
            { "sWidth": "30%", "aTargets": 4 },
            { "sWidth": "5%", "aTargets": 5 },
        ],
        "aoColumns": [
            {
                "sClass": "text-center",
                "sName": "IdEnvio",
                "sTitle": "Envio",
                "bSortable": false,
                "bSearchable": false,
            },
            {
                "sClass": "text-left",
                "sName": "DescTransportista",
                "sTitle": "Transportista",
                "bSortable": false,
                "bSearchable": true,
            },
            {
                "sClass": "text-left",
                "sName": "DescCLiente",
                "sTitle": "Cliente",
                "bSortable": false,
                "bSearchable": false,
            },
            {
                "sClass": "text-center",
                "sName": "FechaCarga",
                "sTitle": "Carga",
                "bSortable": false,
                "bSearchable": false,
            },
            {
                "sClass": "text-left",
                "sName": "Observaciones",
                "sTitle": "Observaciones",
                "bSortable": false,
                "bSearchable": false,
            },
            {
                "sClass": "text-center",
                "sName": "Contactos",
                "sTitle": "Contactos",
                "bSearchable": false,
                "bSortable": false,
                "mRender": function (data, type, oObj) {
                    var IdEnvio = oObj[0];
                    var Conractos = oObj[5];
                    if (Conractos > 0) {
                        return '<a href="#" class="btn btn-primary ver-envio" onclick=VerContactos(' + IdEnvio + '); idEnvio="' + IdEnvio + '"><img border="0" alt="Editar" src="/Content/images/view_icon.png" width="25" height="25" ></a>';
                    }
                    return "";
                }
            }
        ],
        "dom": 'Blrtip',
        "buttons": [
            {
                extend: 'copyHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-files-o"></i> Copy',
                exportOptions: {
                    modifier: { page: 'all', search: 'none' },
                    columns: [0, 1, 2, 3, 4]
                }
            },
            {
                extend: 'excelHtml5',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-file-excel-o"></i> Excel',
                title: 'Listado de Envios',
                footer: true,
                filename: function () {
                    var t = $('#ddlTransportistas').val();
                    return d + t + 'Envios';
                },
                exportOptions: {
                    modifier: { page: 'all', search: 'none' },
                    columns: [0, 1, 2, 3, 4]
                }
            },
            {
                extend: 'print',
                title: 'Listado de Envios',
                className: 'btn btn-dark rounded-0',
                text: '<i class="fa fa-fw fa-print"></i> Print',
                exportOptions: {
                    modifier: { page: 'all', search: 'none' },
                    columns: [0, 1, 2, 3, 4]
                }
            }
        ]
    });

    $('#ddlTransportistas').on('change', function () {
        var table = $('#enviosSalientesDataTable').DataTable();
        table.search($('#ddlTransportistas').val());
        table.draw();
    });
});

function VerContactos(IdEnvio) {
    var url = "/Envios/Contactado";
    var id = IdEnvio;
    $('#modal-container').load(url + '/' + id);
    $('#modal-container').modal('show');
}