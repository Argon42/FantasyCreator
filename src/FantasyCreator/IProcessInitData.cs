namespace FantasyCreator;

public interface IProcessInitData
{
    string ProcessFilePath { get; }
    string WorkingFolder { get; }
    string FileName { get; }
    string GetStartArgument();
}