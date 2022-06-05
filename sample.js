const controller = new Controller({
    portName: 'COM3',
    baudRate: 4800
});
const capture = new VideoCapture({
    index: 1,
    width: 1600,
    height: 1200,
    visible: true
});
const timer = new Timer();
timer.sleep(5000);

controller.execute([

    // ボタン操作はKeyDown/KeyUpです。
    // 'A' | 'B' | 'X' | 'Y' | 'L' | 'R' | 'Z' | 'Start' | 'Left' | 'Right' | 'Down' | 'Up'
    // keyを配列に詰めると同時押し、同時解放になります。
    // 実際には同時に入力しているわけではないので、精密なことは期待しないでください。
    { type: 'KeyDown', key: ['B', 'X', 'Start'], wait: 1000 },
    { type: 'KeyUp', key: ['B', 'X', 'Start'], wait: 10000 },
    { type: 'KeyDown', key: 'A', wait: 100 },
    { type: 'KeyUp', key: 'A', wait: 20000 },
    { type: 'KeyDown', key: 'A', wait: 100 },
    { type: 'KeyUp', key: 'A', wait: 2500 },
    { type: 'KeyDown', key: 'A', wait: 100 },
    { type: 'KeyUp', key: 'A', wait: 1000 },
    
    // スティック操作はTiltです。
    // xAxisのみ | yAxisのみ | xAxis+yAxis | cxAxisのみ | cyAxisのみ | cxAxis+cyAxis を指定できます。
    // 各プロパティには 0 | 128 | 255 を指定してください。左斜め下が(0,0)です。
    // 実際には同時に入力しているわけではないので、同上
    { type: 'Tilt', xAxis: 255, yAxis: 0, wait: 200 },
    { type: 'Tilt', xAxis: 128, yAxis: 128, wait: 1000 },
    { type: 'Tilt', cxAxis: 0, wait: 100 },
    { type: 'Tilt', cxAxis: 128, wait: 200 },
    { type: 'Tilt', cyAxis: 255, wait: 100 },
    { type: 'Tilt', cyAxis: 128, wait: 1000 },

    { type: 'KeyDown', key: 'A', wait: 100 },
    { type: 'KeyUp', key: 'A', wait: 2000 },
    { type: 'KeyDown', key: 'B', wait: 100 },
    { type: 'KeyUp', key: 'B', wait: 500 },
]);

timer.sleep(5000);
