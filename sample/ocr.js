import * as Sunameri from "./modules/Sunameri.core.js";

/** @type {Sunameri.Mat} */
const mat = new Mat("1.png");
const trimmed = mat.Clone({
    x: 410,
    y: 760,
    width: 124,
    height: 49
});
// Or
// const trimmed = mat.Clone(new Rect(410, 760, 124, 49));
Cv2.CvtColor(trimmed, trimmed, ColorConversionCodes.RGB2GRAY);
Cv2.BitwiseNot(trimmed, trimmed);
Cv2.Threshold(trimmed, trimmed, 127, 255, ThresholdTypes.Binary);

const result = parseInt(trimmed.GetOCRResult({
    datapath: "C:\\Program Files\\Tesseract-OCR\\tessdata",
    language: "xdn",
    charWhitelist: "0123456789",
    psmode: 7
}).trim());
System.Console.WriteLine(result);
