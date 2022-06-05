using System.Diagnostics;
using System.IO.Ports;
using System.Text.RegularExpressions;
using Sunameri;

ConsoleApp.Run(args, ([Option("p", "Port name connected")] string port) =>
{
    var sw = new Stopwatch();
    sw.Start();
    long previousMilliseconds = 0;

    var cts = new CancellationTokenSource();
    var ct = cts.Token;
    Task.Run(() =>
    {
        while (!ct.IsCancellationRequested)
        {
            // Enterを含む入力を読み捨てる
            Console.ReadKey(true);
            // Escapeで中断
            if (Keyboad.GetKeyState(Key.VK_ESCAPE) < 0)
            {
                Console.Write(string.Format("{0}}}]\n", sw.ElapsedMilliseconds - previousMilliseconds));
                cts.Cancel();
            }
        }
    }, ct);

    // ボタンID、対応する入力キー、KeyDown/KeyUp時に送信する内容を定義する
    var db = new Dictionary<string, (char key, string down, string up)>()
    {
        {"A",     ('X',                 KeyDown.A, KeyUp.A)},
        {"B",     ('Z',                 KeyDown.B, KeyUp.B)},
        {"X",     ('C',                 KeyDown.X, KeyUp.X)},
        {"Y",     ('S',                 KeyDown.Y, KeyUp.Y)},
        {"L",     ('Q',                 KeyDown.L, KeyUp.L)},
        {"R",     ('R',                 KeyDown.R, KeyUp.R)},
        {"Z",     ('D',                 KeyDown.Z, KeyUp.Z)},
        {"Start", ((char)Key.VK_RETURN, KeyDown.Start, KeyUp.Start)},

        // 十字キー
        {"Left",  ('F', KeyDown.Left,  KeyUp.Left)},
        {"Right", ('H', KeyDown.Right, KeyUp.Right)},
        {"Down",  ('G', KeyDown.Down,  KeyUp.Down)},
        {"Up",    ('T', KeyDown.Up,    KeyUp.Up)},
        
        // コントロールスティック
        {"\"xAxis\":0",   ((char)Key.VK_LEFT,  xAxis._0,   xAxis._128)},
        {"\"xAxis\":255", ((char)Key.VK_RIGHT, xAxis._255, xAxis._128)},
        {"\"yAxis\":0",   ((char)Key.VK_DOWN,  yAxis._0,   yAxis._128)},
        {"\"yAxis\":255", ((char)Key.VK_UP,    yAxis._255, yAxis._128)},
        
        // Cスティック
        {"\"cxAxis\":0",   ('J', cxAxis._0,   cxAxis._128)},
        {"\"cxAxis\":255", ('L', cxAxis._255, cxAxis._128)},
        {"\"cyAxis\":0",   ('K', cyAxis._0,   cyAxis._128)},
        {"\"cyAxis\":255", ('I', cyAxis._255, cyAxis._128)}
    };

    // 対応するボタンが押されているか判定する
    var pressed = new Dictionary<string, bool>();
    foreach (var data in db) pressed.Add(data.Key, false);

    Console.Write("[");

    using (var controller = new SerialPort(port, 4800))
    {
        controller.Open();

        Task.Run(() =>
        {
            while (!ct.IsCancellationRequested)
                foreach (var data in db)
                {
                    // shortの最上位bitで判定する
                    var pressing = Keyboad.GetKeyState(data.Value.key) < 0;
                    (bool change, string message) update = (false, "");
                    var type = "";
                    var tiltBack = false;

                    if (pressing && !pressed[data.Key])
                    {
                        // Ctrlが同時に入力されている場合は無視
                        if (Keyboad.GetKeyState(Key.VK_CONTROL) < 0) continue;

                        update = (true, data.Value.down);
                        type = "KeyDown";
                    }
                    else if (!pressing && pressed[data.Key])
                    {
                        update = (true, data.Value.up);
                        type = "KeyUp";
                    }
                    if (data.Key.Contains("Axis"))
                    {
                        type = "Tilt";
                        tiltBack = !pressing ? true : false;
                    }

                    if (!update.change) continue;

                    lock (pressed) pressed[data.Key] = !pressed[data.Key];

                    // 前回の操作との時間差からwaitを算出する
                    if (previousMilliseconds != 0)
                    {
                        // 初回は不要
                        Console.Write("{0}}},", sw.ElapsedMilliseconds - previousMilliseconds);
                    }
                    previousMilliseconds = sw.ElapsedMilliseconds;

                    Task.Run(() => controller.WriteLine(update.message));

                    // wait以外を書いておく
                    // waitを0にすると、常に2文字削ればwaitを消せる。
                    var json = (string type, string id) => {
                        var json = "";
                        switch (type)
                        {
                            case "KeyDown":
                            case "KeyUp":
                                json = string.Format("{{\"type\":\"{0}\",\"key\":\"{1}\",\"wait\":0}}", type, id);
                                break;
                            case "Tilt":
                                json = string.Format("{{\"type\":\"{0}\",{1},\"wait\":0}}", type, tiltBack ? Regex.Replace(id, ":[0-9]+$", ":128") : id);
                                break;
                        }
                        return json;
                    };
                    var id = data.Key;
                    Console.Write(json(type, id)[..^2]);
                }
        }, ct).Wait();
    }
});
