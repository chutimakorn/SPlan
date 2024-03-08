$(function () {

    var tableData = new DataTable('#table-data', {
        "lengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ],
        "autoWidth": true,
    });

    var tableLog = new DataTable('#table-log', {
        "lengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ]
    });

    console.log("ready!");
    $('#delete-btn').click(function () {
        var checkedCheckboxes = $('input#defaultCheck:checked'); 
        var selectedIds = checkedCheckboxes.map(function () {
            return this.value; 
        }).get().join(',');

        if (selectedIds == "") {
            return;
        }


        $('#delete-modal').modal('show');
    });

    $('#export-btn').click(function () {
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/StoreRelations/ExportToExcel", true);
        xhr.responseType = "blob"; // Expecting a binary response
        xhr.onload = function () {
            var blob = new Blob([xhr.response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = "StoreRelations.xlsx";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        };
        xhr.send();
    });

    
});