namespace FantasyCreator;

public class Request
{
    public event Action<Request, string[]>? OnComplete;
    public event Action<Request, string>? OnCreateImage;
    private readonly Creator _creator;
    private DateTime? _startTime;

    public long ChatId { get; }
    public ImageCreatorData Data { get; }
    public float Time => _startTime.HasValue ? (float)(DateTime.Now - _startTime).Value.TotalSeconds : 0;

    public Request(ImageCreatorData data, long chatId)
    {
        Data = data;
        ChatId = chatId;
        _creator = new Creator();
    }

    public async void Start()
    {
        _startTime = DateTime.Now;
        string[] paths = await _creator.CreateImages(Data, path => OnCreateImage?.Invoke(this, path));
        OnComplete?.Invoke(this, paths);
    }

    public void Stop()
    {
        _creator.Stop();
    }
}