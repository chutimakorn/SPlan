$(function () {
    console.log("ready!");
    $('#import-btn').click(function () {
        $('#import-modal').modal('show');
    });

    $('#export-btn').click(function () {
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/Item/ExportToExcel", true);
        xhr.responseType = "blob"; // Expecting a binary response
        xhr.onload = function () {
            var blob = new Blob([xhr.response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = "ItemList.xlsx";
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
            url: '/Item/Upload', // Update the URL based on your actual URL structure
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
});