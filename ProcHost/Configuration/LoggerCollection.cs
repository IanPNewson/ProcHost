namespace ProcHost.Model;

public class LoggerCollection : ILogger
{

    private List<ILogger> _loggers = new List<ILogger>();

    public void Add(ILogger logger) => _loggers.Add(logger);
    
    public void LogOutput(string message, ChildProcess process)
    {
        foreach (var logger in _loggers)
            logger.LogOutput(message, process);
    }
}
