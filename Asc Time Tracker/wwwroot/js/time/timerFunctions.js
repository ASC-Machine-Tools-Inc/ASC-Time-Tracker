const TIME_STEP = 100;

var jobTimers = new Map();
var currTimerId = 0;
var expectedTime = dateNowRounded(); // What the correct time should be, to check for drift.
var stepInterval;

// Flag to keep timer running if form submitted another way (like Add Log Manually)
var dontEndTimer = false;

// ▀█▀ █▄ █ ▀█▀ ▀█▀ ▀█▀ ▄▀▄ █   ▀█▀ ▀██ ▄▀▄ ▀█▀ ▀█▀ █▀█ █▄ █
// ▄█▄ █ ▀█ ▄█▄  █  ▄█▄ █▀█ █▄▄ ▄█▄ ██▄ █▀█  █  ▄█▄ █▄█ █ ▀█

function startTimers() {
    // Initialization - grab saved timers and recreate them.
    var storedTimers = localStorage["timers"];
    if (storedTimers != null) {
        storedTimers = new Map(JSON.parse(storedTimers));
        // Sort timers by ids so older ones are added first.
        storedTimers = new Map([...storedTimers.entries()].sort());
        for (let timer of storedTimers.values()) {
            addTimer(timer);
        }
    }

    // Start time loop to save timers.
    stepTimers(jobTimers);

    // Update date picker if we're on the right page.
    if ($(".day-picker").length > 0) {
        setDayPicker(new Date());
    }

    // TODO: performance: only need to save timers on page change or minimization
    // use page visibility API for minimization, and beforeUnload for navigation
    setInterval(saveTimers, 1000);
};

// Adds a new timer, optionally with fields.
function addTimer(timer = null) {
    // Check if this page contains the timer to update.
    // Hacky way: check that the timer row exists to add to.
    let updateUi = $("#timersRow").length > 0;

    var jobTimer = new JobTimer(currTimerId, TIME_STEP, updateUi);
    jobTimers.set(currTimerId, jobTimer);

    if (timer) {
        jobTimer.startTime = timer.startTime;
        jobTimer.totalPausedTime = timer.totalPausedTime;
        jobTimer.pauseTime = timer.pauseTime;

        jobTimer.fields = timer.fields;
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

                // Update display.
                if (timer) {
                    setFields(jobTimer, timer.fields);
                    updateTimeDisplay(jobTimer);

                    // If timer was running before, keep it running.
                    toggleTimer(jobTimer);

                    // Toggle to update paused time.
                    if (timer.paused) {
                        toggleTimer(jobTimer);
                    }
                }
            }
        });
    } else {
        // Other timer code to trigger even if not updating UI.
        if (!timer.paused) {
            toggleTimer(jobTimer);
        }
    }

    currTimerId++;
}

function saveTimers() {
    localStorage["timers"] = JSON.stringify(Array.from(jobTimers.entries()));
}

/* Update timers. */
function stepTimers(jobTimers) {
    let drift = dateNowRounded() - expectedTime;

    /* Logging for console drift.
    if (drift > TIME_STEP) {
        console.warn('The drift exceeded the interval.');
    } */

    for (let timer of jobTimers.values()) {
        if (timer.paused) continue;

        if (timer.updateUi) {
            updateTimeDisplay(timer);
        } else {
            // Only update the time.
            getTime(timer);
        }
    }

    expectedTime += TIME_STEP;
    stepInterval = setTimeout(stepTimers.bind(null, jobTimers), Math.max(0, TIME_STEP - drift));
}

// █▀▀ █ █ █▀▀ █▄ █ ▀█▀  █   ▀█▀ █▀▀ ▀█▀ █▀▀ █▄ █ █▀▀ █▀█ █▀▀
// ██▄ ▀▄▀ ██▄ █ ▀█  █   █▄▄ ▄█▄ ▄██  █  ██▄ █ ▀█ ██▄ █▀▄ ▄██

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

// Call the corresponding timer action for the corresponding timer
// on a timer function click.
$(document).on("click",
    ".timer-function-btn",
    function () {
        let timerId = getTimerId(this);

        if ($(this).hasClass("toggle-timer-btn")) {
            toggleTimer(jobTimers.get(timerId));
        } else if ($(this).hasClass("reset-timer-btn")) {
            console.log("reset");
            console.log(jobTimers.get(timerId));
            resetTimer(jobTimers.get(timerId));
            console.log(jobTimers.get(timerId));
        } else if ($(this).hasClass("save-timer-btn")) {
            saveTimer(jobTimers.get(timerId));
        }
    });

// Remove the corresponding timer.
$(document).on("click",
    ".timer-close",
    function () {
        let timerId = getTimerId(this);
        jobTimers.delete(timerId);
        saveTimers();

        // Remove the timer card (four parent levels up - should
        // refactor if we ever change how _Timer.cshtml is structured.
        $(this).closest(".timer-card").remove();
    });

// Detect notes change and update stored field.
$(document).on(
    "input",
    ".timer-field",
    function () {
        let timerId = getTimerId(this);

        // Might have to add more fields here later.
        let timer = jobTimers.get(timerId);
        timer.fields["TimeLog_Notes"] = $(this).val();
    });

// █▄█ █▀▀ █   █▀█ █▀▀ █▀█ █▀▀
// █ █ ██▄ █▄▄ █▀▀ ██▄ █▀▄ ▄██

/* Finds the timer id for an element inside _Timer.cshtml */
function getTimerId(element) {
    return parseInt($(element).closest(".timer-card").find(".timer-value").val());
}