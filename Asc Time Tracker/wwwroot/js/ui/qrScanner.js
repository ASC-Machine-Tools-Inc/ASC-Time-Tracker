/* TODO: readd later with ability to scan
if (window.location.href.endsWith("TimeLog")) {
    const qrCodeScanner = new Html5Qrcode("qr-reader");
}
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