interface VideoCaptureConfig {
    index: number;
    width: number;
    height: number;
    visible: boolean;
}
declare class VideoCapture {
    constructor(config: VideoCaptureConfig);
    getFrame(): Mat;
    setSizeToShow(rect: Size): void;
    Dispose(): void;
}
