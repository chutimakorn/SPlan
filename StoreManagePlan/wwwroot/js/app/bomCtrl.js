$(function () {

    //var tableData = new DataTable('#table-data', {
    //    "lengthMenu": [
    //        [10, 25, 50, -1],
    //        [10, 25, 50, 'All']
    //    ]
    //});

    var tableLog = new DataTable('#table-log', {
        "lengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ]
    });

    console.log("ready!");
    $('#import-btn').click(function () {
        $('#import-modal').modal('show');
    });

    $('#export-btn').click(function () {
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/Bom/ExportToExcel", true);
        xhr.responseType = "blob"; // Expecting a binary response
        xhr.onload = function () {
            var blob = new Blob([xhr.response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = "BomList.xlsx";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        };
        xhr.send();
    });

    $('#save-import').click(function () {
        $('#import-modal').modal('hide');
        var form = $('#formFile')[0].files[0];;
        var formData = new FormData();
        formData.append('file', form);

        $.ajax({
            url: '/Bom/Upload', // Update the URL based on your actual URL structure
            type: 'POST',
            data: formData,
            processData: false, // Important: Don't process the data (already done by FormData)
            contentType: false, // Important: Set content type to false as FormData will take care of it
            success: function (data) {

                if (data.status !== 'success') {
                    $("#import-not-success .toast-body").text(data.message);
                    $('#import-not-success').toast('show');
                }
                else {
                    window.location.reload();
                    $('#import-success').toast('show');
                }


            },
            error: function (error) {
                $('#import-not-success').toast('show');
                console.log(error);
            }
        });
    });


    // เมื่อคลิกที่ปุ่ม delete
    $("#delete-btn").on("click", function () {
        var selectedItems = [];
        var selectedItemHead = [];

        $("input#defaultCheck:checked").each(function () {
            var skuCode = $(this).closest("tr").find("td:nth-child(2)").text(); // SKU Code
            var ingredientSku = $(this).closest("tr").find("td:nth-child(3)").text(); // Ingredient SKU

            var selectedItem = {
                sku_id: skuCode,
                ingredient_sku: ingredientSku
            };

            selectedItems.push(selectedItem);
        });

        $("input#skuCheckBox:checked").each(function () {
            var skuCode = $(this).closest("tr").find("td:nth-child(2)").text(); // SKU Code
            var ingredientSku = $(this).closest("tr").find("td:nth-child(3)").text(); // Ingredient SKU

            var selectedItem = {
                sku_id: skuCode,
                ingredient_sku: ingredientSku
            };

            selectedItemHead.push(selectedItem);
        });

        // นำรายการ SKU และ Ingredient SKU มาใส่ใน hidden input
        $("#hiddenInputId").val(JSON.stringify(selectedItems));
        $("#selectedHeadGroup").val(JSON.stringify(selectedItemHead));
    });

});

