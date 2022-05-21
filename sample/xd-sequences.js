import { KeyDown, KeyUp, xAxis, yAxis } from './modules/Sunameri.mapping.js'

/**
 * @typedef {{ reset: Sunameri.Operation[], moveQuickBattle: Sunameri.Operation[], loadParties: Sunameri.Operation[], discardParties: Sunameri.Operation[], entryToBattle: Sunameri.Operation[], exitBattle: Sunameri.Operation[], moveMenu: Sunameri.Operation[], moveOptions: Sunameri.Operation[], enableVibration: Sunameri.Operation[], disableVibration: Sunameri.Operation[], moveContinue: Sunameri.Operation[], load: Sunameri.Operation[], moveSave: Sunameri.Operation[], save: Sunameri.Operation[], moveItems: Sunameri.Operation[], openCloseItems: Sunameri.Operation[], watchSteps: Sunameri.Operation[], finalize: Sunameri.Operation[] }} Sequences
 */
/**
 * 自動化に使用する各種動作を定義する
 * 
 * @type {Sequences}
 */
export const sequences = {
    /**
     * B+X+Stでソフトリセットし、「つづきをあそぶ」まで
     * @type {Sunameri.Operation[]}
     */
    reset: [
        { message: KeyDown.B, wait: 50 },
        { message: KeyDown.X, wait: 50 },
        { message: KeyDown.Start, wait: 1000 },
        { message: KeyUp.Start, wait: 50 },
        { message: KeyUp.X, wait: 50 },
        { message: KeyUp.B, wait: 10000 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 20000 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 2500 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1000 }
    ],
    /**
     * 「つづきをあそぶ」-> いますぐバトル「さいきょう」まで
     * @type {Sunameri.Operation[]}
     */
    moveQuickBattle: [
        { message: xAxis._255, wait: 100 },
        { message: xAxis._128, wait: 800 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1500 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 2000 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1500 }
    ],
    /**
     * 「さいきょう」を選択し手持ちを生成
     * @type {Sunameri.Operation[]}
     */
    loadParties: [
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1500 }
    ],
    /**
     * いますぐバトルのパーティが表示されている画面から、B押して破棄
     * @type {Sunameri.Operation[]}
     */
    discardParties: [
        { message: KeyDown.B, wait: 300 },
        { message: KeyUp.B, wait: 1500 }
    ],
    /**
     * いますぐバトルのパーティが表示されている画面から、「はい」を押して戦闘が開始し、操作可能になるまで待機
     * @type {Sunameri.Operation[]}
     */
    entryToBattle: [
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 7000 },
        { message: KeyDown.B, wait: 100 },
        { message: KeyUp.B, wait: 4000 },
        { message: KeyDown.B, wait: 100 },
        { message: KeyUp.B, wait: 26000 }
    ],
    /**
     * 戦闘を降参で離脱
     * @type {Sunameri.Operation[]}
     */
    exitBattle: [
        { message: KeyDown.Start, wait: 100 },
        { message: KeyUp.Start, wait: 500 },
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 500 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 12000 },
        { message: KeyDown.B, wait: 100 },
        { message: KeyUp.B, wait: 4000 }
    ],
    /**
     * いますぐバトル「さいきょう」->「つづきをあそぶ」まで
     * @type {Sunameri.Operation[]}
     */
    moveMenu: [
        { message: KeyDown.B, wait: 100 },
        { message: KeyUp.B, wait: 800 },
        { message: KeyDown.B, wait: 100 },
        { message: KeyUp.B, wait: 1500 },
        { message: KeyDown.B, wait: 100 },
        { message: KeyUp.B, wait: 2000 },
        { message: xAxis._0, wait: 100 },
        { message: xAxis._128, wait: 1000 }
    ],
    /**
     * 「つづきをあそぶ」->「せってい」
     * @type {Sunameri.Operation[]}
     */
    moveOptions: [
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 1000 }
    ],
    /**
     * 「せってい」-> 振動onにして「せってい」まで
     * @type {Sunameri.Operation[]}
     */
    enableVibration: [
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1500 },
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 300 },
        { message: xAxis._0, wait: 100 },
        { message: xAxis._128, wait: 300 },
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 300 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 500 },
        { message: yAxis._255, wait: 100 },
        { message: yAxis._128, wait: 300 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 8000 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 2000 }
    ],
    /**
     * 「せってい」-> 振動offにして「せってい」まで
     * @type {Sunameri.Operation[]}
     */
    disableVibration: [
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1500 },
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 300 },
        { message: xAxis._255, wait: 100 },
        { message: xAxis._128, wait: 300 },
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 300 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 500 },
        { message: yAxis._255, wait: 100 },
        { message: yAxis._128, wait: 300 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 8000 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 2000 }
    ],
    /**
     * 「せってい」->「つづきをあそぶ」
     * @type {Sunameri.Operation[]}
     */
    moveContinue: [
        { message: yAxis._255, wait: 100 },
        { message: yAxis._128, wait: 2000 }
    ],
    /**
     * 「つづきをあそぶ」-> ロードを待ってメニューを開く
     * @type {Sunameri.Operation[]}
     */
    load: [
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 2000 },
        { message: yAxis._255, wait: 100 },
        { message: yAxis._128, wait: 500 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 6000 },
        { message: KeyDown.X, wait: 100 },
        { message: KeyUp.X, wait: 1000 }
    ],
    /**
     * 「ポケモン」->「レポート」
     * @type {Sunameri.Operation[]}
     */
    moveSave: [
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 100 },
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 100 },
        { message: yAxis._0, wait: 100 },
        { message: yAxis._128, wait: 1000 }
    ],
    /**
     * レポートを書いて、「レポート」に戻るまで
     * @type {Sunameri.Operation[]}
     */
    save: [
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1000 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1000 },
        { message: yAxis._255, wait: 100 },
        { message: yAxis._128, wait: 100 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 10000 },
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 1000 }
    ],
    /**
     * 「レポート」->「もちもの」
     * @type {Sunameri.Operation[]}
     */
    moveItems: [
        { message: yAxis._255, wait: 100 },
        { message: yAxis._128, wait: 1000 }
    ],
    /**
     * 持ち物を開いて閉じる
     * @type {Sunameri.Operation[]}
     */
    openCloseItems: [
        { message: KeyDown.A, wait: 100 },
        { message: KeyUp.A, wait: 2000 },
        { message: KeyDown.B, wait: 100 },
        { message: KeyUp.B, wait: 2500 }
    ],
    /**
     * メニューを閉じて主人公の腰振りを見て、再度メニューを開く
     * @type {Sunameri.Operation[]}
     */
    watchSteps: [
        { message: KeyDown.B, wait: 100 },
        { message: KeyUp.B, wait: 13000 },
        { message: KeyDown.X, wait: 100 },
        { message: KeyUp.X, wait: 1000 }
    ],
    /**
     * seedが調整されてプログラムが終了する状態から行う、任意の動作を定義できます。
     * @type {Sunameri.Operation[]}
     */
    finalize: []
}
