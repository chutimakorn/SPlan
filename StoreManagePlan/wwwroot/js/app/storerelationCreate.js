$(function () {


    $('#submit').click(function (e) {
        var values = [];

        var dateStart = $('#html5-date-start').val();
        var dateEnd = $('#html5-date-end').val();

        if (dateStart) {
            dateStart = formatDate(dateStart);
        }

        if (dateEnd) {
            dateEnd = formatDate(dateEnd);
        }


        // ตรวจสอบการเลือกรายการใน dropdown และ multiselect_to
        if ($('#defaultSelect').val() === '0') {
            e.preventDefault()
            $("#import-not-success .toast-body").text("กรุณาเลือก Producer");
            jQuery('#import-not-success').toast('show');
            //alert('กรุณาเลือก Producer');

            return;
        }



        // ดำเนินการ submit เมื่อผู้ใช้เลือกรายการครบถ้วน
        var x = document.getElementById("multiselect_to");
        if (x.length === 0) {

            //alert('กรุณาเลือก Selected Seller อย่างน้อย 1 รายการ');
            $("#import-not-success .toast-body").text("กรุณาเลือก Sellers อย่างน้อย 1 รายการ");
            jQuery('#import-not-success').toast('show');
            e.preventDefault()
            return;
        }



        for (var i = 0; i < x.length; i++) {
            values.push(x.options[i].value);
        }
        var resultString = values.join(', ');
        var hub = $('#defaultSelect').val();

        $('#start').val(dateStart);
        $('#end').val(dateEnd);
        $('#listSpoke').val(resultString);
        $('#hubID').val(hub);
        // $('#form1').submit();

    });

    function formatDate(dateString) {
        // Assuming the input date format is "yyyy-mm-dd"
        var parts = dateString.split("-");
        var formattedDate = parts[0] + parts[1] + parts[2];
        return formattedDate;
    }

});

