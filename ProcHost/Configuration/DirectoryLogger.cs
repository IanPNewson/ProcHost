using CsvHelper;
using CsvHelper.Configuration;
using INHelpers.ExtensionMethods;
using System.Text;

namespace ProcHost.Model;

public class DirectoryLogger : ILogger
{
    private DirectoryInfo _directory;

    public DirectoryLogger(string directoryPath, string fileNameFormat, string fileExtension)
    {
        _directory = new DirectoryInfo(directoryPath);
        FileNamePattern = fileNameFormat;
        FileExtension = fileExtension;
    }

    public string FileNamePattern { get; }
    public string FileExtension { get; }

    private FileInfo GetFile()
    {
        var builder = new StringBuilder();
        builder.Append(DateTime.Now.ToString(FileNamePattern));
        if (!this.FileExtension.StartsWith("."))
            builder.Append(".");
        builder.Append(this.FileExtension);

        return _directory.EnsureExists()
            .File(builder.ToString());
    }

    public void LogOutput(string message, ChildProcess process)
    {
        var file = this.GetFile();
        var newFile = !file.Exists;

        using (var writer = new StreamWriter(file.FullName, true))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(cultureInfo: System.Globalization.CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = !newFile
        }))
        {
            if (newFile)
            {
                csv.WriteHeader<CsvLogRow>();
                csv.NextRecord();
            }

            var row = new CsvLogRow(
                DateTime.Now,
                Environment.MachineName,
                process.Name,
                message,
                process.Process.Executable,
                process.Process.Arguments
            );
            csv.WriteRecord(row);
            csv.NextRecord();
        }
    }
}
