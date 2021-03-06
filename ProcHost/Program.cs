using Newtonsoft.Json;
using ProcHost.ChildTracker;
using ProcHost.Model;
using System.Diagnostics;
static void Write(string str) => Console.WriteLine(str);

var configFile = new FileInfo(args.Single());
if (!configFile.Exists)
    throw new FileNotFoundException(configFile.FullName);

var configs = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configFile.FullName));

var loggers = new LoggerCollection();

foreach(var loggerType in configs.Loggers)
    loggers.Add(loggerType.Instantiate<ILogger>());

OutputSettings? currentSettings = null;

var processes = new List<Process>();

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
        processes.Add(process);
        ChildProcessTracker.AddProcess(process);
        process.OutputDataReceived += (sender, args) =>
        {
            if (string.IsNullOrWhiteSpace(args.Data)) return;

            lock (Console.Out)
            {
                announce(config);
                Console.BackgroundColor = config.OutputSettings.BackColor;
                Console.ForegroundColor = config.OutputSettings.TextColor;

                Write(args.Data ?? "");

                loggers.LogOutput(args.Data, config);
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


