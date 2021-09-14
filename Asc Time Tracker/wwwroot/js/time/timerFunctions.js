var jobTimers = new Map();
var currTimerId = 0;

// Flag to keep timer running if form submitted another way (like Add Log Manually)
var dontEndTimer = false;

function startTimers() {
    // Initialization - grab saved timers and recreate them.
    var storedTimers = localStorage["timers"];
    if (storedTimers != null) {
        storedTimers = new Map(JSON.parse(storedTimers));
        for (var timer of storedTimers.values()) {
            addTimer(timer);
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
function addTimer(timer = null) {
    // Check if this page contains the timer to update.
    // Hacky way: check that the timer row exists to add to.
    let updateUi = $("#timersRow").length > 0;

    var jobTimer = new JobTimer(currTimerId, 10, updateUi);
    jobTimers.set(currTimerId, jobTimer);

    if (timer) {
        jobTimer.startTime = timer.startTime;
        jobTimer.fields = timer.fields;
        jobTimer.savedTime = timer.savedTime;
        jobTimer.paused = timer.paused;
    }

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
                if (timer) {
                    setFields(jobTimer, timer.fields);
                }

                startTimer(jobTimer);

                // If paused, start timer, but don't let time run
                // (to show the previous saved time).
                if (timer.paused) {
                    stopTimer(jobTimer);
                }
            }
        });
    } else {
        // Don't need to wait for AJAX to finish, so we can update right away.
        if (!timer.paused) {
            startTimer(jobTimer);
        }
    }

    currTimerId++;
}

function saveTimers() {
    localStorage["timers"] = JSON.stringify(Array.from(jobTimers.entries()));
    console.log(localStorage["timers"]);
}

// If saving current timer, end it on submission.
// Shouldn't have to worry about validation - those fields are already populated.
$("#timeLogFormSubmit").on("click",
    function (event) {
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
$("#logoutButton").on("click",
    function () {
        for (let timer in jobTimers) {
            reset(timer);
        }

        jobTimers = new Map();
        saveTimers();
    });

// Add new timer.
$("#addTimerBtn").on("click",
    function () {
        addTimer();
    });

// Call the corresponding timer action for the corresponding timer
// on a timer function click.
$(".timer-function-btn").on("click",
    function () {
        var timerId = $(this).closest("#timerId").val();

        if ($(this).hasClass("start-timer-btn")) {
            jobTimers.get(timerId).start();
        } // TODO: other functions
    });

// Remove the corresponding timer.
$(document).on("click", ".timer-close", function () {
    // Clean up the timer, delete it from our map of timers, and save the changes.
    var timerId = parseInt($(this).closest(".timer-card").find(".timer-value").val());
    deleteTimer(jobTimers.get(timerId));
    jobTimers.delete(timerId);
    saveTimers();

    // Remove the timer card (four parent levels up - should
    // refactor if we ever change how _Timer.cshtml is structured.
    $(this).closest(".timer-card").remove();
});