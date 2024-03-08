$(function () {

    var tableData = new DataTable('#table-data', {
        "lengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ]
    });

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

    $('#delete-btn').click(function () {
        var selected = [];
        $("input#defaultCheck:checked").each(function () {
            var sku = $(this).closest("tr").find("td:eq(1)").text(); // แก้ตำแหน่ง column ตามต้องการ
            selected.push(sku);
        });

        if (selected.length === 0) {
            $("#import-not-success .toast-body").text("กรุณาเลือกสิ่งที่ต้องการลบ");
            $('#import-not-success').toast('show');
            return;
        }


        $('#delete-modal').modal('show');
    });

    $('#export-btn').click(function () {
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/StoreTypes/ExportToExcel", true);
        xhr.responseType = "blob"; // Expecting a binary response
        xhr.onload = function () {
            var blob = new Blob([xhr.response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = "StoreTypes.xlsx";
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
            url: '/StoreTypes/Upload', // Update the URL based on your actual URL structure
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
        // ตรวจสอบว่า Checkbox ทั้งหมดควรติ๊กหรือไม่
        var checkboxes = $("tbody input[type=checkbox]");
        var checkAllCheckbox = $("#checkAll");
        checkAllCheckbox.prop("checked", $("tbody input[type=checkbox]:checked").length === checkboxes.length);
    });

    // เมื่อคลิกที่ปุ่ม delete
    $("#delete-item").on("click", function () {
        // รวม SKU ที่ถูก check ใน checkbox
        var selected = [];
        $("input#defaultCheck:checked").each(function () {
            var sku = $(this).closest("tr").find("td:eq(1)").text(); // แก้ตำแหน่ง column ตามต้องการ
            selected.push(sku);
        });

        // นำรายการ SKU มาใส่ใน hidden input
        $("#hiddenInputId").val(selected.join(','));

       
    });
});