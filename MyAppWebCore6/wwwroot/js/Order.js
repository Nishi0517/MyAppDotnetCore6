var dtable; //= new DataTable('#productTable');

$(document).ready(function () {


    var url = window.location.search;
    if (url.includes("pending")) {
        OrderTable("pending")
    }
    else {
        if (url.includes("approved")) {
            OrderTable("approved")
        }
        else {
            if (url.includes("shipped")) {
                OrderTable("shipped")
            }
            else {
                if (url.includes("underprocess")) {
                    OrderTable("underprocess")
                }
                else {
                    OrderTable("all");
                }
            }
        }
    }
    
});

function OrderTable(status) {
    dtable = $('#orderTable').DataTable({

        "ajax": {
            "url": "/Admin/Order/AllOrders?status="+status
        },
        "columns": [
            { "data": "name" },
            { "data": "phone" },
            { "data": "orderStatus" },
            { "data": "orderTotal" },
            {
                "data": "id",
                "render": function (data) {
                    return '<a href="/Admin/order/OrderDetails/' + data + '" class="btn btn-success">Update</a> '

                }
            }
        ]
    });
}

function RemoveOrder(url) {
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