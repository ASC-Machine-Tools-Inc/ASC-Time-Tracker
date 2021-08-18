// Convert the stored time in seconds into hours and minutes for the input.
function convertTime() {
    let time = $('#TimeLog_Time').val();
    console.log(time);

    let hours = Math.floor(time / 3600);
    let minutes = Math.floor((time % 3600) / 60);

    $('#timeHoursEdit').val(hours);
    $('#timeMinutesEdit').val(minutes);
};