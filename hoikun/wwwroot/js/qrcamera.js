let dotNetHelper = null;

function startQrScanner(dotNetObjRef) {
    dotNetHelper = dotNetObjRef;

    const html5QrCode = new Html5Qrcode("qr-reader");
    const config = { fps: 10, qrbox: 250 };

    html5QrCode.start(
        { facingMode: "environment" },
        config,
        (decodedText, decodedResult) => {
            console.log(`QR Code detected: ${decodedText}`);
            dotNetHelper.invokeMethodAsync("OnQrCodeScanned", decodedText);
            html5QrCode.stop(); // 読み取り後に停止（必要に応じて）
        },
        (errorMessage) => {
            // 読み取り失敗時
            console.warn(`QR scan error: ${errorMessage}`);
        }
    ).catch(err => {
        console.error("Camera start error", err);
    });
}
