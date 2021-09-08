var jobTimers = [];
var currTimerId = 0;

// Flag to keep timer running if form submitted another way (like Add Log Manually)
var dontEndTimer = false;

function startTimer() {
    // Check if this page contains the timer to update.
    // Hacky way: check that there's an instance of the class time-col,
    // which represents the column containing the action cards and timers.
    let uiFlag = $(".time-col").length > 0;

    // Initialization.
    var storedTimers = JSON.parse(localStorage.getItem("timers"));
    if (storedTimers != null) {
        for (var timer of storedTimers.timers) {
            var jobTimer = new JobTimer(1, 10, uiFlag);
            jobTimers.add(jobTimer);
            currTimerId++;
            // TODO: UI work to generate timers?

            jobTimer.timeExpended = timer.savedTime;
            jobTimer.start();

            // Start timer, but don't let time run (to show previous saved time).
            if (jobTimer.paused) {
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

// Add new timer on click.
$("#addTimerBtn").on("click", function () {
    $.ajax({
        type: "GET",
        url: "/TimeLog/_Timer",
        data: {
            timerId: currTimerId
        },
        beforeSend: function () {
            // Display placeholder?
        },
        success: function (view) {
            $("#timersRow").prepend(view);
        }
    });
});

// Call the corresponding timer action for the corresponding timer
// on a timer function click.
$(".timer-function-btn").on("click", function () {
    var timerId = $(this).closest("#timerId");

    if ($(this).hasClass("start-timer-btn")) {
        jobTimers[timerId].start();
    }
})