var jobTimers = new Map();
var currTimerId = 0;

// Flag to keep timer running if form submitted another way (like Add Log Manually)
var dontEndTimer = false;

function startTimers() {
    // Initialization - grab saved timers and recreate them.
    var storedTimers = localStorage["timers"];
    if (storedTimers != null) {
        storedTimers = new Map(JSON.parse(storedTimers));
        console.log(storedTimers);
        for (var timer of storedTimers.values()) {
            console.log(timer);
            jobTimers.set(timer.id, timer);
            startTimer(timer);
        }
    }

    // Update date picker if we're on the right page.
    if ($(".day-picker").length > 0) {
        setDayPicker(new Date());
    }

    // Start time loop to save timers.
    // TODO: performance: only need to save timers on page change or minimization
    // use page visibility API for minimization, and beforeUnload for navigation
    setInterval(saveTimers, 1000);
};

// Adds a new timer, optionally with fields.
function addTimer(fields = null) {
    // Check if this page contains the timer to update.
    // Hacky way: check that the timer row exists to add to.
    let updateUi = $("#timersRow").length > 0;

    var jobTimer = new JobTimer(currTimerId, 10, updateUi);
    jobTimers.set(currTimerId, jobTimer);

    // Update the UI if we're on the right page.
    if (updateUi) {
        $.ajax({
            type: "GET",
            url: "/TimeLog/_Timer",
            data: {
                timerId: currTimerId
            },
            success: function (view) {
                $("#timersRow").prepend(view);

                // Update fields.
                jobTimer.setFields(fields);
            }
        });
    }

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

function saveTimers() {
    localStorage["timers"] = JSON.stringify(Array.from(jobTimers.entries()));
}

// If saving current timer, end it on submission.
// Shouldn't have to worry about validation - those fields are already populated.
$("#timeLogFormSubmit").on("click", function (event) {
    // TODO: see which timer to reset
    // Reset now destroys timers -> change to delete?
    if (dontEndTimer) {
        dontEndTimer = false;
    } else {
        // TODO
    }
});

// Set flag that we're manually adding a log.
$("#actionCardAdd").on("click", function () {
    dontEndTimer = true;
});

// Destroy timers on logout.
$("#logoutButton").on("click", function () {
    for (let timer in jobTimers) {
        reset(timer);
    }

    jobTimers = new Map();
    saveTimers();
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

    let fields = { "TimeLog_Notes": $("#timerNotes").val() };
    addTimer(fields);

    // Clear input field.
    $("#timerNotes").val("");
});

// Call the corresponding timer action for the corresponding timer
// on a timer function click.
$(".timer-function-btn").on("click", function () {
    var timerId = $(this).closest("#timerId").val();

    if ($(this).hasClass("start-timer-btn")) {
        jobTimers.get(timerId).start();
    } // TODO: other functions
});

// Remove the corresponding timer
$(document).on("click", ".timer-close", function () {
    //var timerId = $(this).closest(".card-body").find(".timer-value").val();
    //console.log(timerId);

    // Remove the timer card (four parent levels up - should
    // refactor if we ever change how _Timer.cshtml is structured.
    $(this).parents().eq(3).remove();
});