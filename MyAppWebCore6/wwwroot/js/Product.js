let table; //= new DataTable('#productTable');

table = $('#productTable').DataTable({

    "ajax": {
        "url":"/Admin/Product/AllProducts"
    },
    "columns": [
        { "data": "name" },
        { "data": "description" },
        { "data": "price" },
        { "data": "category.name" },
        {
            "data": "id",
            "render": function (data) {
                return '<a href="/Admin/Product/CreateUpdate/' + data + '" class="btn btn-success">Update</a> ' +
                    '<a class="btn btn-danger" onclick="RemoveProduct(\'/Admin/Product/DeleteProduct/' + data + '\')">Delete</a>';
            }
        }
    ]
});

function RemoveProduct(url) {
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
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        table.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}

//var dtable;

//$(document).ready(function () {
//    dtable = $('#productTable').DataTable({
//        "ajax": {
//            "url": "/Admin/Product/AllProducts"
//        },
//        "columns": [
//            { "data": "name" },
//            { "data": "description" },
//            { "data": "price" }
//        ]
//    });
//});