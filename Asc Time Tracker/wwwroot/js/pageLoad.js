// Load up saved localstorage data for our page.
$(document).ready(function () {
    startPickers();
    startTimers();

    // Enable tooltips.
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    })
});