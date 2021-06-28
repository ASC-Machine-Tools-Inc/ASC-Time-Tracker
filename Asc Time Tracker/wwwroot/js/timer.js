/**
 * Timer for logging time spent on stuff.
 *
 * @param {int}      interval  Interval speed (in milliseconds)
 * @param {bool}     logError (Optional) Flag to log error if drift exceeds interval
 */
function JobTimer(interval, logError) {
    var self = this;
    var expected, timeout;
    this.time = 0;
    this.interval = interval;

    this.start = function () {
        expected = Date.now() + this.interval;
        timeout = setTimeout(step, this.interval);

        // Javascript is a pain sometimes. Sorry Bootstrap, but I still need jQuery
        // every now and then to make things work.
        $('#jobStatusCollapse').collapse('hide');
        $('#jobTimeCollapse').collapse('show');

        document.getElementById('startBtn').style.display = 'none';
        document.getElementById('stopBtn').style.display = 'block';
        document.getElementById('resetBtn').style.display = 'block';
    }

    this.stop = function () {
        clearTimeout(timeout);
        document.getElementById('startBtn').style.display = 'block';
        document.getElementById('stopBtn').style.display = 'none';
    }

    this.reset = function () {
        clearTimeout(timeout);
        self.time = 0;

        // Update time display to 0.
        updateTime();

        $('#jobStatusCollapse').collapse('show');
        $('#jobTimeCollapse').collapse('hide');

        document.getElementById('startBtn').style.display = 'block';
        document.getElementById('stopBtn').style.display = 'none';
        document.getElementById('resetBtn').style.display = 'none';
    }

    this.getTime = function () {
        return self.time;
    }

    function step() {
        var drift = Date.now() - expected;
        if (drift > self.interval) {
            if (logError) {
                console.warn('The drift exceeded the interval.');
            }
        }

        // Update time.
        self.time += self.interval;

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