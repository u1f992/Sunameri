using System.Diagnostics;
using System.IO.Ports;
using System.Text.Json;
using Sunameri;

var app = ConsoleApp.Create(args);
app.AddCommands<SampleApp>();
app.Run();

public class SampleApp : ConsoleAppBase
{
    [Command("record", "Record key inputs.")]
    public void Record
    (
        [Option("p", "Port name connected")] string port
    )
    {
        var sw = new Stopwatch();
        sw.Start();
        long previousMilliseconds = 0;

        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        // Escapeで終了
        Task.Run(() => 
        {
            while (!ct.IsCancellationRequested)
                if (Keyboad.GetKeyState(Key.VK_ESCAPE) < 0)
                {
                    Console.Write(string.Format("{0}}}]\n", sw.ElapsedMilliseconds - previousMilliseconds));
                    cts.Cancel();
                }
        }, ct);

        // ボタンID、対応する入力キー、KeyDown/KeyUp時に送信する内容を定義する
        var db = new Dictionary<string, (char key, char down, char up)>()
        {
            {"A",     ('X',                KeyDown.A, KeyUp.A)},
            {"B",     ('Z',                KeyDown.B, KeyUp.B)},
            {"X",     ('C',                KeyDown.X, KeyUp.X)},
            {"Y",     ('S',                KeyDown.Y, KeyUp.Y)},
            {"L",     ('Q',                KeyDown.L, KeyUp.L)},
            {"R",     ('R',                KeyDown.R, KeyUp.R)},
            {"Z",     ('D',                KeyDown.Z, KeyUp.Z)},
            {"Start", ((char)Key.VK_SPACE, KeyDown.Start, KeyUp.Start)},

            // 十字キー
            {"Left",  ('F', KeyDown.Left,  KeyUp.Left)},
            {"Right", ('H', KeyDown.Right, KeyUp.Right)},
            {"Down",  ('G', KeyDown.Down,  KeyUp.Down)},
            {"Up",    ('T', KeyDown.Up,    KeyUp.Up)},
            
            // コントロールスティック
            {"xAxis_Left",  ((char)Key.VK_LEFT,  xAxis._0,   xAxis._128)},
            {"xAxis_Right", ((char)Key.VK_RIGHT, xAxis._255, xAxis._128)},
            {"yAxis_Down",  ((char)Key.VK_DOWN,  yAxis._0,   yAxis._128)},
            {"yAxis_Up",    ((char)Key.VK_UP,    yAxis._255, yAxis._128)},
            
            // Cスティック
            {"cxAxis_Left",  ('J', cxAxis._0,   cxAxis._128)},
            {"cxAxis_Right", ('L', cxAxis._255, cxAxis._128)},
            {"cyAxis_Down",  ('K', cyAxis._0,   cyAxis._128)},
            {"cyAxis_Up",    ('I', cyAxis._255, cyAxis._128)}
        };

        // 対応するボタンが押されているか判定する
        var pressed = new Dictionary<string, bool>();
        foreach (var data in db) pressed.Add(data.Key, false);

        Console.Write("[");
        // 初回入力までの時間を空入力の待機として再現させる
        Console.Write(new Operation(Special.Empty, 0).ToString()[..^2]);
        
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
                        (bool change, char message) update = (false, '\0');

                        if (pressing && !pressed[data.Key])
                        {
                            // Ctrlが同時に入力されている場合は無視
                            if (Keyboad.GetKeyState(Key.VK_CONTROL) < 0) continue;

                            update = (true, data.Value.down);
                        }
                        else if (!pressing && pressed[data.Key])
                        {
                            update = (true, data.Value.up);
                        }

                        if (!update.change) continue;

                        lock (pressed) pressed[data.Key] = !pressed[data.Key];

                        // 前回の操作との時間差からdurationを算出する
                        Console.Write("{0}}},", sw.ElapsedMilliseconds - previousMilliseconds);
                        previousMilliseconds = sw.ElapsedMilliseconds;

                        var operation = new Operation(update.message, 0);
                        Task.Run(() => controller.Run(operation));

                        // duration以外を書いておく
                        Console.Write(operation.ToString()[..^2]);
                    }
            }, ct).Wait();
        }
    }

    [Command("replay", "Replay the operations.")]
    public void Replay
    (
        [Option("p", "Port name connected")] string port,
        [Option("i", "JSON file input")] string input
    )
    {
        var sequences = JsonSerializer.Deserialize<Operation[]>(File.ReadAllText(input));
        if (sequences == null) return;

        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        // Escで中断
        Task.Run(() => 
        {
            while (!ct.IsCancellationRequested)
                if (Keyboad.GetKeyState(Key.VK_ESCAPE) < 0) cts.Cancel();
        }, ct);

        Task.Run(() => {
            using (var controller = new SerialPort(port, 4800))
            {
                controller.Open();

                var prev = 0;
                for (var i = 0; i < sequences.Length; i++)
                {
                    if (ct.IsCancellationRequested) break;

                    var operation = sequences[i];
                    var msg = string.Format("Execute {0}/{1}, {2}", i + 1, sequences.Length, operation.ToString());
                    Console.Write("\r{0}\r{1}", new string(' ', prev), msg);
                    controller.Run(operation);

                    prev = msg.Length;
                }
                Console.Write("\n");
            }
        }, ct).Wait();
    }
}