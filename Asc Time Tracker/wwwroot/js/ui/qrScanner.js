const qrCodeScanner = new Html5Qrcode("qr-reader");
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

// If you want to prefer back camera