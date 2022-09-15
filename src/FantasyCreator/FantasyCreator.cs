namespace FantasyCreator;

internal class Creator
{
    private const string Folder = @"G:\PROGRAMS\StableDiffusion\diffusers\examples\inference\";
    private const string File = @"dml_onnx.py";
    private const string Python = @"G:\PROGRAMS\Python\python.exe";

    private const string Prompt = "prompt";

    private const string Steps = "steps";
    private const int DefaultValueStartSteps = 10;

    private const string StepsCount = "steps_count";
    private const int DefaultValueStepsCount = 1;

    private const string Step = "step";
    private const int DefaultValueStep = 1;

    private const string Count = "count";
    private const int DefaultValueCount = 1;

    private const string Scale = "scale";
    private const float DefaultValueScale = 7.5f;

    private const string Seed = "seed";
    private const int DefaultValueStartSeed = -1;

    private ProcessHandler? _creator;


    public async Task<string[]> CreateImages(ImageCreatorData data)
    {
        _creator = new ProcessHandler(data);
        string paths = await _creator.Start();
        _creator = null;

        return paths.Split();
    }

    public void Stop()
    {
        _creator?.ForceStop();
        _creator = null;
    }

    public static bool TryCreatorData(Dictionary<string, string> input, out ImageCreatorData data)
    {
        data = new ImageCreatorData(Python, Folder, File,
            GetValue(input, Prompt, "", s => s),
            folder: AppContext.BaseDirectory.TrimEnd('\\'),
            startSteps: GetValue(input, Steps, DefaultValueStartSteps, int.Parse),
            stepsCount: GetValue(input, StepsCount, DefaultValueStepsCount, int.Parse),
            step: GetValue(input, Step, DefaultValueStep, int.Parse),
            count: GetValue(input, Count, DefaultValueCount, int.Parse),
            scale: GetValue(input, Scale, DefaultValueScale, float.Parse),
            startSeed: GetValue(input, Seed, DefaultValueStartSeed, long.Parse)
        );

        return string.IsNullOrEmpty(data.Prompt) == false;
    }

    private static T GetValue<T>(
        IReadOnlyDictionary<string, string> dictionary,
        string name, T defaultValue, Func<string, T> convert)
    {
        if (dictionary.TryGetValue(name, out string? value) == false || string.IsNullOrEmpty(value))
            return defaultValue;

        try
        {
            T tValue = convert(value);
            return tValue;
        }
        catch (Exception e)
        {
            return defaultValue;
        }
    }
}