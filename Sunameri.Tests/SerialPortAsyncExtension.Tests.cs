using Xunit;

using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Sunameri;

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
            // char & uint
            await controller.RunAsync(KeyDown.A, 100);
            await controller.RunAsync(KeyUp.A, 100);
            // Operation
            await controller.RunAsync(new Operation(KeyDown.A));
            await controller.RunAsync(new Operation(KeyUp.A));
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
            // char + uint
            await controller.RunAsync(KeyDown.A, 100, ct);
            await controller.RunAsync(KeyUp.A, 100, ct);
            // Operation
            await controller.RunAsync(new Operation(KeyDown.A), ct);
            await controller.RunAsync(new Operation(KeyUp.A), ct);
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
        }
    }
    [Fact(DisplayName = "RunAsyncは正しく機能している")]
    public async void RunAsync_works_properly()
    {
        using (var controller = new SerialPort("COM6", 4800))
        {
            controller.Open();
            
            // await
            await controller.RunAsync(new List<(char, uint)>
            {
                (KeyDown.Left, 1000),
                (KeyUp.Left, 100),
                (KeyDown.Right, 1000),
                (KeyUp.Right, 100),
            });
            Thread.Sleep(2000);

            // cancel
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            var task = controller.RunAsync(new List<(char, uint)>
            {
                (KeyDown.Up, 1000),
                (KeyUp.Up, 100),
                (KeyDown.Down, 1000),
                (KeyUp.Down, 1000), // ここでキャンセル
                (KeyDown.Up, 1000),
                (KeyUp.Up, 100),
                (KeyDown.Down, 1000),
                (KeyUp.Down, 100),
            }, ct);
            Thread.Sleep(2500);
            cts.Cancel();
            task.Wait();
        } 
    }
}