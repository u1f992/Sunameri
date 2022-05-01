import * as Sunameri from "./lib/Sunameri.core.js";
import { KeyDown, KeyUp, xAxis, yAxis, cxAxis, cyAxis } from "./lib/Sunameri.mapping.js";

// .NETのSystem以下のクラスを利用できます。
System.Console.WriteLine("Hello Sunameri");

// Sunameri.core.jsで定義される以下のメソッドで、ハードウェアを初期化できます。
const controller = Sunameri.getController({
    portName: "COM8",
    baudRate: 4800
});
const capture = Sunameri.getCapture({
    index: 0,
    width: 1600,
    height: 1200,
    visible: true
});

// SerialPortの実体は.NETのSerialPortそのものです。
// プロパティやメソッドはそのまま利用できますが、自動化に有用な内容は拡張メソッドで生やしてあります。
controller.wait(5000);
controller.write([
    { message: KeyDown.A, wait: 200 },
    { message: KeyUp.A, wait: 2000 }
]);

// VideoCaptureの実体はVideoCaptureWrapperです。
// visible: trueでインスタンスを初期化した場合は常に画像を表示する他、表示中はsを入力すると現在のMatをファイルに保存します。
// getFrameで現在のMatを取得できます。
const full = capture.getFrame();
/** @type {Sunameri.Rect} */
const rect = {
    x: 894,
    y: 236,
    width: 183,
    height: 153
};
const trimmed = capture.getFrame(rect);

// Matの実体はOpenCvSharp.Matそのものです。
// 有用そうなものは拡張メソッドで切り出しています。
trimmed.save("test_trimmed.png");
let tmp = [];
tmp.push(full.trim(rect));
tmp.push(full.trim({x: 1402, y: 10, width: 183, height: 153}));

System.Console.WriteLine(`similarity: ${tmp[0].getSimilarity("test_trimmed.png")} is very high.`); // ファイル名で指定
System.Console.WriteLine(`similarity: ${trimmed.getSimilarity(tmp[1])} is low.`); // Matで指定

// JavaScriptにはusingがないので、手動でDisposeし忘れると簡単にメモリリークします。
tmp.map(mat => mat.Dispose());
full.Dispose();
trimmed.Dispose();

// const result = trimmed.getOCRResult();
controller.wait(3000);

controller.Dispose();
capture.Dispose();
