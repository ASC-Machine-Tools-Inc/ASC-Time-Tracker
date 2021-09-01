var jobTimer;
var jobTimers;

// Flag to keep timer running if form submitted another way (like Add Log Manually)
var dontEndTimer = false;

function startTimer() {
    // Check if this page contains the timer to update.
    // Hacky way: check that there's an instance of the class time-col,
    // which represents the column containing the action cards and timers.
    let uiFlag = $(".time-col").length > 0;

    // Initialization
    jobTimer = new JobTimer(1, 10, uiFlag);

    var timers = JSON.parse(localStorage.getItem("timers"));
    for (var timer of timers) {
        var savedTime = parseInt(localStorage.getItem("savedTime"));
        var paused = JSON.parse(localStorage.getItem("paused"));
        if (savedTime) {
            jobTimer.timeExpended = savedTime;
            jobTimer.start();

            // Start timer, but don't let time run.
            if (paused) {
                jobTimer.stop();
            }
        }
    }

    // Update date picker if we're on the right page.
    if ($(".day-picker")[0]) {
        setDayPicker(new Date());
    }
};

// If saving current timer, end it on submission.
// Shouldn't have to worry about validation - those fields are already populated.
$("#timeLogFormSubmit").on("click", function (event) {
    // TODO: see which timer to reset
    // Reset now destroys timers -> change to delete?
    if (dontEndTimer) {
        dontEndTimer = false;
    } else {
        jobTimer.reset();
    }
});

// Set flag that we're manually adding a log.
$("#actionCardAdd").on("click", function () {
    dontEndTimer = true;
});

// Destroy timers on logout.
$("#logoutButton").on("click", function () {
    for (var timer of jobTimers) {
        jobTimer.reset();
    }
});