# Sunameri

NINTENDO GAMECUBE automation library for .NET, compatible with [WHALE](https://github.com/mizuyoukanao/WHALE) firmware by mizuyoukanao.

## Usage

```cs
using System.IO.Ports;
using System.Text.Json;
using Sunameri;

using (var controller = new SerialPort("COM6", 4800))
{
    controller.Open();

    // Release all buttons and sticks
    controller.Initialize();
    
    // Press A
    // (interval: 100ms by default)
    controller.Run(KeyDown.A);

    // Release A and wait 200ms
    controller.Run(KeyUp.A, 200);

    // dR -> hold A 500ms -> B
    controller.Run(new Operation[]
    {
        new Operation(KeyDown.Right),
        new Operation(KeyUp.Right),
        new Operation(KeyDown.A, 500),
        new Operation(KeyUp.A),
        new Operation(KeyDown.B),
        new Operation(KeyUp.B)
    });

    // same as above but async
    var cts = new CancellationTokenSource();
    var ct = cts.Token;
    var task = controller.RunAsync(new List<(char, uint)>
    {
        (KeyDown.Right, Operation.DefaultInterval),
        (KeyUp.Right,   Operation.DefaultInterval),
        (KeyDown.A,     500),
        (KeyUp.A,       Operation.DefaultInterval),
        (KeyDown.B,     Operation.DefaultInterval),
        (KeyUp.B,       Operation.DefaultInterval)
    }, ct);
    task.Wait();

    // Deserialize from JSON
    var json = @"[{""command"":""a"",""interval"":100},{""command"":""m"",""interval"":100}]";
    var sequences = JsonSerializer.Deserialize<Operation[]>(json);
    if (sequences != null) controller.Run(sequences);
}
```

## Diagrams

```mermaid
classDiagram
class SerialPortExtension {
    +Initialize(SerialPort serialPort)$ void
    +Run(SerialPort serialPort, char message)$ void
    +Run(SerialPort serialPort, char message, uint interval)$ void
    +Run(SerialPort serialPort, Operation operation)$ void
    +Run(SerialPort serialPort, char[] sequence)$ void
    +Run(SerialPort serialPort, List~char~ sequence)$ void
    +Run(SerialPort serialPort, Operation[] sequence)$ void
    +Run(SerialPort serialPort, List~Operation~ sequence)$ void
    +Run(SerialPort serialPort, List~(char, uint)~ sequence)$ void
}
```

```mermaid
classDiagram
class SerialPortAsyncExtension {
    +RunAsync(SerialPort serialPort, char message)$ Task
    +RunAsync(SerialPort serialPort, char message, CancellationToken cancellationToken)$ Task
    +RunAsync(SerialPort serialPort, char message, uint interval)$ Task
    +RunAsync(SerialPort serialPort, char message, uint interval, CancellationToken cancellationToken)$ Task
    +RunAsync(SerialPort serialPort, Operation operation)$ Task
    +RunAsync(SerialPort serialPort, Operation operation, CancellationToken cancellationToken)$ Task
    +RunAsync(SerialPort serialPort, char[] sequence)$ Task
    +RunAsync(SerialPort serialPort, char[] sequence, CancellationToken cancellationToken)$ Task
    +RunAsync(SerialPort serialPort, List~char~ sequence)$ Task
    +RunAsync(SerialPort serialPort, List~char~ sequence, CancellationToken cancellationToken)$ Task
    +RunAsync(SerialPort serialPort, Operation[] sequence)$ Task
    +RunAsync(SerialPort serialPort, Operation[] sequence, CancellationToken cancellationToken)$ Task
    +RunAsync(SerialPort serialPort, List~Operation~ sequence)$ Task
    +RunAsync(SerialPort serialPort, List~Operation~ sequence, CancellationToken cancellationToken)$ Task
    +RunAsync(SerialPort serialPort, List~(char, uint)~ sequence)$ Task
    +RunAsync(SerialPort serialPort, List~(char, uint)~ sequence, CancellationToken cancellationToken)$ Task
}
```

```mermaid
classDiagram
class Operation {
    +uint DefaultInterval$
    +char Message
    +uint Interval

    +Operation()
    +Operation(char message)
    +Operation((char, uint) operation)
    +Operation(char message, uint interval)
    +ToJson() string
}
```

```mermaid
classDiagram
class KeyDown {
    +char A$
    +char B$
    +char X$
    +char Y$
    +char L$
    +char R$
    +char Z$
    +char Start$
    +char Left$
    +char Right$
    +char Down$
    +char Up$
}
class KeyUp {
    +char A$
    +char B$
    +char X$
    +char Y$
    +char L$
    +char R$
    +char Z$
    +char Start$
    +char Left$
    +char Right$
    +char Down$
    +char Up$
}
class xAxis {
    +char _0$
    +char _128$
    +char _255$
}
class yAxis {
    +char _0$
    +char _128$
    +char _255$
}
class cxAxis {
    +char _0$
    +char _128$
    +char _255$
}
class yxAxis {
    +char _0$
    +char _128$
    +char _255$
}
class Special {
    +char Reset$
    +char Empty$
}
```
