/* TODO: readd later with ability to scan

    const qrCodeScanner = new Html5Qrcode("qr-reader");
var scannerOn = false;

/* QR code scanner format:
 * ASC|TimeLog_{field}:{value}|TimeLog_{field}:{value}|...
 *
 * Example:
 * ASC|TimeLog_JobNum:12345E

const qrCodeSuccessCallback = (decodedText, decodedResult) => {
    console.log(`Code scanned = ${decodedText}`, decodedResult);

    // Check for valid ASC code.
    if (decodedText.substring(0, 3) === "ASC") {
        jobTimer.start(decodedText.substring(4));

        // Stop scanning on a successful scan.
        toggleScanner();
    } else {
        // Show invalid scan modal.
    }
};

const config = {
    fps: 10
};

function toggleScanner() {
    if (scannerOn) {
        $("#qrScanModal").modal("toggle");
        qrCodeScanner.stop();
    } else {
        $("#qrScanModal").modal("show");
        qrCodeScanner.start({ facingMode: "environment" }, config, qrCodeSuccessCallback);
    }

    scannerOn = !scannerOn;
}
 */

/**
 * Takes a specially formatted string for a timer and updates the timer's card.
 * String looks like: TimeLog_{field}:{value}|TimeLog_{field}:{value}|...
 *
 * @param {int}    id      The id for the job timer to update.
 * @param {string} fields  The formatted string to convert into fields.
 */
stringToFields = function (id, fields) {
    let splitValues = fields.split("|");

    for (let i = 0; i < splitValues.length; i++) {
        let splitPairs = splitValues[i].split(":");
        let pairKey = splitPairs[0];
        let pairValue = splitPairs[1];

        this.savedFields[pairKey] = pairValue;

        $("#timer_" + id).find("#" + pairKey + "_Display").html(pairValue);
    }
}