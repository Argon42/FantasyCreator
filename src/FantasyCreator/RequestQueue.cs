namespace FantasyCreator;

public class RequestQueue
{
    private const string StartRequestResponse = "Ваш запрос выполняется, примерное время ожидания {0:g}";
    private const string IncorrectRequestResponse = "Некорректный запрос";
    private const string RequestAddToQueue = "Запрос в очереди №{0}, примерное время ожидания {1:g}";

    public event Action<long, string[]>? OnCreateImages;

    private readonly Queue<Request> _queue = new(20);
    private Request? _currentRequest;

    public (bool, string) CreateNewRequest(Dictionary<string, string> input, long chatId)
    {
        if (Creator.TryCreatorData(input, out ImageCreatorData data) == false) return (false, IncorrectRequestResponse);

        Request request = new(data, chatId);
        _queue.Enqueue(request);
        if (_queue.Count == 1 && _currentRequest == null)
        {
            StartRequest(_queue.Dequeue());
            float requestTime = data.CalculateTime();
            return (true, string.Format(StartRequestResponse, TimeSpan.FromSeconds(requestTime)));
        }

        float time = _queue.Sum(request1 => request1.Data.CalculateTime()) - _currentRequest?.Time ?? 0;
        return (true, string.Format(RequestAddToQueue, _queue.Count, TimeSpan.FromSeconds(time)));
    }

    private void OnComplete(Request sender, string[] paths)
    {
        OnCreateImages?.Invoke(sender.ChatId, paths);

        _currentRequest = null;
        sender.OnComplete -= OnComplete;
        if (_queue.Count > 0) StartRequest(_queue.Dequeue());
    }

    private void StartRequest(Request request)
    {
        _currentRequest = request;
        _currentRequest.OnComplete += OnComplete;
        _currentRequest.Start();
    }
}