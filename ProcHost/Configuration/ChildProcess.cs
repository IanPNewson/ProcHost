namespace ProcHost.Model
{
    public class ChildProcess
    {

        public string Name { get; set; }
        public ProcessConfig Process { get; set; }

        public OutputSettings OutputSettings { get; set; }

        public int DelayStart { get; set; }
    }
}
