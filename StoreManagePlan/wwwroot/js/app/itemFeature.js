$(function () {
    console.log("ready!");
    $('#import-btn').click(function () {
        $('#import-modal').modal('show');
    });

    $('#export-btn').click(function () {
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/ItemFeatures/ExportToExcel", true);
        xhr.responseType = "blob"; // Expecting a binary response
        xhr.onload = function () {
            var blob = new Blob([xhr.response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = "ItemFeatures.xlsx";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        };
        xhr.send();
    });

    $('#save-import').click(function () {
        var form = $('#formFile')[0].files[0];; 
        var formData = new FormData();
        formData.append('file', form);

        $.ajax({
            url: '/ItemFeatures/Upload', // Update the URL based on your actual URL structure
            type: 'POST',
            data: formData,
            processData: false, // Important: Don't process the data (already done by FormData)
            contentType: false, // Important: Set content type to false as FormData will take care of it
            success: function (data) {
                //alert(data);
                $('#import-modal').modal('hide');
                $('#import-success').toast('show');
                window.location.reload();
            },
            error: function (error) {
                $('#import-modal').modal('hide');
                $('#import-not-success').toast('show');
                console.log(error);
            }
        });
    });

    $("#checkAll").on("change", function () {
        // หา Checkbox ทุกตัวใน tbody
        var checkboxes = $("tbody input[type=checkbox]");

        // ตั้งค่า Checked ของ Checkbox ทุกตัวตาม Checkbox ทั้งหมด
        checkboxes.prop("checked", $(this).prop("checked"));
    });

    // เมื่อคลิกที่ Checkbox ใน tbody
    $("tbody input[type=checkbox]").on("change", function () {
        var checkboxes = $("tbody input[type=checkbox]");
        // ตรวจสอบว่า Checkbox ทั้งหมดควรติ๊กหรือไม่
        var checkAllCheckbox = $("#checkAll");
        checkAllCheckbox.prop("checked", $("tbody input[type=checkbox]:checked").length === checkboxes.length);
    });

    // เมื่อคลิกที่ปุ่ม delete
    $("#delete-btn").on("click", function () {
        var selectedItems = [];
        $("input#defaultCheck:checked").each(function () {
            var storeCode = $(this).closest("tr").find("td:nth-child(2)").text(); // แก้ตำแหน่ง column ตามต้องการ
            var skuCode = $(this).closest("tr").find("td:nth-child(4)").text(); // แก้ตำแหน่ง column ตามต้องการ
            var selectedItem = {
                store_code: storeCode,
                sku_code: skuCode
            };
            selectedItems.push(selectedItem);
        });

        // นำรายการ SKU มาใส่ใน hidden input
        $("#hiddenInputId").val(JSON.stringify(selectedItems));

        // แสดง alert เพื่อตรวจสอบผลลัพธ์
        alert("Selected SKUs: " + $("#hiddenInputId").val());

   

    });

  

   
});