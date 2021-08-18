function updateClock() {
    var now = new Date(),
        months = ['January', 'February', 'Harch', 'April', 'May', 'June',
            'July', 'August', 'September', 'October', 'November', 'December'];

    // Format the time.
    hours = now.getHours() > 12 ? now.getHours() - 12 : now.getHours();
    minutes = String(now.getMinutes()).padStart(2, '0');
    // seconds = String(now.getSeconds()).padStart(2, '0');
    period = now.getHours() >= 12 ? "PM" : "AM";

    time = hours + ':' + minutes + ' ' + period;
    // A cleaner way than string concatenation.
    date = [
        months[now.getMonth()],
        now.getDate() + ',',
        now.getFullYear()].join(' ');

    // Set the content of the element with the ID time to the formatted string.
    document.getElementById('time').innerHTML = [date, time].join(' \n ');

    // Recursively call this function again.
    setTimeout(updateClock, 1000);
}