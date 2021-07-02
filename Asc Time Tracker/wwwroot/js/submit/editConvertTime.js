let time = $('#TimeLog_TIME').val();

let secs = Math.floor(jobTimer.getTime() / 1000);
let hours = Math.floor(secs / 3600);
let minutes = Math.floor((secs % 3600) / 60);

$('#timeHours').val(hours);
$('#timeMinutes').val(minutes);