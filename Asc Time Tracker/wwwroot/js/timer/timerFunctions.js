const TIME_STEP = 100;

var jobTimers = new Map();
var currTimerId = 0;
var expectedTime = dateNowRounded(); // What the correct time should be, to check for drift.
var stepInterval;

// Flag to check if this page contains the timer to update.
// Hacky way: check that the timer row exists to add to.
var updateUi = $("#timersRow").length > 0;

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
    if (updateUi) {
        stepTimers(jobTimers);
    }

    // TODO: performance: only need to save timers on page change or minimization
    // use page visibility API for minimization, and beforeUnload for navigation
    setInterval(saveTimers, 1000);
};

/**
 * Adds a new timer, optionally with fields.
 *
 * @param {JobTimer} timer  (Optional) Saved timer to copy over fields from.
 */
function addTimer(timer = null) {
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
                $("#timersRow").append(view);

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

/* Update timers on the UI. */
function stepTimers(jobTimers) {
    let drift = dateNowRounded() - expectedTime;

    /* Logging for console drift.
    if (drift > TIME_STEP) {
        cons
        ole.warn('The drift exceeded the interval.');
    } */

    for (let timer of jobTimers.values()) {
        if (timer.paused) continue;

        updateTimeDisplay(timer);
    }

    expectedTime += TIME_STEP;
    stepInterval = setTimeout(stepTimers.bind(null, jobTimers), Math.max(0, TIME_STEP - drift));
}

// █▀▀ █ █ █▀▀ █▄ █ ▀█▀  █   ▀█▀ █▀▀ ▀█▀ █▀▀ █▄ █ █▀▀ █▀█ █▀▀
// ██▄ ▀▄▀ ██▄ █ ▀█  █   █▄▄ ▄█▄ ▄██  █  ██▄ █ ▀█ ██▄ █▀▄ ▄██

// If the creation form was populated from a timer, delete that timer on submission.
$("#timeLogForm").on("submit",
    function () {
        let timerId = $("#timerToRemove").val();
        if (timerId) {
            removeTimer(parseInt(timerId));
            $("#timerToRemove").val(null);
        }
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
            resetTimer(jobTimers.get(timerId));
        } else if ($(this).hasClass("save-timer-btn")) {
            saveTimer(jobTimers.get(timerId));
            $("#timerToRemove").val(timerId);
        }
    });

// Remove the corresponding timer.
$(document).on("click",
    ".timer-close",
    function () {
        removeTimer(getTimerId(this));
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

/** Finds the timer id for an element inside _Timer.cshtml. */
function getTimerId(element) {
    return parseInt($(element).closest(".timer-card").find(".timer-value").val());
}

/** Removes a timer completely. */
function removeTimer(timerId) {
    jobTimers.delete(timerId);
    saveTimers();

    // Remove the timer card (four parent levels up - should
    // refactor if we ever change how _Timer.cshtml is structured.
    if (updateUi) {
        $("#timer_" + timerId).remove();
    }
}