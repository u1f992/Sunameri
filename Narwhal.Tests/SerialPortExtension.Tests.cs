using Xunit;

using System.Collections.Generic;
using System.IO.Ports;
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

            controller.Run(new List<(char, uint)>
            {
                (KeyDown.Right, Operation.DefaultInterval),
                (KeyUp.Right,   Operation.DefaultInterval),
                (KeyDown.A,     500),
                (KeyUp.A,       Operation.DefaultInterval),
                (KeyDown.B,     Operation.DefaultInterval),
                (KeyUp.B,       Operation.DefaultInterval)
            });
        }
    }
}