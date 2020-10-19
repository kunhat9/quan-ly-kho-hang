$(document).ready(function () {
    ActiveMenu();
});


var ActiveMenu = function () {
    if (typeof mnActive != 'undefined') {
        $('.' + mnActive).addClass('active');
    }
}