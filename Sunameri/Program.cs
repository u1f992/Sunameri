using System.Reflection;

using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

using OpenCvSharp;

ConsoleApp.Run(args, ([Option(0, "scriptfile")] string input) =>
{
    var inputPath = GetRootedPath(input);

    var engines = new V8ScriptEngine[Environment.ProcessorCount];
    using (var engine = new V8ScriptEngine())
    {
        // ^C を入力するとスクリプトを停止する
        Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
        {
            Console.WriteLine("Execution cancelled.");
            engine.Interrupt();
            Environment.Exit(1);
        };

        // System 以下のものを何でも使える
        engine.AddHostObject("dotnet", HostItemFlags.GlobalMembers, new HostTypeCollection("mscorlib", "System.Core"));

        // seed = newVar(System.UInt32); など必須
        engine.AddHostObject("host", HostItemFlags.GlobalMembers, new HostFunctions());

        engine.AddHostTypes(new Type[]
        {
            typeof(SerialPortWrapper),
            typeof(VideoCaptureWrapper),
            typeof(Cv2),
            typeof(Mat),
            typeof(MatExtension)
        });

        // pluginsディレクトリ内の*.dllから追加する
        var plugins = Path.Join(AppContext.BaseDirectory, "plugins");
        if (Directory.Exists(plugins))
        {
            var files = Directory.GetFiles(plugins);
            var dlls = files.Where(path => Path.GetExtension(path) == "dll");
            foreach (string plugin in dlls)
                try
                {
                    var asm = Assembly.LoadFrom(plugin);
                    engine.AddHostTypes(asm.GetTypes());
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
        }

        // enable module
        engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
        
        // execute
        try
        {
            engine.Execute(new DocumentInfo { Category = ModuleCategory.Standard }, File.ReadAllText(inputPath));
        }
        catch (ScriptInterruptedException)
        {
            // 本当はここで止まってほしいが、たぶんsleepの実装が悪くてキャンセルできない。
            // Environment.Exit(1); で吹き飛ばしている。
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Environment.ExitCode = 1;
        }
    }
});

/// <summary>
/// 絶対パスに変換する
/// </summary>
/// <param name="path">パス文字列</param>
/// <returns></returns>
string GetRootedPath(string path)
{
    return Path.IsPathRooted(path) ? path : Path.Join(AppContext.BaseDirectory, path);
}
