$(function () {

    $('.select-action').on('change', function () {
        console.log('Selected value');
        document.getElementById("form1").submit();
    });

    $('#printButton').click(function () {
        $('#item-table').addClass('section-to-print');
        window.print();
        $('#item-table').removeClass('section-to-print');
    });

    $('#printButton2').click(function () {
        $('#boms').addClass('section-to-print');
        window.print();
        $('#boms').removeClass('section-to-print');
    });

    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('#goToTopBtn').fadeIn();
        } else {
            $('#goToTopBtn').fadeOut();
        }
    });

    // Scroll to the top of the page when the button is clicked
    $('#goToTopBtn').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
        return false;
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