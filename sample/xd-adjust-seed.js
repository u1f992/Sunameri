import * as Sunameri from './modules/Sunameri.core.js'
import { sequences } from 'D:/Documents/Sunameri/sample/xd-sequences.js';

Number.prototype.toUInt32 = function () { const result = newVar(System.UInt32); result.value = this; return result; }
Array.prototype.toUInt32Array = function () { if (this.filter(element => typeof element !== "number").length !== 0) throw new Error("Array must contain only numbers."); const result = newArr(System.UInt32, this.length); this.forEach((value, index) => result[index] = value.toUInt32()); return result; }

/**
 * @typedef {{ reset: Sunameri.Operation[], moveQuickBattle: Sunameri.Operation[], loadParties: Sunameri.Operation[], discardParties: Sunameri.Operation[], entryToBattle: Sunameri.Operation[], exitBattle: Sunameri.Operation[], moveMenu: Sunameri.Operation[], moveOptions: Sunameri.Operation[], enableVibration: Sunameri.Operation[], disableVibration: Sunameri.Operation[], moveContinue: Sunameri.Operation[], load: Sunameri.Operation[], moveSave: Sunameri.Operation[], save: Sunameri.Operation[], moveItems: Sunameri.Operation[], openCloseItems: Sunameri.Operation[], watchSteps: Sunameri.Operation[], finalize: Sunameri.Operation[] }} Sequences
 */
/**
 * 振動設定「あり」にする
 * 
 * 事前条件: XDが起動しており、ソフトリセットできる。
 * 
 * 事後条件: 振動設定が「あり」に変更されている。なお元の設定によって終了位置は異なる。
 * 
 * @param {Sunameri.SerialPortWrapper} controller 
 * @param {Sequences} sequences
 */
function enableVibration(controller, sequences) {

    controller.run([
        [].concat(
            sequences.reset,
            sequences.moveOptions,
            sequences.enableVibration
        )
    ]);
}

/**
 * 初期seed厳選
 * 
 * 事前条件: XDが起動しており、ソフトリセットできる。
 * 
 * 事後条件: 現在のseedが判明し、いますぐバトルの「はい」にカーソルが合っている。
 * 
 * @param {any} targetSeeds 
 * @param {Sunameri.SerialPortWrapper} controller 
 * @param {Sunameri.VideoCaptureWrapper} capture 
 * @param {Sequences} sequences 
 */
function selectSeed(targetSeeds, controller, capture, sequences, tessConfig, detectInfo, trainerInfo) {

    // 1回目捨てまで
    controller.run([
        [].concat(
            sequences.reset,
            sequences.moveQuickBattle,
            sequences.loadParties,
            sequences.discardParties
        )
    ]);

    const currentSeed = getCurrentSeed(controller, capture, sequences, tessConfig, detectInfo, trainerInfo);
    System.Console.WriteLine(currentSeed);
}

/**
 * @typedef {{ player: {icon: {rect: Sunameri.Rect, template: Sunameri.Mat[]}, hp: Sunameri.Rect[]}, com: {icon: {rect: Sunameri.Rect, template: Sunameri.Mat[]}, hp: Sunameri.Rect[]} }} QuickBattleDetectInformation
 * @typedef {{ tsv: any }} TrainerInformation
 */
/**
 * いますぐバトル「さいきょう」にカーソルが合った状態から、seedを求めて返す。
 * 「はい」にカーソルが合った状態で終了する
 * 
 * @param {Sunameri.SerialPortWrapper} controller 
 * @param {Sunameri.VideoCaptureWrapper} capture 
 * @param {Sequences} sequences 
 * @param {Sunameri.TesseractConfig} tessConfig 
 * @param {QuickBattleDetectInformation} detectInfo 
 * @param {TrainerInformation} trainerInfo 
 * @returns {any}
 */
function getCurrentSeed(controller, capture, sequences, tessConfig, detectInfo, trainerInfo = {tsv: (0x10000).toUInt32()}) {

    /** @type {{ Count(): number }} */
    let result;

    controller.run(sequences.loadParties);

    let mat = capture.getFrame();
    let data1 = getQuickBattleData(mat, tessConfig, detectInfo);
    mat.Dispose();

    do {
        controller.run([].concat(sequences.discardParties, sequences.loadParties));

        mat = capture.getFrame();
        let data2 = getQuickBattleData(mat, tessConfig, detectInfo);
        mat.Dispose();

        if (typeof data1 !== 'undefined' && typeof data2 !== 'undefined') {
            result = XDDatabase.SearchSeed(
                data1.pIndex, data1.eIndex, data1.hp.toUInt32Array(),
                data2.pIndex, data2.eIndex, data2.hp.toUInt32Array(),
                trainerInfo.tsv
            );
            if (result.Count === 1)
                break;
        }
        data1 = data2;

    } while (true);

    return result[0].toUInt32();
}

/**
 * 画像からQuickBattleDataを返す
 * 
 * @param {Sunameri.Mat} mat
 * @param {Sunameri.TesseractConfig} tessConfig
 * @param {QuickBattleDetectInformation} detectInfo
 * @returns {QuickBattleData}
 */
function getQuickBattleData(mat, tessConfig, detectInfo) {

    const mat_pIndex = mat.Clone(detectInfo.player.icon.rect);
    const mat_eIndex = mat.Clone(detectInfo.com.icon.rect);
    const mat_hp_0 = mat.Clone(detectInfo.player.hp[0]);
    const mat_hp_1 = mat.Clone(detectInfo.player.hp[1]);
    const mat_hp_2 = mat.Clone(detectInfo.com.hp[0]);
    const mat_hp_3 = mat.Clone(detectInfo.com.hp[1]);

    const pIndex = detectInfo.player.icon.template.findIndex(value => mat_pIndex.Contains(value));
    const eIndex = detectInfo.com.icon.template.findIndex(value => mat_eIndex.Contains(value));
    const hp_0 = parseInt(mat_hp_0.GetOCRResult(tessConfig), 10);
    const hp_1 = parseInt(mat_hp_1.GetOCRResult(tessConfig), 10);
    const hp_2 = parseInt(mat_hp_2.GetOCRResult(tessConfig), 10);
    const hp_3 = parseInt(mat_hp_3.GetOCRResult(tessConfig), 10);

    // const fileName = `${Date.now().toString()}`;
    // mat.SaveImage(`${fileName}.png`);
    // mat_pIndex.SaveImage(`${fileName}-pIndex_${pIndex}.png`);
    // mat_eIndex.SaveImage(`${fileName}-eIndex_${eIndex}.png`);
    // mat_hp_0.SaveImage(`${fileName}-hp_0_${hp_0}.png`);
    // mat_hp_1.SaveImage(`${fileName}-hp_1_${hp_1}.png`);
    // mat_hp_2.SaveImage(`${fileName}-hp_2_${hp_2}.png`);
    // mat_hp_3.SaveImage(`${fileName}-hp_3_${hp_3}.png`);

    if (pIndex === -1 || eIndex === -1) {
        System.Console.WriteLine("getQuickBattleData failed");
        return undefined; // 失敗したときにundefined返すってOKなのか...？
    }

    mat_pIndex.Dispose();
    mat_eIndex.Dispose();
    mat_hp_0.Dispose();
    mat_hp_1.Dispose();
    mat_hp_2.Dispose();
    mat_hp_3.Dispose();

    // HPは相手側から
    const hp = [hp_2, hp_3, hp_0, hp_1];
    const result = {
        pIndex: pIndex,
        eIndex: eIndex,
        hp: hp
    };
    System.Console.WriteLine(JSON.stringify(result));
    return result;
}

(() => {
    System.Console.WriteLine(__dirname);
    const controller = Sunameri.getController({
        portName: 'COM3',
        baudRate: 4800
    });
    const capture = Sunameri.getCapture({
        index: 1,
        width: 1600,
        height: 1200,
        visible: true
    });
    const timer = Sunameri.getTimer();
    timer.sleep(2000);

    const targetSeeds = [
        0xbadface
    ].toUInt32Array();

    /** @type {Sunameri.TesseractConfig} */
    const tessConfig = {
        datapath: 'C:\\Program Files\\Tesseract-OCR\\tessdata\\',
        language: 'xdn',
        charWhitelist: '0123456789',
        oem: 3,
        psmode: 7,
    }

    /** @type {QuickBattleDetectInformation} */
    const detectInfo = {
        player: {
            /** プレイヤー側上のポケモンのアイコン */
            icon: {
                /** 位置 */
                rect: { x: 180, y: 670, width: 130, height: 130 },
                /** 判定に用いるテンプレート */
                template: [
                    new Mat(`${__dirname}/player/0.png`),
                    new Mat(`${__dirname}/player/1.png`),
                    new Mat(`${__dirname}/player/2.png`),
                    new Mat(`${__dirname}/player/3.png`),
                    new Mat(`${__dirname}/player/4.png`)
                ]
            },
            /** プレイヤー側のHP位置 */
            hp: [
                { x: 415, y: 760, width: 130, height: 60 },
                { x: 415, y: 995, width: 130, height: 60 }
            ]
        },
        com: {
            /** COM側上のポケモンのアイコン */
            icon: {
                /** 位置 */
                rect: { x: 860, y: 670, width: 130, height: 130 },
                /** 判定に用いるテンプレート */
                template: [
                    new Mat(`${__dirname}/com/0.png`),
                    new Mat(`${__dirname}/com/1.png`),
                    new Mat(`${__dirname}/com/2.png`),
                    new Mat(`${__dirname}/com/3.png`),
                    new Mat(`${__dirname}/com/4.png`)
                ]
            },
            /** プレイヤー側のHP位置 */
            hp: [
                { x: 1095, y: 760, width: 130, height: 60 },
                { x: 1095, y: 995, width: 130, height: 60 }
            ]
        }
    }

    /** @type {TrainerInformation} */
    const trainerInfo = {
        tsv: (0x10000).toUInt32()
    }

    enableVibration(controller, sequences);
    selectSeed(targetSeeds, controller, capture, sequences, tessConfig, detectInfo, trainerInfo);

    timer.sleep(2000);
    controller.Dispose();
    capture.Dispose();
})();
