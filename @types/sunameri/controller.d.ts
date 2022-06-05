declare type KeySpecifier = 'A' | 'B' | 'X' | 'Y' | 'L' | 'R' | 'Z' | 'Start' | 'Left' | 'Right' | 'Down' | 'Up';
declare type KeySpecBase = { key: KeySpecifier | KeySpecifier[] };
declare type KeyDown = { type: 'KeyDown' } & KeySpecBase;
declare type KeyUp = { type: 'KeyUp' } & KeySpecBase;
declare type TiltSpecifier = 0 | 128 | 255;
declare type XTilt = { xAxis: TiltSpecifier };
declare type YTilt = { yAxis: TiltSpecifier };
declare type Tilt = { type: 'Tilt' } & (XTilt | YTilt | XTilt & YTilt);
declare type CXTilt = { cxAxis: TiltSpecifier };
declare type CYTilt = { cyAxis: TiltSpecifier };
declare type CTilt = { type: 'CTilt' } & (CXTilt | CYTilt | CXTilt & CYTilt);
declare type Operation = (KeyDown | KeyUp | Tilt | CTilt) & { wait: number };
interface ControllerConfig {
    portName: string;
    baudRate: number;
}
declare class Controller {
    constructor(config: ControllerConfig);
    execute(sequence: Operation[]): void;
}
