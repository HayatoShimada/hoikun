let dotNetHelper = null;
let html5QrCode = null;

function startQrScanner(dotNetObjRef) {
    dotNetHelper = dotNetObjRef;

    if (html5QrCode === null) {
        html5QrCode = new Html5Qrcode("qr-reader");
    }

    const config = { fps: 10, qrbox: 250 };

    html5QrCode.start(
        { facingMode: "environment" },
        config,
        (decodedText) => {
            console.log(`QR Code detected: ${decodedText}`);
            dotNetHelper.invokeMethodAsync("OnQrCodeScanned", decodedText);
            html5QrCode.stop();
        },
        (errorMessage) => {
            console.warn(`QR scan error: ${errorMessage}`);
        }
    ).catch(err => {
        console.error("Camera start error", err);
    });
}
