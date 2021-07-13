const qrCodeScanner = new Html5Qrcode("qr-reader");
var scannerOn = false;

const qrCodeSuccessCallback = (decodedText, decodedResult) => {
    console.log(`Code scanned = ${decodedText}`, decodedResult);

    // Check for valid ASC code.
    if (decodedText.substring(0, 3) == 'ASC') {
        jobTimer.start(decodedText.substring(4));

        // Stop scanning on a successful scan.
        qrCodeScanner.stop();
    } else {
        // Show invalid scan modal.
    }
};

const config = { // 4:3 aspect ratio
    fps: 10, aspectRatio: 1.333334
};

function toggleScanner() {
    if (scannerOn) {
        $('#scannerBtn').html('Start with QR code');
        qrCodeScanner.stop();
    } else {
        $('#scannerBtn').html('Close');
        qrCodeScanner.start({ facingMode: 'environment' }, config, qrCodeSuccessCallback);
    }

    scannerOn = !scannerOn;
}