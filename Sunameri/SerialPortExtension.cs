using System.IO.Ports;

using Microsoft.ClearScript;

public static class SerialPortExtension
{
    /// <summary>
    /// WHALEにメッセージを送信する。
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="scriptObject">プロパティmessageとwaitを含むオブジェクト | その配列</param>
    public static void write(this SerialPort serialPort, ScriptObject scriptObject)
    {
        // https://github.com/microsoft/ClearScript/issues/131

        dynamic _scriptObject = scriptObject;
        if (_scriptObject.constructor.name == "Array")
        {
            var sequence = _scriptObject;

            for (var i = 0; i < sequence.length; i++)
                serialPort.write((ScriptObject)sequence[i]);
        }
        else
        {
            var operation = scriptObject;
            var propertyNames = operation.PropertyNames;
            if (!propertyNames.Contains("message") || !propertyNames.Contains("wait"))
                throw new Exception("Object must contain the properties message and wait.");

            serialPort.write((string)operation.GetProperty("message"), (int)operation.GetProperty("wait"));
        }
    }
    /// <summary>
    /// WHALEにメッセージを送信する。
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="message">先頭1文字のみ有効</param>
    /// <param name="wait"></param>
    public static void write(this SerialPort serialPort, string message, int wait)
    {
        if (!string.IsNullOrEmpty(message))
            serialPort.WriteLine(message[0].ToString());
        
        if (wait != 0)
            serialPort.wait(wait);
    }
    /// <summary>
    /// 指定されたミリ秒間待機する。
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="millisecondsTimeout"></param>
    public static void wait(this SerialPort serialPort, int millisecondsTimeout)
    {
        Task.Run(() =>
        {
            long elapsed = 0;

            var interval = 10000000 / 1000;
            var next = DateTime.Now.Ticks + interval;

            while (elapsed < millisecondsTimeout)
                if (next <= DateTime.Now.Ticks)
                {
                    elapsed++;
                    next += interval;
                }
        }).Wait();
    }
}
