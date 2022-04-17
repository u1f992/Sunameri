using System.Diagnostics;
using System.IO.Ports;

namespace Sunameri;

/// <summary>
/// Provides methods for asynchronously running GAMECUBE operations.
/// </summary>
public static class SerialPortAsyncExtension
{
    /// <summary>
    /// Asynchronously run the operation.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="message"></param>
    public static async Task RunAsync(this SerialPort serialPort, char message) { await serialPort.RunAsync(message, Operation.DefaultInterval, CancellationToken.None); }
    /// <summary>
    /// Asynchronously run the operation.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAsync(this SerialPort serialPort, char message, CancellationToken cancellationToken) { await serialPort.RunAsync(message, Operation.DefaultInterval, cancellationToken); }
    /// <summary>
    /// Asynchronously run the operation.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="message"></param>
    /// <param name="interval"></param>
    public static async Task RunAsync(this SerialPort serialPort, char message, uint interval) { await serialPort.RunAsync(message, interval, CancellationToken.None); }
    /// <summary>
    /// Asynchronously run the operation.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="message"></param>
    /// <param name="interval"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAsync(this SerialPort serialPort, char message, uint interval, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (message != Special.Empty)
        {
            // "GCが起動していないか、接続されていません。" 以外のメッセージ受信を待機する。
            var sent = false;
            SerialDataReceivedEventHandler watch = (object sender, SerialDataReceivedEventArgs e) =>
            {
                if (!((SerialPort)sender).ReadExisting().Contains("GC")) sent = true;
            };

            var tmp = serialPort.DtrEnable;
            serialPort.DtrEnable = true;
            serialPort.DataReceived += watch;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            serialPort.WriteLine(message.ToString());

            // 100ms応答がなければ失敗と判断する。標準エラー出力に警告を書く
            await Task.Run(() =>
            {
                while (!sent && stopWatch.ElapsedMilliseconds < 100 && !cancellationToken.IsCancellationRequested) ;
                stopWatch.Stop();
                if (stopWatch.ElapsedMilliseconds >= 100) Console.Error.WriteLine("[Sunameri] [Warning] A message '{0}' had been ignored.", message);
            }, cancellationToken);

            // 購読解除と設定の復元
            serialPort.DataReceived -= watch;
            serialPort.DtrEnable = tmp;
        }

        var timeout = interval;
        if (interval != 0)
            await Task.Run(() =>
            {
                long elapsed = 0;

                var interval = 10000000 / 1000;
                var next = DateTime.Now.Ticks + interval;

                while (elapsed < timeout && !cancellationToken.IsCancellationRequested)
                {
                    if (next <= DateTime.Now.Ticks)
                    {
                        elapsed++;
                        next += interval;
                    }
                }
            }, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();


    }
    /// <summary>
    /// Asynchronously run the operation.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="operation"></param>
    public static async Task RunAsync(this SerialPort serialPort, Operation operation) { await serialPort.RunAsync(operation, CancellationToken.None); }
    /// <summary>
    /// Asynchronously run the operation.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="operation"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAsync(this SerialPort serialPort, Operation operation, CancellationToken cancellationToken) { await serialPort.RunAsync(operation.Message, operation.Interval, cancellationToken); }
    /// <summary>
    /// Asynchronously run the sequence.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    public static async Task RunAsync(this SerialPort serialPort, char[] sequence) { await serialPort.RunAsync(sequence, CancellationToken.None); }
    /// <summary>
    /// Asynchronously run the sequence.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAsync(this SerialPort serialPort, char[] sequence, CancellationToken cancellationToken) { foreach (var operation in sequence) { await serialPort.RunAsync(operation, Operation.DefaultInterval, cancellationToken); } }
    /// <summary>
    /// Asynchronously run the sequence.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    public static async Task RunAsync(this SerialPort serialPort, List<char> sequence) { await serialPort.RunAsync(sequence, CancellationToken.None); }
    /// <summary>
    /// Asynchronously run the sequence.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAsync(this SerialPort serialPort, List<char> sequence, CancellationToken cancellationToken) { await serialPort.RunAsync(sequence.ToArray(), cancellationToken); }
    /// <summary>
    /// Asynchronously run the sequence.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    public static async Task RunAsync(this SerialPort serialPort, Operation[] sequence) { await serialPort.RunAsync(sequence, CancellationToken.None); }
    /// <summary>
    /// Asynchronously run the sequence.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAsync(this SerialPort serialPort, Operation[] sequence, CancellationToken cancellationToken) { foreach (var operation in sequence) { await serialPort.RunAsync(operation, cancellationToken); } }
    /// <summary>
    /// Asynchronously run the sequence.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    public static async Task RunAsync(this SerialPort serialPort, List<Operation> sequence) { await serialPort.RunAsync(sequence, CancellationToken.None); }
    /// <summary>
    /// Asynchronously run the sequence.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAsync(this SerialPort serialPort, List<Operation> sequence, CancellationToken cancellationToken) { await serialPort.RunAsync(sequence.ToArray(), cancellationToken); }
    /// <summary>
    /// Asynchronously run the sequence.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    public static async Task RunAsync(this SerialPort serialPort, List<(char message, uint interval)> sequence) { await serialPort.RunAsync(sequence, CancellationToken.None); }
    /// <summary>
    /// Asynchronously run the sequence.
    /// </summary>
    /// <param name="serialPort"></param>
    /// <param name="sequence"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAsync(this SerialPort serialPort, List<(char message, uint interval)> sequence, CancellationToken cancellationToken) { foreach (var operation in sequence) { await serialPort.RunAsync(operation.message, operation.interval, cancellationToken); } }
}
