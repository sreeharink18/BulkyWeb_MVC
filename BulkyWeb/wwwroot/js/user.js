
var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    /*if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }*/
    dataTable = $('#tblData').DataTable({
        "ajax": {url: '/admin/user/get'},
        "columns": [
            { data: 'name',"width" :"15%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'company.name', "width": "15%" },
            { data: '', "width": "10%" },
           
           /* {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                     <a onClick=Delete('/admin/company/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>`
                },
                "width": "25%",
            },*/
        ]
    });
}
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire(
                $.ajax({
                    url: url,
                    type: 'Delete',
                    success: function (data) {
                        dataTable.ajax.reload();
                        toastr.success(data.errrorMessage);
                    }
                })
            )
        }
    })
}