using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DisposeAndFinalize
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ConsoleMonitor instance....");
            ConsoleMonitor monitor = new ConsoleMonitor();
            monitor.Write();
            monitor.Dispose();
        }
    }
}

public class ConsoleMonitor : IDisposable
{
    const int STD_INPUT_HANDLE = -10;
    const int STD_OUTPUT_HANDLE = -11;
    const int STD_ERROR_HANDLE = -12;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteConsole(IntPtr hConsoleOutput, string lpBuffer,
           uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten,
           IntPtr lpReserved);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr handle);

    private bool disposed = false;
    private IntPtr handle;
    private Component component;

    public ConsoleMonitor()
    {
        handle = GetStdHandle(STD_OUTPUT_HANDLE);
        if (handle == IntPtr.Zero)
            throw new InvalidOperationException("A console handle is not available.");

        component = new Component();

        string output = "The ConsoleMonitor class constructor.\n";
        uint written = 0;
        WriteConsole(handle, output, (uint)output.Length, out written, IntPtr.Zero);
    }

    // The destructor calls Object.Finalize.
    ~ConsoleMonitor()
    {
        if (handle != IntPtr.Zero)
        {
            string output = "The ConsoleMonitor finalizer.\n";
            uint written = 0;
            WriteConsole(handle, output, (uint)output.Length, out written, IntPtr.Zero);
        }
        else
        {
            Console.Error.WriteLine("Object finalization.");
        }
        // Call Dispose with disposing = false.
        Dispose(false);
    }

    public void Write()
    {
        string output = "The Write method.\n";
        uint written = 0;
        WriteConsole(handle, output, (uint)output.Length, out written, IntPtr.Zero);
    }

    public void Dispose()
    {
        string output = "The Dispose method.\n";
        uint written = 0;
        WriteConsole(handle, output, (uint)output.Length, out written, IntPtr.Zero);

        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        string output = String.Format("The Dispose({0}) method.\n", disposing);
        uint written = 0;
        WriteConsole(handle, output, (uint)output.Length, out written, IntPtr.Zero);

        // Execute if resources have not already been disposed.
        if (!disposed)
        {
            // If the call is from Dispose, free managed resources.
            if (disposing)
            {
                Console.Error.WriteLine("Disposing of managed resources.");
                if (component != null)
                    component.Dispose();
            }
            // Free unmanaged resources.
            output = "Disposing of unmanaged resources.";
            WriteConsole(handle, output, (uint)output.Length, out written, IntPtr.Zero);

            if (handle != IntPtr.Zero)
            {
                if (!CloseHandle(handle))
                    Console.Error.WriteLine("Handle cannot be closed.");
            }
        }
        disposed = true;
    }

//    If you are dealing with unmanaged resources - 
    //Implement both Dispose and Finalize. Dispose is to be called by developers to free up the resources as soon as they see it that its no longer needed for them. If they forget to call the Dispose then Framework calls the finalize in its own GC cycle (usually will take its own sweet time).
//    If you are NOT dealing with unmanaged resources- 
    //Then dont do anything. Dont implement Finalize nor Dispose.
//    If your object uses Disposable objects internally - 
    //You implement Dispose() if you created and retained a reference to any object of a type which implements Dispose() and which you haven't already disposed.
}
