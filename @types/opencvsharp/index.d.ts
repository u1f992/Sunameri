interface Rect {
    x: number;
    y: number;
    width: number;
    height: number;
}
interface Size {
    width: number;
    height: number;
}
interface Ratio {
    fx: number;
    fy: number;
}
declare class Mat {
    constructor(fileName: string);
    Clone(): Mat;
    Clone(rect: Rect): Mat;
    Resize(ratio: Ratio): Mat;
    Resize(rect: Rect): Mat;
    Contains(source: Mat, threshold?: number): boolean;
    ToStream(fileName: { out: string }): Stream;
    GetOCRResult(tessConfig: TesseractConfig): string;
    Dispose(): void;
}
interface TesseractConfig {
    datapath?: string;
    language?: string;
    charWhitelist?: string;
    oem?: number;
    psmode?: number;
}
