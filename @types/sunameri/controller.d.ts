declare type KeySpecifier = 'A' | 'B' | 'X' | 'Y' | 'L' | 'R' | 'Z' | 'Start' | 'Left' | 'Right' | 'Down' | 'Up';
declare type KeySpecBase = { key: KeySpecifier | KeySpecifier[] };
declare type KeyDown = { type: 'KeyDown' } & KeySpecBase;
declare type KeyUp = { type: 'KeyUp' } & KeySpecBase;

declare type TiltSpecifier = 0 | 128 | 255;
declare type XTilt = { xAxis: TiltSpecifier };
declare type YTilt = { yAxis: TiltSpecifier };
declare type CXTilt = { cxAxis: TiltSpecifier };
declare type CYTilt = { cyAxis: TiltSpecifier };
// xy/cxyそれぞれについて、xのみ | yのみ | xy複合を認める
declare type Tilt = { type: 'Tilt' } & (XTilt | YTilt | (XTilt & YTilt) | CXTilt | CYTilt | (CXTilt & CYTilt));

declare type Operation = (KeyDown | KeyUp | Tilt) & { wait: number };

interface ControllerConfig {
    portName: string;
    baudRate: number;
}
declare class Controller {
    constructor(config: ControllerConfig);
    execute(sequence: Operation[]): void;
}
