// Convert the stored time in seconds into hours and minutes for the input.
$(document).ready(function () {
    let time = $('#TimeLog_TIME').val();

    let hours = Math.floor(time / 3600);
    let minutes = Math.floor((time % 3600) / 60);

    $('#timeHours').val(hours);
    $('#timeMinutes').val(minutes);
});