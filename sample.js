const controller = new Controller({
    portName: 'COM3',
    baudRate: 4800
});
controller.execute([
    { message: KeyDown.A, wait: 200 },
    { message: KeyUp.A, wait: 200 }
]);

const capture = new VideoCapture({
    index: 1,
    width: 1600,
    height: 1200,
    visible: true
});
new Timer().sleep(5000);
