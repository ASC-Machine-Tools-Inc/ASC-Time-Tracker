class JobTimer {
    /**
     * Timer for logging time spent on stuff.
     *
     * @param {int}     id         Number to identify this specific timer by.
     * @param {int}     interval   Time step interval speed (in milliseconds).
     * @param {bool}    updateUi   (Optional) Flag to change display (set if on right page).
     */
    constructor(id, interval, updateUi = false) {
        this.id = id;
        this.interval = interval;
        this.updateUi = updateUi;

        this.startTime = null;
        this.fields = {};

        // Timeout for the time step.
        this.stepTimeout = null;

        // What the correct time should be, to check for drift.
        this.expectedTime = 0;

        // Needed to save time when timer is paused (>= expected time).
        this.savedTime = 0;

        this.paused = false;
    }
}

/** Start the timer. */
function startTimer(timer) {
    timer.paused = false;
    timer.startTime = timer.expectedTime = Date.now();

    step(timer); // Start timeout for updating time.

    if (timer.updateUi) {
        document.getElementById("startBtn").style.display = "none";
        //document.getElementById("scannerBtn").style.display = "none";
        document.getElementById("stopBtn").style.display = "block";
        document.getElementById("saveBtn").style.display = "block";
        document.getElementById("deleteBtn").style.display = "block";
    }
}

/** Pause the timer. */
function stop(timer) {
    timer.paused = true;
    timer.savedTime = getTime(timer);

    clearTimeout(timeout);

    if (timer.updateUi) {
        document.getElementById("startBtn").style.display = "block";
        document.getElementById("stopBtn").style.display = "none";
    }
}

/** Restart the timer back to 0. */
function reset(timer) {
    clearTimeout(timer.stepTimeout);
    timer.savedTime = 0;

    if (timer.updateUi) {
        // Update time display to 0.
        updateTime(timer);

        // TODO: Convert into own function after adding more fields?
        $("#TimeLog_JobNum_Display").html("");

        document.getElementById("startBtn").style.display = "block";
        //document.getElementById("scannerBtn").style.display = "inline-block";
        document.getElementById("stopBtn").style.display = "none";
        document.getElementById("saveBtn").style.display = "none";
        document.getElementById("deleteBtn").style.display = "none";
    }
}

/** Populate the modal fields for adding a log to the database. */
function save(timer) {
    let secs = Math.floor(getTime(timer) / 1000);
    let hours = Math.floor(secs / 3600);
    let minutes = Math.floor((secs % 3600) / 60);

    if (timer.updateUi) {
        $("#timeLogSubmitModal").modal("show");
        $("#timeHours").val(hours);
        $("#timeMinutes").val(minutes);

        // Save fields to modal form fields.
        for (let field in timer.fields) {
            $("#" + field).val(timer.fields[field]);
        }
    }
}

/** Calculate the time for this timer. */
function getTime(timer) {
    return Date.now() - timer.startTime + timer.savedTime;
}

// Time step that adjusts for drift.
function step(timer) {
    let drift = Date.now() - timer.expectedTime;

    /* Logging for console drift.
    if (drift > this.interval) {
        console.warn('The drift exceeded the interval.');
    } */

    // Update this timer in the array.
    jobTimers.set(timer.id, timer);

    if (timer.updateUi) {
        updateTime(timer);
    }

    timer.expectedTime += timer.interval;
    timer.timeout = setTimeout(step.bind(null, timer), Math.max(0, timer.interval - drift));
}

/** Update the display for this timer. */
function updateTime(timer) {
    let elapsedTime = getTime(timer);

    let secs = Math.floor(elapsedTime / 1000);
    let hours = String(Math.floor(secs / 3600)).padStart(2, "0");
    let minutes = String(Math.floor((secs % 3600) / 60)).padStart(2, "0");
    let seconds = String(secs % 60).padStart(2, "0");

    let time = [hours, minutes, seconds].join(":");

    $("#timer_" + timer.id).find(".time-display").html(time);
}

function setFields(timer, fields) {
    for (let fieldKey in fields) {
        this.fields[fieldKey] = fields[fieldKey];

        $("#timer_" + timer.id).find("#" + fieldKey + "_Display").html(fields[fieldKey]);
    }
}