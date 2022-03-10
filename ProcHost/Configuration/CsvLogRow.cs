namespace ProcHost.Model;

internal record struct CsvLogRow(DateTime Time, string Machine, string TaskName, string Message, string Executable, string Arguments)
{
    public static implicit operator (DateTime Time, string Machine, string TaskName, string Message, string Executable, string Arguments)(CsvLogRow value)
    {
        return (value.Time, value.Machine, value.TaskName, value.Message, value.Executable, value.Arguments);
    }

    public static implicit operator CsvLogRow((DateTime Time, string Machine, string TaskName, string Message, string Executable, string Arguments) value)
    {
        return new CsvLogRow(value.Time, value.Machine, value.TaskName, value.Message, value.Executable, value.Arguments);
    }
}