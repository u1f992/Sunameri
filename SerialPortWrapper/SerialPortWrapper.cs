using System.IO.Ports;
using System.Text;
using Microsoft.ClearScript;
using NLog;

public class SerialPortWrapper : IDisposable
{
    static Logger _logger = LogManager.GetCurrentClassLogger();

    SerialPort _serialPort;
    Timer _timer = new Timer();
    string _buffer = "";
    object lockObject = new Object();

    public SerialPortWrapper(ScriptObject config) : this((string)config.GetProperty("portName"), (int)config.GetProperty("baudRate")) { }
    public SerialPortWrapper(string portName, int baudRate)
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

    /// <summary>
    /// WHALEにメッセージを送信する。
    /// </summary>
    /// <param name="scriptObject">プロパティmessageとwaitを含むオブジェクト | その配列</param>
    public void execute(ScriptObject scriptObject)
    {
        // https://github.com/microsoft/ClearScript/issues/131

        dynamic _scriptObject = scriptObject;
        if (_scriptObject.constructor.name == "Array")
        {
            var sequence = _scriptObject;

            for (var i = 0; i < sequence.length; i++)
                execute((ScriptObject)sequence[i]);
        }
        else
        {
            var operation = scriptObject;
            var propertyNames = operation.PropertyNames;
            if (!propertyNames.Contains("message") || !propertyNames.Contains("wait"))
                throw new Exception("Object must contain the properties message and wait.");

            var message = (string)operation.GetProperty("message");
            var wait = (int)operation.GetProperty("wait");
            execute(message, wait);
        }
    }
    /// <summary>
    /// WHALEにメッセージを送信する。
    /// </summary>
    /// <param name="message">先頭1文字のみ有効</param>
    /// <param name="wait"></param>
    void execute(string message, int wait)
    {
        if (!string.IsNullOrEmpty(message))
            _serialPort.WriteLine(message[0].ToString());

        if (wait != 0)
            _timer.sleep(wait);
    }

    #region IDisposable
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _serialPort.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Hoge()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
