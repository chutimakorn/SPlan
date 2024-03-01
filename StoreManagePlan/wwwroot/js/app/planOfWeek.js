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
});