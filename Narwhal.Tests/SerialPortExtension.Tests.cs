using Xunit;

using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Narwhal;

public class SerialPortExtension_Tests
{
    [Fact(DisplayName = "READMEに載せたもの")]
    public void Sample()
    {
        using (var controller = new SerialPort("COM6", 4800))
        {
            controller.Open();
            
            controller.Run(KeyDown.A);

            controller.Run(KeyUp.A, 200);

            controller.Run(new Operation[]
            {
                new Operation(KeyDown.Right),
                new Operation(KeyUp.Right),
                new Operation(KeyDown.A, 500),
                new Operation(KeyUp.A),
                new Operation(KeyDown.B),
                new Operation(KeyUp.B)
            });

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
        }
    }
    [Fact(DisplayName = "Initializeは正しく機能している")]
    public void Initialize_works_properly()
    {
        using (var controller = new SerialPort("COM6", 4800))
        {
            controller.Open();
            // hold right
            controller.Run(KeyDown.Right, 0);
            
            Thread.Sleep(2500);
            controller.Initialize();

            // tilt left
            controller.Run(xAxis._0, 0);
            
            Thread.Sleep(1500);
            controller.Initialize();
        }
    }
}