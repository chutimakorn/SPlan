$(function () {

    //var tableData = new DataTable('#table-data', {
    //    "lengthMenu": [
    //        [10, 25, 50, -1],
    //        [10, 25, 50, 'All']
    //    ]
    //});


    console.log("ready!");

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

    $('.btn-edit').click(function () {
        var store = $(this).data('store');
        var sku = $(this).data('sku');
        var day = $(this).data('day');
        var week = $(this).data('week');
        var spoke = $(this).data('spoke');

        // Redirect the user to the desired URL
        var url = '/IbtOut/Edit?Store=' + store + '&SkuId=' + sku + '&Day=' + day + '&Spoke=' + spoke + '&Week=' + week;
        window.location.href = url;
    });

    $('#call-approve').click(function () {
        $('#modal-confirm').modal('show');
    });

    $('#approve-all').click(function () {
        event.preventDefault();
        var store = $("#spokeSelect").val();
        var week = $("#weekSelect").val();
        var day = $("#daySelect").val();

        var formData = new FormData();
        formData.append('Spoke', store);
        formData.append('Week', week);
        formData.append('Day', day);
        $.ajax({
            url: '/IbtOut/Approve', // 
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {

                if (data.success) {

                    $('#status-app').text("ยืนยันแล้ว");
                    $('#approve-all').prop('disabled', true);
                    $('#import-success').toast('show');
                }
                else {
                    $("#import-not-success .toast-body").text("อนุมัติไม่สำเร็จ ข้อความ: " + data.message);
                    $('#import-not-success').toast('show');
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    


    let mybutton = document.getElementById("btn-back-to-top");

    // When the user scrolls down 20px from the top of the document, show the button
    window.onscroll = function () {
        scrollFunction();
    };

    function scrollFunction() {
        if (
            document.body.scrollTop > 20 ||
            document.documentElement.scrollTop > 20
        ) {
            mybutton.style.display = "block";
        } else {
            mybutton.style.display = "none";
        }
    }
    // When the user clicks on the button, scroll to the top of the document
    mybutton.addEventListener("click", backToTop);

    function backToTop() {
        document.body.scrollTop = 0;
        document.documentElement.scrollTop = 0;
    }
});