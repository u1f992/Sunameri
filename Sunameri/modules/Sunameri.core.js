/**
 * @typedef {{ sleep(millisecondsTimeout: number): void, start(): Task, start(milliseconds: number): Task<void>, submit(milliseconds: number): void, stop(): void }} Timer
 * @typedef {{ Wait(): void }} Task
 */
/**
 * タイマーを取得する
 * @returns {Timer}
 */
export function getTimer() {
    return new Timer();
}

/**
 * @typedef {{ message: string, wait: number }} Operation
 * @typedef {{ portName: string, baudRate: number }} SerialPortConfig
 * @typedef {{ run(sequence: Array<Operation>): void, Dispose(): void }} SerialPortWrapper
 */
/**
 * コントローラーを取得する
 * @param {SerialPortConfig} config 
 * @returns {SerialPortWrapper}
 */
export function getController(config) {
    
    System.Console.Write("Initializing controller... ");
    const controller = new SerialPortWrapper(config.portName, config.baudRate);
    System.Console.WriteLine("done");

    return controller;
}

/**
 * @typedef {{ x: number, y: number, width: number, height: number }} Rect
 * @typedef {{ fx: number, fy: number }} Ratio
 * @typedef {{ width: number, height: number }} Size
 * @typedef {{ index: number, width: number, height: number, visible: boolean }} VideoCaptureConfig
 * @typedef {{ datapath?: string, language?: string, charWhitelist?: string, oem?: number, psmode?: number }} TesseractConfig
 * @typedef {{ Clone(rect: Rect): Mat, Resize(ratio: Ratio): Mat, Resize(size: Size): Mat, Contains(source: Mat, threshold?: number): boolean, GetOCRResult(tessConfig: TesseractConfig): string, ToStream(fileName: string): object, Dispose(): void }} Mat
 * @typedef {{ getFrame(): Mat, setSizeToShow(width: number, height: number): void, Dispose(): void }} VideoCaptureWrapper
 */
/**
 * キャプチャデバイスを取得する
 * @param {VideoCaptureConfig} config 
 * @returns {VideoCaptureWrapper}
 */
export function getCapture(config) {

    System.Console.Write("Initializing capture... ");
    const capture = new VideoCaptureWrapper(config.index, config.width, config.height, config.visible);
    System.Console.WriteLine("done");
    
    return capture;
}
