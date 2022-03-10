namespace ProcHost.Model
{
    public interface ILogger
    {
        void LogOutput(string message, ChildProcess process);
    }
}
