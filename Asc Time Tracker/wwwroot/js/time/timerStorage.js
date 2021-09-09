/**
 * A structure for saving timers into storage and retrieving them.
 *
 * @param {int}              id         Number to identify this specific timer by.
 * @param {date}             startTime  The time this timer was started.
 * @param {int}              savedTime  Time passed for this timer.
 * @param {bool}             paused     If this time was saved while paused.
 * @param {{string,string}}  fields     (Optional) List of fields in the format key:value,
 *                                                 with key-value pairs split with |.
 */
function StoredTimer(id, savedTime, paused, fields) {
}