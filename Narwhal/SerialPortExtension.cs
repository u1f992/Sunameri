using System.IO.Ports;

namespace Narwhal;

/// <summary>
/// Provides NINTENDO GAMECUBE controller emulation via Arduino.
/// <see href="https://github.com/mizuyoukanao/WHALE"/>
/// </summary>
public static class SerialPortExtension
{
    /// <summary>
    /// Run the operation.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="message"></param>
    public static void Run(this SerialPort serialPort, char message) { serialPort.Run(message, Operation.DefaultInterval); }
    /// <summary>
    /// Run the sequence.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="sequence"></param>
    public static void Run(this SerialPort serialPort, char[] sequence) { foreach (var operation in sequence) serialPort.Run(operation, Operation.DefaultInterval); }
    /// <summary>
    /// Run the sequence.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="sequence"></param>
    public static void Run(this SerialPort serialPort, List<char> sequence) { serialPort.Run(sequence.ToArray()); }
    /// <summary>
    /// Run the operation.
    /// </summary>
    /// <param name="operation"></param>
    public static void Run(this SerialPort serialPort, Operation operation) { serialPort.Run(operation.Message, operation.Interval); }
    /// <summary>
    /// Run the sequence.
    /// </summary>
    /// <param name="sequence"></param>
    public static void Run(this SerialPort serialPort, Operation[] sequence) { foreach (var operation in sequence) serialPort.Run(operation); }
    /// <summary>
    /// Run the sequence.
    /// </summary>
    /// <param name="sequence"></param>
    public static void Run(this SerialPort serialPort, List<Operation> sequence) { serialPort.Run(sequence.ToArray()); }
    /// <summary>
    /// Run the sequence.
    /// </summary>
    /// <param name="sequence"></param>
    public static void Run(this SerialPort serialPort, List<(char message, uint interval)> sequence) { foreach (var operation in sequence) serialPort.Run(operation.message, operation.interval); }
    /// <summary>
    /// Run the operation.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="interval"></param>
    public static void Run(this SerialPort serialPort, char message, uint interval)
    {
        if (message != Special.Empty) serialPort.WriteLine(message.ToString());

        var timeout = interval;
        if (interval != 0)
            Task.Run(() => {
                long elapsed = 0;

                var interval = 10000000 / 1000;
                var next = DateTime.Now.Ticks + interval;

                while (elapsed < timeout)
                {
                    if (next <= DateTime.Now.Ticks)
                    {
                        elapsed++;
                        next += interval;
                    }
                }
            }).Wait();
    }
}
