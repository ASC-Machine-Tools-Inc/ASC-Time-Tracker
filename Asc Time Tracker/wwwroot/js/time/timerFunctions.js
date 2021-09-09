var jobTimers = [];
var currTimerId = 0;

// Flag to keep timer running if form submitted another way (like Add Log Manually)
var dontEndTimer = false;

function startTimer() {
    // Initialization.
    var storedTimers = JSON.parse(localStorage.getItem("timers"));
    if (storedTimers != null) {
        for (var timer of storedTimers.timers) {
            addTimer();
        }
    }

    // Update date picker if we're on the right page.
    if ($(".day-picker")[0]) {
        setDayPicker(new Date());
    }
};

// Adds a new timer.
function addTimer(fields) {
    // Check if this page contains the timer to update.
    // Hacky way: check that the timer row exists to add to.
    let updateUi = $("#timersRow").length > 0;

    // Update the UI if we're on the right page.
    var jobTimer;
    if (updateUi) {
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

                // Wait for timer to be added before updating fields.
                jobTimer = new JobTimer(currTimerId, 10, updateUi, fields);
            }
        });
    } else {
        jobTimer = new JobTimer(currTimerId, 10, updateUi, fields);
    }

    jobTimers.push(jobTimer);
    currTimerId++;

    //jobTimer.timeExpended = timer.savedTime;
    //jobTimer.start();

    // If paused, start timer, but don't let time run
    // (to show the previous saved time).
    /*
    if (jobTimer.paused) {
        jobTimer.stop();
    } */
}

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

// Open modal for entering timer notes before creation.
$("#addTimerBtn").on("click", function () {
    $("#timeLogCreateNotesModal").modal("show");
    // Wait for modal to open before focusing text field.
    $("#timeLogCreateNotesModal").on("shown.bs.modal", function () {
        $("#timerNotes").focus();
    });
});

// Add new timer on submit.
$("#timerFieldsForm").on("submit", function (e) {
    e.preventDefault();
    $("#timeLogCreateNotesModal").modal("hide");

    var notes = $("#timerNotes").val();
    addTimer(`TimeLog_Notes:${notes}`);

    // Clear input field.
    $("#timerNotes").val("");
});

// Call the corresponding timer action for the corresponding timer
// on a timer function click.
$(".timer-function-btn").on("click", function () {
    var timerId = $(this).closest("#timerId");

    if ($(this).hasClass("start-timer-btn")) {
        jobTimers[timerId].start();
    }
})