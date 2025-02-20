window.barcodeScan = {
    dotNetObj: null,

    startCapture: function (dotNetObj) {
        this.dotNetObj = dotNetObj;

        console.log("StartCapture called, dotNetObj:", this.dotNetObj);

        const target = document.getElementById("camera");
        if (!target) {
            console.error("Error: Camera target element not found.");
            return;
        }

        Quagga.init({
            inputStream: {
                type: "LiveStream",
                constraints: {
                    width: { ideal: 640 },
                    height: { ideal: 480 },
                    facingMode: "environment"
                },
                target: target
            },
            decoder: {
                readers: ["code_128_reader"]
            }
        }, (err) => {
            if (err) {
                console.error("QuaggaJS init error:", err);
                return;
            }
            console.log("QuaggaJS started.");
            Quagga.start();
        });

        Quagga.onDetected((data) => {
            window.barcodeScan.onDetected(data);
        });
    },

    onDetected: function (success) {
        const barcode = success.codeResult?.code;
        if (!barcode) {
            console.warn("No barcode detected.");
            return;
        }

        console.log("Barcode detected:", barcode);

        if (!this.dotNetObj) {
            console.warn("dotNetObj is null. Cannot invoke CodeDetected.");
            return;
        }

        try {
            this.dotNetObj.invokeMethodAsync('CodeDetected', barcode);
            console.log("Blazor method CodeDetected invoked successfully.");

            // 1回だけ呼び出す
            Quagga.stop();
        } catch (error) {
            console.error("Error invoking CodeDetected:", error);
        }
    }
};
