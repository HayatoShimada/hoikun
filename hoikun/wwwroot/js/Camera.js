window.barcodeScan = {
    // Blazor 側の DotNetObjectReference を保持するための変数
    dotNetObj: null,

    // バーコードのキャプチャ開始
    startCapture: function (dotNetObj) {
        // 参照を保持
        this.dotNetObj = dotNetObj;

        // QuaggaJS 初期化
        Quagga.init({
            inputStream: {
                type: "LiveStream",
                constraints: {
                    facingMode: "environment"  // 背面カメラ (スマホ等)
                },
                // カメラ映像を表示するターゲット要素
                target: document.querySelector('#camera')
            },
            decoder: {
                readers: ["ean_reader"]  // EAN(ISBN)を読む
            }
        }, (err) => {
            if (err) {
                console.log(err);
                return;
            }
            // バーコードスキャン開始
            Quagga.start();
        });

        // バーコードが検出されたときのイベントを登録
        Quagga.onDetected((data) => {
            window.barcodeScan.onDetected(data);
        });
    },

    // バーコード検知時の処理
    onDetected: function (success) {
        const isbn = success.codeResult.code;
        // ISBNは通常 "978" で始まる (978-4-xx)
        if (isbn && isbn.startsWith("978")) {
            if (this.dotNetObj) {
                // Blazor側(C#)に「バーコードを読み取った」ことを通知
                this.dotNetObj.invokeMethod('CodeDetected', isbn);
            }
            // 1回検出したら止めたい場合は、Quagga.stop() など呼んでもOK
            // Quagga.stop();
        }
    }
};
