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

        // Save start time so we can get the time even while off the page.
        this.startTime = null;

        // List of fields for the timer used in submitting.
        this.fields = {};

        // To keep track of when the timer's paused to not count towards total time.
        this.totalPausedTime = 0;
        this.pauseTime = null;

        this.paused = true;
    }
}

/** Play/pause the timer. */
function toggleTimer(timer) {
    if (timer.paused) {  // Play timer.
        if (!timer.startTime) {  // Set the start time if it's the first start.
            timer.startTime = Date.now();
        }

        if (timer.pauseDate) {  // If previously paused, ignore this time.
            timer.totalPausedTime += Date.now() - timer.pauseDate;
            timer.pauseDate = null;
        }

        if (timer.updateUi) {
            let timerUi = $("#timer_" + timer.id);
            timerUi.find(".reset-timer-btn").toggleClass("d-none d-inline-block");
            timerUi.find(".save-timer-btn").toggleClass("d-none d-inline-block");

            timerUi.find(".bi-play-fill").toggleClass("bi-play-fill bi-pause-fill");

            //document.getElementById("scannerBtn").style.display = "none";
        }
    } else {  // Pause timer.
        timer.pauseDate = Date.now();

        if (timer.updateUi) {
            $("#timer_" + timer.id).find(".bi-pause-fill").toggleClass("bi-pause-fill bi-play-fill");
        }
    }

    timer.paused = !timer.paused;
}

/** Restart the timer back to 0. */
function resetTimer(timer) {
    timer.startTime = null;
    timer.totalPausedTime = 0;
    timer.paused = true;

    if (timer.updateUi) {
        // Update time display to 0.
        updateTimeDisplay(timer);

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
function saveTimer(timer) {
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
    let time = 0;

    // If the timer was started, calculate the time.
    if (timer.startTime) {
        time += Date.now() - timer.startTime;
        time -= timer.totalPausedTime;  // Ignore the paused time.
    }

    // If currently paused, ignore all time passed since pausing.
    if (timer.pausedDate) {
        time -= Date.now() - timer.pausedDate;
    }

    return time;
}

/** Update the display for this timer. */
function updateTimeDisplay(timer) {
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
        timer.fields[fieldKey] = fields[fieldKey];

        $("#timer_" + timer.id).find("#" + fieldKey + "_Display").val(fields[fieldKey]);
    }
}