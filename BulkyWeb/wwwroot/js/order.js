
var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    if ($.fn.DataTable.isDataTable('#orderData')) {
        $('#orderData').DataTable().destroy();
    }
    dataTable = $('#orderData').DataTable({
        "ajax": {url: '/admin/order/get'},
        "columns": [
            { data: 'id',"width" :"5%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "15%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "10%" },
            

            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/detail?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> </a>               
                    
                    </div>`
                },
                "width": "25%",
            },
        ]
    });
}
