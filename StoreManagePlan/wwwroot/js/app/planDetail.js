$(function () {

    var tableData = new DataTable('#table-data', {
        "lengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ]
    });

    //var tableLog = new DataTable('#table-log', {
    //    "lengthMenu": [
    //        [10, 25, 50, -1],
    //        [10, 25, 50, 'All']
    //    ]
    //});


    console.log("ready!");

    $('#export-btn').click(function () {

        var weekExport = $('#week').val();
        var storeExport = $('#storeDetail').val();

        console.log('weekExport :', weekExport);
        console.log('storeExport :', storeExport);

        var formData = new FormData();
        formData.append('weekNo', weekExport);
        formData.append('storeId', storeExport);

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/ProductPlanReview/ExportToExcel", true);
        xhr.responseType = "blob"; // Expecting a binary response
        xhr.onload = function () {
            var blob = new Blob([xhr.response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = "PlanDetail.xlsx";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        };
        xhr.send(formData);
    }); 

    $("#calculate-btn").click(function () {
        //$("#cal-label").text("Product Plan Status : Calculating...");

        var newText = document.createTextNode(" Calculating");

        var newDiv = $("<div>")
            .addClass("spinner-border spinner-border-sm text-primary")
            .css('margin-left', '5px')
            .attr('role', 'status')
            .html("");

        var newSpan = $("<span>").text("Loading...").addClass("visually-hidden");

        //// Append the text and div to the parent div
        $("#cal-label").empty().append(newText, newDiv.append(newSpan));

    });
});