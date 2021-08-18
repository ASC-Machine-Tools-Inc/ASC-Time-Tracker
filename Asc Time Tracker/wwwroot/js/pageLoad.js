// Runs all the client-side scripts when ASP.Net posts back (server-side).
// Fixes the issue with scripts not running on IndexInfo's full view.
function pageLoad() {
    startDatePicker();
    startTimer();
    convertTime();
}

// Call regularly for MainIndex.
$(document).ready(function () {
    startDatePicker();
    startTimer();
    convertTime();
});