using Xunit;

using System.Threading;
using System.Collections.Generic;
using System.IO.Ports;
using Narwhal;

public class SerialPortAsyncExtension_Tests
{
    [Fact(DisplayName = "RunAsyncのオーバーロードを正しく実装している")]
    public async void Various_overloads_of_RunAsync()
    {
        using (var controller = new SerialPort("COM6", 4800))
        {
            controller.Open();
            // char
            await controller.RunAsync(KeyDown.A);
            await controller.RunAsync(KeyUp.A);
            // char[]
            await controller.RunAsync(new char[]
            {
                KeyDown.A,
                KeyUp.A
            });
            // List<char>
            await controller.RunAsync(new List<char>
            {
                KeyDown.A,
                KeyUp.A
            });
            // Operation
            await controller.RunAsync(new Operation(KeyDown.A));
            await controller.RunAsync(new Operation(KeyUp.A));
            // Operation[]
            await controller.RunAsync(new Operation[]
            {
                new Operation(KeyDown.A),
                new Operation(KeyUp.A)
            });
            // List<Operation>
            await controller.RunAsync(new List<Operation>
            {
                new Operation(KeyDown.A),
                new Operation(KeyUp.A)
            });
            // List<(char, uint)>
            await controller.RunAsync(new List<(char, uint)>
            {
                (KeyDown.A, 100),
                (KeyUp.A, 100)
            });
            // char + uint
            await controller.RunAsync(KeyDown.A, 100);
            await controller.RunAsync(KeyUp.A, 100);
        }
    }
    [Fact(DisplayName = "CancellationTokenありのRunAsyncのオーバーロードを正しく実装している")]
    public async void Various_overloads_of_RunAsync_with_CancellationToken()
    {
        var ct = new CancellationTokenSource().Token;

        using (var controller = new SerialPort("COM6", 4800))
        {
            controller.Open();
            // char
            await controller.RunAsync(KeyDown.A, ct);
            await controller.RunAsync(KeyUp.A, ct);
            // char[]
            await controller.RunAsync(new char[]
            {
                KeyDown.A,
                KeyUp.A
            }, ct);
            // List<char>
            await controller.RunAsync(new List<char>
            {
                KeyDown.A,
                KeyUp.A
            }, ct);
            // Operation
            await controller.RunAsync(new Operation(KeyDown.A), ct);
            await controller.RunAsync(new Operation(KeyUp.A), ct);
            // Operation[]
            await controller.RunAsync(new Operation[]
            {
                new Operation(KeyDown.A),
                new Operation(KeyUp.A)
            }, ct);
            // List<Operation>
            await controller.RunAsync(new List<Operation>
            {
                new Operation(KeyDown.A),
                new Operation(KeyUp.A)
            }, ct);
            // List<(char, uint)>
            await controller.RunAsync(new List<(char, uint)>
            {
                (KeyDown.A, 100),
                (KeyUp.A, 100)
            }, ct);
            // char + uint
            await controller.RunAsync(KeyDown.A, 100, ct);
            await controller.RunAsync(KeyUp.A, 100, ct);
        }
    }
}