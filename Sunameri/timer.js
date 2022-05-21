import * as Sunameri from "./modules/Sunameri.core.js";

const timer = Sunameri.getTimer();

System.Console.WriteLine("start");
timer.sleep(5000);
System.Console.WriteLine("stop");

System.Console.WriteLine("start");
    var task = timer.start();
    System.Console.WriteLine("submit");
    timer.submit(5000);

    task.Wait();
    System.Console.WriteLine("end");