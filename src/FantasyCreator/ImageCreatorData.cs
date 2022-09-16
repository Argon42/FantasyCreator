using System.Globalization;

namespace FantasyCreator;

public class ImageCreatorData : IProcessInitData
{
    public int Count { get; }
    public string FileName { get; }
    public string Folder { get; }
    public float? Strength { get; }
    public string? InitImageUrl { get; }
    public string? InitMaskUrl { get; }
    public string ProcessFilePath { get; }
    public string Prompt { get; }
    public float Scale { get; }
    public long StartSeed { get; }
    public string Orientation { get; }
    public int StartSteps { get; }
    public int Step { get; }
    public int StepsCount { get; }
    public string WorkingFolder { get; }

    public ImageCreatorData(string processFilePath, string workingWorkingFolder, string fileName, string prompt,
        int startSteps,
        int stepsCount,
        int step,
        int count,
        float scale,
        long startSeed,
        string orientation,
        string folder = ".",
        float? strength = null,
        string? initImageUrl = null,
        string? initMaskUrl = null
    )
    {
        ProcessFilePath = processFilePath;
        WorkingFolder = workingWorkingFolder;
        FileName = fileName;
        Prompt = prompt;
        StartSteps = startSteps;
        StepsCount = stepsCount;
        Step = step;
        Count = count;
        Scale = scale;
        StartSeed = startSeed;
        Orientation = orientation;
        Folder = folder;
        Strength = strength != null ? (float?)Math.Round(strength.GetValueOrDefault(), 3) : null;
        InitImageUrl = initImageUrl;
        InitMaskUrl = initMaskUrl;
    }

    public float CalculateTime()
    {
        const float iterationDuration = 4f;
        const float startingDelay = 20f;
        return Count * (StartSteps + Step * (1 + StepsCount) / 2) * StepsCount * iterationDuration + startingDelay;
    }

    public string GetStartArgument()
    {
        return "{0} " +
               $"--prompt=\"{Prompt}\" " +
               $"--start_steps={StartSteps} " +
               $"--steps_count={StepsCount} " +
               $"--step={Step} " +
               $"--count={Count} " +
               $"--scale={Scale.ToString(CultureInfo.InvariantCulture)} " +
               $"--start_seed={StartSeed} " +
               $"--orientation={Orientation} " +
               (Strength != null ? $"--strength={Strength?.ToString(CultureInfo.InvariantCulture)} " : "") +
               (InitImageUrl != null ? $"--init_image_url=\"{InitImageUrl}\" " : "") +
               (InitMaskUrl != null ? $"--init_mask_url=\"{InitMaskUrl}\" " : "") +
               $"--folder={Folder}";
    }
}