/**
 * メモリリーク確認用
 * 10分くらい表示してみる
 */

import * as Sunameri from "./lib/Sunameri.core.js";

const config = {
    /** @type {Sunameri.SerialPortConfig} */
    controller: {
        portName: "COM8",
        baudRate: 4800
    },
    /** @type {Sunameri.VideoCaptureConfig} */
    capture: {
        index: 0,
        width: 1600,
        height: 1200,
        visible: true
    }
}
const controller = Sunameri.getController(config.controller);
const capture = Sunameri.getCapture(config.capture);

controller.wait(10 * 60 * 1000);
