var dtable;
$(document).ready(function () {
    dtable = $('#myTable').DataTable({

        "ajax": {
            "url": "/Admin/Product/AllProducts"
        },

        "columns": [

            { "data": "name" },
            { "data": 'description' },
            { "data": "price" },
            { "data": "category.name" },

            {

                "data": "id",
                "render": function (data) {
                    return `   

                        <a href="/Admin/Product/CreateEdit?id=${data}"><i class="bi bi-pencil-square"></i></a>
                        <a asp-action="Delete" asp-controller="Product" asp-route-id="@item.Id"><i class="bi bi-trash"></i></a>
`
                }
            }

        ]

    });
});