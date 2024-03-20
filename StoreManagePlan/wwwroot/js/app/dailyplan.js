$(function () {

    $('.select-action').on('change', function () {
        console.log('Selected value');
        document.getElementById("form1").submit();
    });
});