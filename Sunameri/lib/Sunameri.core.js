/**
 * @typedef {{ message: string, wait: number }} Operation
 * @typedef {{ portName: string, baudRate: number }} SerialPortConfig
 * @typedef {{ write(operation: Operation): void, write(sequence: Array<Operation>): void, wait(millisecondsTimeout: number): void, Dispose(): void }} SerialPort
 */

/**
 * コントローラーを取得する
 * @param {SerialPortConfig} config 
 * @returns {SerialPort}
 */
export function getController(config) {

    const controller = new SerialPort(config.portName, config.baudRate)
    controller.DtrEnable = true;
    controller.Open();

    return controller;
}
/**
 * @typedef {{ index: number, width: number, height: number, visible: boolean }} VideoCaptureConfig
 * @typedef {{ trim(rect: Rect): Mat, save(fileName: string): boolean, getSimilarity(fileName: string): number, getSimilarity(template: Mat): number, Dispose(): void }} Mat
 * @typedef {{ x: number, y: number, width: number, height: number }} Rect
 * @typedef {{ getFrame(): Mat, getFrame(rect: Rect): Mat, Dispose(): void }} VideoCaptureWrapper
 */
/**
 * キャプチャデバイスを取得する
 * @param {VideoCaptureConfig} config 
 * @returns {VideoCaptureWrapper}
 */
export function getCapture(config) {

    return new VideoCaptureWrapper(config.index, config.width, config.height, config.visible);
}
