$(function () {

    //var tableData = new DataTable('#table-data', {
    //    "lengthMenu": [
    //        [10, 25, 50, -1],
    //        [10, 25, 50, 'All']
    //    ]
    //});


    console.log("ready!");

    $('#export-btn').click(function () {
        var store = $('#html5-date-input-store').val();
        var week = $('#html5-date-input-week').val();
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/PlanOfWeek/ExportToExcel?Store=" + store + "&Week=" + week, true);
        xhr.responseType = "blob"; // Expecting a binary response
        xhr.onload = function () {
            var blob = new Blob([xhr.response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = "PlanOfWeek.xlsx";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        };
        xhr.send();
    });


    $('.select-action').on('change', function () {
        console.log('Selected value');
        document.getElementById("form1").submit();
    });

    function submitForm() {
        console.log("action!");
        document.getElementById("form1").submit();
    };

    $('#printButton').click(function () {
        window.print();
    });

    $('#submitButton').click(function () {
        event.preventDefault();
        var week = $("#html5-date-input-week").val();
        var store = $("#html5-date-input-store").val();
        var type = $("#html5-date-input-type").val();

        var formData = new FormData();
        formData.append('selectedStore', store);
        formData.append('selectedWeek', week);
        formData.append('type', type);
        $.ajax({
            url: '/PlanOfWeek/Approve', // Update the URL based on your actual URL structure
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {

                if (data.success) {
                    $('#import-success').toast('show');
                    
                }
                else {
                    $("#import-not-success .toast-body").text("อนุมัติไม่สำเร็จ ข้อความ: "+data.message);
                    $('#import-not-success').toast('show');
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
});