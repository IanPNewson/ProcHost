using System.Runtime.InteropServices;

namespace ProcHost.ChildTracker;

[StructLayout(LayoutKind.Sequential)]
public struct IO_COUNTERS
{
    public UInt64 ReadOperationCount;
    public UInt64 WriteOperationCount;
    public UInt64 OtherOperationCount;
    public UInt64 ReadTransferCount;
    public UInt64 WriteTransferCount;
    public UInt64 OtherTransferCount;
}
