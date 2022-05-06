/**
 * @typedef {{ message: string, wait: number }} Operation
 * @typedef {{ portName: string, baudRate: number }} SerialPortConfig
 * @typedef {{ run(sequence: Array<Operation>): void, sleep(millisecondsTimeout: number): void, Dispose(): void }} SerialPortWrapper
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
 * @typedef {{ index: number, width: number, height: number, visible: boolean }} VideoCaptureConfig
 * @typedef {{ trim(rect: Rect): Mat, getSimilarity(fileName: string): number, getSimilarity(template: Mat): number, Dispose(): void }} Mat
 * @typedef {{ x: number, y: number, width: number, height: number }} Rect
 * @typedef {{ getFrame(): Mat, Dispose(): void }} VideoCaptureWrapper
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
