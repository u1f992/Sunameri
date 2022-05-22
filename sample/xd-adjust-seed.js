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
 * @typedef {{ pivotSeed: System.UInt32, targetSeed: System.UInt32, waitTime: System.TimeSpan }} SelectedResult
 */
/**
 * 初期seed厳選
 * 
 * 事前条件: XDが起動しており、ソフトリセットできる。
 * 
 * 事後条件: 現在のseedが判明し、いますぐバトルの「はい」にカーソルが合っている。
 * 
 * @param {number[]} targetSeeds 
 * @param {Sunameri.SerialPortWrapper} controller 
 * @param {Sunameri.VideoCaptureWrapper} capture 
 * @param {Sequences} sequences 
 * @param {Sunameri.TesseractConfig} tessConfig 
 * @param {QuickBattleDetectInformation} detectInfo 
 * @param {TrainerInformation} trainerInfo 
 * @param {AdvanceInformation} advanceInfo 
 * @param {WaitTimeInformation} waitTimeInfo 
 * @returns {SelectedResult}
 */
function selectSeed(targetSeeds, controller, capture, sequences, tessConfig, detectInfo, trainerInfo, advanceInfo, waitTimeInfo) {

    /** @type {System.UInt32} */
    let currentSeed;

    do {
        // 1回目捨てまで
        controller.run([
            [].concat(
                sequences.reset,
                sequences.moveQuickBattle,
                sequences.loadParties,
                sequences.discardParties
            )
        ]);

        // getCurrentSeedが失敗したらリセット
        // = 5回連続でgetQuickBattleDataが失敗
        let flag = false;
        try {
            currentSeed = getCurrentSeed(controller, capture, sequences, tessConfig, detectInfo, trainerInfo);
        } catch (error) {
            System.Console.Error.WriteLine(error.message);
            flag = true;
        }
        if (flag) {
            continue;
        }
        
        for (let i = 0; i < targetSeeds.length; i++) {

            const targetSeed = targetSeeds[i].toUInt32();
            const waitTime = getWaitTime(currentSeed, targetSeed, advanceInfo);

            if (isCloseEnough(waitTime, waitTimeInfo)) {
                return {
                    pivotSeed: currentSeed,
                    targetSeed: targetSeed,
                    waitTime: waitTime
                };
            }
        }
    } while (true);
}

/**
 * @typedef {{ faster: {perSecond: number}, afterLoad?: {byLoading: number, byOpeningItems: number} }} AdvanceInformation
 * @typedef {{ maximum: System.TimeSpan, minimum: System.TimeSpan }} WaitTimeInformation
 */
/**
 * waitTimeがminimum以上maximum以下
 * 
 * @param {System.TimeSpan} waitTime
 * @param {WaitTimeInformation} waitTimeInfo 
 * @returns {boolean}
 */
function isCloseEnough(waitTime, waitTimeInfo) {

    // C#側の演算子のオーバーロードはClearScript側から利用できない
    const longerThanMinimum = waitTime.CompareTo(waitTimeInfo.minimum) >= 0;
    const shorterThanMaximum = waitTime.CompareTo(waitTimeInfo.maximum) <= 0;
    
    return longerThanMinimum && shorterThanMaximum;
}

/**
 * advanceInfoに基づいてseed間の待機時間を算出する
 * 
 * @param {System.UInt32} currentSeed 
 * @param {System.UInt32} targetSeed 
 * @param {AdvanceInformation} advanceInfo 
 * @returns {System.TimeSpan}
 */
function getWaitTime(currentSeed, targetSeed, advanceInfo) {
    return System.TimeSpan.FromSeconds(GCLCGExtension.GetIndex(targetSeed, currentSeed) / advanceInfo.faster.perSecond);
}

/**
 * @typedef {{ player: {icon: {rect: Sunameri.Rect, template: Sunameri.Mat[]}, hp: Sunameri.Rect[]}, com: {icon: {rect: Sunameri.Rect, template: Sunameri.Mat[]}, hp: Sunameri.Rect[]} }} QuickBattleDetectInformation
 * @typedef {{ tsv: System.UInt32 }} TrainerInformation
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
 * @throws {Error} 5回連続でgetQuickBattleDataの取得に失敗した場合
 * @returns {System.UInt32}
 */
function getCurrentSeed(controller, capture, sequences, tessConfig, detectInfo, trainerInfo) {

    /** @type {{ Count(): number }} */
    let result;

    let count = 0;

    controller.run(sequences.loadParties);

    let mat = capture.getFrame();
    let data1
    try {
        data1 = getQuickBattleData(mat, tessConfig, detectInfo);
    } catch (error) {
        System.Console.Error.WriteLine(error.message);
        count++;

        data1 = undefined;
    } finally {
        mat.Dispose();
    }
    
    do {
        controller.run([].concat(sequences.discardParties, sequences.loadParties));

        mat = capture.getFrame();
        let data2;
        try {
            data2 = getQuickBattleData(mat, tessConfig, detectInfo);
            count = 0;
        } catch (error) {
            // 5回連続で失敗したらthrow
            System.Console.Error.WriteLine(error.message);
            count++;
            if (count === 5) {
                throw new Error(`getQuickBattleData failed ${count} times in a row.`);
            }

            data2 = undefined;
        } finally {
            mat.Dispose();
        }

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

    // なぜかnumberが返ってくるんだよな...
    return result[0].toUInt32();
}

/** @typedef {{ pIndex: number, eIndex: number, hp: number[] }} QuickBattleData */
/**
 * 画像からQuickBattleDataを返す
 * 
 * @param {Sunameri.Mat} mat
 * @param {Sunameri.TesseractConfig} tessConfig
 * @param {QuickBattleDetectInformation} detectInfo
 * @throws {Error} 不正な画像の場合・いますぐバトルが表示されていない場合 など
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
        throw new Error("getQuickBattleData failed");
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

/**
 * 
 * @param {SelectedResult} selectedResult 
 * @param {Sunameri.SerialPortWrapper} controller 
 * @param {Sunameri.VideoCaptureWrapper} capture 
 * @param {Sequences} sequences 
 * @param {Sunameri.TesseractConfig} tessConfig 
 * @param {QuickBattleDetectInformation} detectInfo 
 * @param {TrainerInformation} trainerInfo 
 * @param {WaitTimeInformation} waitTimeInfo  
 */
function adjustSeed(selectedResult, controller, capture, sequences, tessConfig, detectInfo, trainerInfo, waitTimeInfo) {

    advanceByMoltres(selectedResult, controller, capture, sequences, tessConfig, detectInfo, waitTimeInfo);

    /** @type {System.UInt32} */
    const currentSeed = getCurrentSeed(controller, capture, sequences, tessConfig, detectInfo, trainerInfo);
    
    System.Console.WriteLine(JSON.stringify({currentSeed: currentSeed.value}));

}

function advanceByMoltres(selectedResult, controller, capture, sequences, tessConfig, detectInfo, waitTimeInfo) {

    // ファイヤーが出るまで再生成
    while (true) {
        const mat = capture.getFrame();
        let quickBattleData;
        try {
            quickBattleData = getQuickBattleData(mat, tessConfig, detectInfo)
        } catch (error) {
            System.Console.Error.WriteLine(error.message);
            quickBattleData = undefined;
        } finally {
            mat.Dispose();
        }

        if (typeof quickBattleData !== 'undefined') {
            if (quickBattleData.eIndex === 2) {
                break;
            }    
        }

        controller.run(
            [].concat(
                sequences.discardParties,
                sequences.loadParties
            )
        );
    }

    // 戦闘入って出る
    controller.run(sequences.entryToBattle);
    const milliseconds = Math.floor(selectedResult.waitTime.Subtract(waitTimeInfo.minimum).TotalMilliseconds);
    Sunameri.getTimer().sleep(milliseconds);

    // 高速消費後1回分捨てる
    controller.run(
        [].concat(
            sequences.exitBattle,
            sequences.loadParties,
            sequences.discardParties
        )
    );
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
        0x0,
        0xbadface,
        0xdeadbeef,
        0xFFFFFFFF
    ];

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

    /** @type {AdvanceInformation} */
    const advanceInfo = {
        /** 高速消費 */
        faster: {
            /**
             * 消費速度 (消費/s)
             * 
             * 「いますぐバトル」でファイヤーを見る場合: 約3842消費/s
             */
            perSecond: 3842
        }//,
        /**
         * ロード後の消費
         * 
         * ロードさせない場合は定義しない
         */
        // afterLoad: {
        //     /** ロードにかかる消費 (オブジェクトの読み込みなど？) */
        //     byLoading: number,
        //     /** 「もちもの」消費 */
        //     byOpeningItems: number
        // }
    }

    /** @type {WaitTimeInformation} */
    const waitTimeInfo = {
        maximum: System.TimeSpan.Parse("0.03:00:00"),
        minimum: System.TimeSpan.Parse("0.00:02:00")
    }

    /** @type {TrainerInformation} */
    const trainerInfo = {
        tsv: (0x10000).toUInt32()
    }

    enableVibration(controller, sequences);
    const result = selectSeed(targetSeeds, controller, capture, sequences, tessConfig, detectInfo, trainerInfo, advanceInfo, waitTimeInfo);
    System.Console.WriteLine(JSON.stringify({pivotSeed: result.pivotSeed.value, targetSeed: result.targetSeed.value, waitTime: result.waitTime.ToString()}));
    adjustSeed(result, controller, capture, sequences, tessConfig, detectInfo, trainerInfo, waitTimeInfo);

    timer.sleep(2000);
    controller.Dispose();
    capture.Dispose();
})();
