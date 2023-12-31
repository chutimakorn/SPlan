$(function () {
    console.log("ready!");
    $('#import-btn').click(function () {
        $('#import-modal').modal('show');
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
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
});