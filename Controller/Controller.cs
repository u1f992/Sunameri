namespace Sunameri;

using System.IO.Ports;
using System.Text;
using Microsoft.ClearScript;
using NLog;

public class Controller
{
    static Logger _logger = LogManager.GetCurrentClassLogger();

    // たぶんボーレート依存だが計算がめんどくさいので適当
    const int MinimumWaitTime = 10;

    SerialPort _serialPort;
    Timer _timer = new Timer();
    string _buffer = "";
    object lockObject = new Object();
    readonly Dictionary<string, string> messagedb = new Dictionary<string, string> {
        { "KeyDown.A", "a" },
        { "KeyDown.B", "b" },
        { "KeyDown.X", "c" },
        { "KeyDown.Y", "d" },
        { "KeyDown.L", "e" },
        { "KeyDown.R", "f" },
        { "KeyDown.Z", "g" },
        { "KeyDown.Start", "h" },
        { "KeyDown.Left", "i" },
        { "KeyDown.Right", "j" },
        { "KeyDown.Down", "k" },
        { "KeyDown.Up", "l" },
        { "KeyUp.A", "m" },
        { "KeyUp.B", "n" },
        { "KeyUp.X", "o" },
        { "KeyUp.Y", "p" },
        { "KeyUp.L", "q" },
        { "KeyUp.R", "r" },
        { "KeyUp.Z", "s" },
        { "KeyUp.Start", "t" },
        { "KeyUp.Left", "u" },
        { "KeyUp.Right", "v" },
        { "KeyUp.Down", "w" },
        { "KeyUp.Up", "x" },
        { "xAxis.0", "y" },
        { "xAxis.128", "z" },
        { "xAxis.255", "1" },
        { "yAxis.0", "2" },
        { "yAxis.128", "3" },
        { "yAxis.255", "4" },
        { "cxAxis.0", "5" },
        { "cxAxis.128", "6" },
        { "cxAxis.255", "7" },
        { "cyAxis.0", "8" },
        { "cyAxis.128", "9" },
        { "cyAxis.255", "0" }
    };

    public Controller(ScriptObject config) : this((string)config.GetProperty("portName"), (int)config.GetProperty("baudRate")) { }
    public Controller(string portName, int baudRate)
    {
        _serialPort = new SerialPort(portName, baudRate);

        // https://www.arduino.cc/reference/en/language/functions/communication/serial/println/
        _serialPort.Encoding = Encoding.UTF8;
        _serialPort.NewLine = "\r\n";

        _serialPort.DtrEnable = true;
        _serialPort.DataReceived += (object sender, SerialDataReceivedEventArgs e) =>
        {
            var serialPort = (SerialPort)sender;
            if (!serialPort.IsOpen)
                return;

            // _bufferに追加する
            var buffer = serialPort.ReadExisting();
            lock (lockObject) _buffer += buffer;

            // _bufferの最初の改行までをログに書く
            var newline = serialPort.NewLine;
            var split = _buffer.Split(newline);
            if (split.Length > 1)
            {
                var toWrite = split[0];
                _logger.Trace(toWrite);

                // 残りは_bufferに返す
                lock (lockObject) _buffer = _buffer[(toWrite.Length + newline.Length)..];
            }
        };

        _serialPort.Open();
    }
    public void execute(ScriptObject scriptObject)
    {
        // https://github.com/microsoft/ClearScript/issues/131
        dynamic _scriptObject = scriptObject;
        if (_scriptObject.constructor.name == "Array")
        {
            var sequence = _scriptObject;
            for (var i = 0; i < sequence.length; i++)
                executeOperation((ScriptObject)sequence[i]);
        }
        else
        {
            throw new Exception("Object must be Array.");
        }
    }
    /// <summary>
    /// JavaScriptから与えられたOperationを、executeKeyInputとexecuteTiltInputに振り分ける
    /// </summary>
    /// <param name="operation"></param>
    void executeOperation(ScriptObject operation)
    {
        var propertyNames = operation.PropertyNames;
        if (!propertyNames.Contains("type") || !propertyNames.Contains("wait"))
            throw new Exception("Object must contain the properties type and wait.");

        var type = (string)operation.GetProperty("type");
        var wait = (int)operation.GetProperty("wait");

        switch (type)
        {
            case "KeyDown":
            case "KeyUp":
                executeKeyInput(operation, type, wait);
                break;
            case "Tilt":
                executeTiltInput(operation, wait);
                break;
            default:
                throw new Exception("Invalid operation was sent.");
        }
    }
    /// <summary>
    /// キーに関するOpeartionを捌く
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="type"></param>
    /// <param name="wait"></param>
    void executeKeyInput(ScriptObject operation, string type, int wait)
    {
        var propertyNames = operation.PropertyNames;
        if (!propertyNames.Contains("key"))
            throw new Exception("Object must contain the properties key.");

        // 辞書からメッセージを取ってくるやつ
        var search = (string type, string key) => messagedb[string.Format("{0}.{1}", type, key)];

        var message = "";
        dynamic _key = operation.GetProperty("key");
        if (_key is string)
        {
            // keyが単体の場合
            message = search(type, (string)_key);
            executeRaw(message, wait);
        }
        else
        {
            // keyが配列で与えられた場合、同時入力
            // wait最小で送る
            var keys = _key;
            for (var i = 0; i < keys.length; i++)
            {
                message = search(type, (string)keys[i]);
                if (i != keys.length - 1)
                {
                    executeRaw(message, MinimumWaitTime);
                }
                else
                {
                    executeRaw(message, wait);
                }
            }
        }
    }
    /// <summary>
    /// スティックに関するOpeartionを捌く
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="wait"></param>
    void executeTiltInput(ScriptObject operation, int wait)
    {
        // 何も含まれなければ一応弾く
        // パフォーマンスに問題があればチェックしないことも考える
        var propertyNames = operation.PropertyNames;
        if (!propertyNames.Contains("xAxis") && !propertyNames.Contains("yAxis") && !propertyNames.Contains("cxAxis") && !propertyNames.Contains("cyAxis"))
        {
            throw new Exception("Object must contain the properties xAxis or yAxis or cxAxis or cyAxis.");
        }

        // operationに含まれる*Axisをリストにためる
        var propList = new List<string>();
        foreach (var propName in new List<string> { "xAxis", "yAxis", "cxAxis", "cyAxis" })
        {
            if (propertyNames.Contains(propName)) propList.Add(propName);
        }

        // messageをwait最小で送信する
        var search = (string type, int key) => messagedb[string.Format("{0}.{1}", type, key)];
        var done = new List<string>();
        foreach (var propName in propList)
        {
            var message = search(propName, (int)operation.GetProperty(propName));
            done.Add(propName);
            if (done.Count != propList.Count)
            {
                executeRaw(message, MinimumWaitTime);
            }
            else
            {
                executeRaw(message, wait);
            }
        }
    }
    /// <summary>
    /// WHALEにメッセージを送信する。
    /// </summary>
    /// <param name="message">先頭1文字のみ有効</param>
    /// <param name="wait"></param>
    void executeRaw(string message, int wait)
    {
        if (!string.IsNullOrEmpty(message))
            _serialPort.WriteLine(message[0].ToString());

        _timer.sleep(wait);
    }
}
