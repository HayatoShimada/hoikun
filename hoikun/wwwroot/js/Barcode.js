window.generateBarcode = (elementId, barcodeValue) => {
    console.log("generateBarcode called for:", elementId, "with value:", barcodeValue);

    if (!barcodeValue) {
        console.warn("Barcode value is empty.");
        return;
    }

    const target = document.getElementById(elementId);
    if (!target) {
        console.error("Target element not found:", elementId);
        return;
    }

    JsBarcode("#" + elementId, barcodeValue, {
        format: "CODE128",
        displayValue: true,
        lineColor: "#000",
        width: 2,
        height: 50
    });

    console.log("Barcode generated successfully for:", elementId);
};
