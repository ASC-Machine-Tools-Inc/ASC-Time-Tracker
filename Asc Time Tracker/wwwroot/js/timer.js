/**
 * Timer for logging time spent on stuff.
 *
 * @param {int}      interval  Interval speed (in milliseconds)
 * @param {bool}     logError (Optional) Flag to log error if drift exceeds interval
 */
function JobTimer(interval, logError) {
    var self = this;
    var expected, timeout, startTime;
    this.timeExpended = 0; // Needed to save time when timer is paused
    this.interval = interval;

    this.start = function () {
        expected = Date.now() + this.interval;
        timeout = setTimeout(step, this.interval);
        startTime = Date.now();

        // Javascript is a pain sometimes. Sorry Bootstrap, but I still need jQuery
        // every now and then to make things work.
        $('#jobStatusCollapse').collapse('hide');
        $('#jobTimeCollapse').collapse('show');

        document.getElementById('startBtn').style.display = 'none';
        document.getElementById('stopBtn').style.display = 'block';
        document.getElementById('saveBtn').style.display = 'block';
        document.getElementById('resetBtn').style.display = 'block';

        // Show alert if first time clicking start
        let timerAlert = document.getElementById('timerStartAlert');
        if (timerAlert) {
            timerAlert.style.display = 'block';
            setTimeout(function () {
                $('#timerStartAlert').alert('close');
            }, 5000);
        }
    }

    this.stop = function () {
        self.timeExpended = this.getTime();
        console.log("pausing with time: " + self.timeExpended);

        clearTimeout(timeout);
        document.getElementById('startBtn').style.display = 'block';
        document.getElementById('stopBtn').style.display = 'none';
    }

    this.reset = function () {
        clearTimeout(timeout);
        self.timeExpended = 0;

        // Update time display to 0.
        updateTime();

        $('#jobStatusCollapse').collapse('show');
        $('#jobTimeCollapse').collapse('hide');

        document.getElementById('startBtn').style.display = 'block';
        document.getElementById('stopBtn').style.display = 'none';
        document.getElementById('saveBtn').style.display = 'none';
        document.getElementById('resetBtn').style.display = 'none';
    }

    this.save = function () {
        $('#timeLogSubmitModal').modal('show');

        let secs = Math.floor(jobTimer.getTime() / 1000);
        let hours = Math.floor(secs / 3600);
        let minutes = Math.floor((secs % 3600) / 60);

        $('#timeHours').val(hours);
        $('#timeMinutes').val(minutes);
    }

    this.getTime = function () {
        return Date.now() - startTime + self.timeExpended;
    }

    // Self-adjusting for drift time step. Only updates UI.
    function step() {
        var drift = Date.now() - expected;
        if (drift > self.interval) {
            if (logError) {
                console.warn('The drift exceeded the interval.');
            }
        }

        updateTime();
        expected += self.interval;
        timeout = setTimeout(step, Math.max(0, self.interval - drift));
    }

    // Update relevant html.
    function updateTime() {
        let elapsedTime = jobTimer.getTime();

        let secs = Math.floor(elapsedTime / 1000);
        let hours = String(Math.floor(secs / 3600)).padStart(2, '0');
        let minutes = String(Math.floor((secs % 3600) / 60)).padStart(2, '0');
        let seconds = String(secs % 60).padStart(2, '0');

        let time = [hours, minutes, seconds].join(':');

        document.getElementById('jobTime').innerHTML = time;
    }
}

var jobTimer = new JobTimer(10, false);