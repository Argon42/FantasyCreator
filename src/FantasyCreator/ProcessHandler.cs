using System.Diagnostics;
using System.Text;

namespace FantasyCreator;

public class ProcessHandler : IDisposable
{
    private readonly Process _process;
    private bool _hasFirstStart;

    public bool Enabled => _hasFirstStart && _process.HasExited == false;

    public IProcessInitData InitData { get; }

    public ProcessHandler(IProcessInitData initData)
    {
        InitData = initData;
        _process = new Process();

        _process.StartInfo = new ProcessStartInfo(initData.ProcessFilePath)
        {
            Arguments = string.Format(initData.GetStartArgument(),
                Path.Combine(initData.WorkingFolder, initData.FileName)),
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            WorkingDirectory = initData.WorkingFolder,
            CreateNoWindow = false
        };
    }

    public void Dispose()
    {
        _process.Kill();
        _process.Dispose();
    }

    public void ForceStop()
    {
        if (Enabled == false) throw new InvalidOperationException("Process disabled");

        _process.Kill();
    }

    public async Task<string> Start(Action<string>? onImageCreated = null)
    {
        if (Enabled) throw new InvalidOperationException("Process enabled");

        _process.Start();
        _hasFirstStart = true;


        StreamReader reader = _process.StandardOutput;
        StringBuilder allOutput = new(255);
        while (Enabled)
        {
            string? output = await reader.ReadLineAsync();
            if (output == null) continue;
            onImageCreated?.Invoke(output);
            allOutput.AppendLine(output);
        }

        await _process.WaitForExitAsync();
        return allOutput.ToString();
    }
}