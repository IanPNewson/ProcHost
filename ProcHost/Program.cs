using Newtonsoft.Json;
using ProcHost.Model;
using System.Diagnostics;
static void Write(string str) => Console.WriteLine(str);

var configFile = new FileInfo(args.Single());
if (!configFile.Exists)
    throw new FileNotFoundException(configFile.FullName);

var configs = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configFile.FullName));

OutputSettings? currentSettings = null;

var announce = (ChildProcess config) =>
{
    var settings = config.OutputSettings;
    if (currentSettings == null ||
        currentSettings != settings)
    {
        lock (Console.Out)
        {
            Console.BackgroundColor = settings.BackColor;
            Console.ForegroundColor = settings.TextColor;

            Write("============");
            Write(config.Name ?? config.Process.Executable);
            Write("============");
        }

        currentSettings = settings;
    }
};

var tasks = configs.Children
    .Select(config => new Task(async () =>
    {
        var name = config.Name ?? config.Process.Executable;

        if (config.DelayStart > 0)
        {
            Write($"Delaying task {name} for {config.DelayStart} ms");
            Thread.Sleep(config.DelayStart);
        }

        var startArgs = new ProcessStartInfo(config.Process.Executable, config.Process.Arguments);
        startArgs.CreateNoWindow = true;
        startArgs.UseShellExecute = false;
        startArgs.RedirectStandardOutput = true;
        startArgs.RedirectStandardError = true;

        var process = Process.Start(startArgs)!;
        process.OutputDataReceived += (sender, args) =>
        {
            announce(config);
            lock (Console.Out)
            {
                Console.BackgroundColor = config.OutputSettings.BackColor;
                Console.ForegroundColor = config.OutputSettings.TextColor;

                Write(args.Data ?? "");
            }
        };
        process.BeginOutputReadLine();

        await process.WaitForExitAsync();
    }));

foreach (var task in tasks)
    task.Start();

Console.WriteLine("Type 'exit' and press enter to quit");
while (Console.ReadLine() != "exit") ;

Console.WriteLine("Stopping...");
