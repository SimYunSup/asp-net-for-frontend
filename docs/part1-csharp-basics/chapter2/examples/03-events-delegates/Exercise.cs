// 이벤트와 델리게이트 연습 문제

// TODO 1: 파일 업로드 진행 상태 이벤트 시스템
public class FileUploadEventArgs : EventArgs
{
    public required string FileName { get; set; }
    public long BytesUploaded { get; set; }
    public long TotalBytes { get; set; }
    public int ProgressPercentage => (int)((BytesUploaded * 100) / TotalBytes);
}

public class FileUploader
{
    // TODO: 이벤트 선언
    // - UploadStarted
    // - ProgressChanged
    // - UploadCompleted
    // - UploadFailed

    public async Task UploadFileAsync(string filePath)
    {
        // TODO: 파일 업로드 시뮬레이션 + 이벤트 발생
        throw new NotImplementedException();
    }
}

// TODO 2: 승인 프로세스 이벤트 체인
public enum ApprovalStatus
{
    Pending,
    ManagerApproved,
    DirectorApproved,
    Approved,
    Rejected
}

public class ApprovalEventArgs : EventArgs
{
    public int RequestId { get; set; }
    public ApprovalStatus Status { get; set; }
    public required string ApprovedBy { get; set; }
    public required string Comments { get; set; }
}

public class ApprovalProcess
{
    // TODO: 승인 단계별 이벤트 선언

    public void SubmitRequest(int requestId)
    {
        // TODO: 승인 프로세스 시작
        throw new NotImplementedException();
    }
}

// TODO 3: 실시간 채팅 메시지 시스템
public class ChatMessage
{
    public required string Sender { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; }
}

public delegate void MessageReceivedDelegate(ChatMessage message);
public delegate bool MessageFilterDelegate(ChatMessage message);

public class ChatRoom
{
    // TODO: 델리게이트 기반 메시지 시스템 구현
    // - AddMessageHandler: 메시지 핸들러 추가
    // - AddMessageFilter: 메시지 필터 추가 (욕설 필터 등)
    // - SendMessage: 메시지 전송 (필터 통과 후 핸들러 호출)

    throw new NotImplementedException();
}

// TODO 4: 주식 가격 변동 알림
public class StockPriceEventArgs : EventArgs
{
    public required string Symbol { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public decimal ChangePercentage =>
        OldPrice > 0 ? ((NewPrice - OldPrice) / OldPrice) * 100 : 0;
}

public class StockMarket
{
    // TODO: 이벤트 선언
    // - PriceChanged: 가격 변동 시
    // - SignificantChange: 5% 이상 변동 시
    // - ThresholdReached: 특정 가격 도달 시

    public void UpdatePrice(string symbol, decimal newPrice)
    {
        // TODO: 가격 업데이트 + 이벤트 발생
        throw new NotImplementedException();
    }
}

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: 파일 업로드
public class FileUploader
{
    public event EventHandler<FileUploadEventArgs>? UploadStarted;
    public event EventHandler<FileUploadEventArgs>? ProgressChanged;
    public event EventHandler<FileUploadEventArgs>? UploadCompleted;
    public event EventHandler<string>? UploadFailed;

    public async Task UploadFileAsync(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var totalBytes = new FileInfo(filePath).Length;

        try
        {
            // 업로드 시작
            OnUploadStarted(new FileUploadEventArgs
            {
                FileName = fileName,
                BytesUploaded = 0,
                TotalBytes = totalBytes
            });

            // 진행 상태 시뮬레이션
            for (long uploaded = 0; uploaded <= totalBytes; uploaded += totalBytes / 10)
            {
                await Task.Delay(100); // 업로드 시뮬레이션

                OnProgressChanged(new FileUploadEventArgs
                {
                    FileName = fileName,
                    BytesUploaded = Math.Min(uploaded, totalBytes),
                    TotalBytes = totalBytes
                });
            }

            // 완료
            OnUploadCompleted(new FileUploadEventArgs
            {
                FileName = fileName,
                BytesUploaded = totalBytes,
                TotalBytes = totalBytes
            });
        }
        catch (Exception ex)
        {
            OnUploadFailed(ex.Message);
        }
    }

    protected virtual void OnUploadStarted(FileUploadEventArgs e) =>
        UploadStarted?.Invoke(this, e);

    protected virtual void OnProgressChanged(FileUploadEventArgs e) =>
        ProgressChanged?.Invoke(this, e);

    protected virtual void OnUploadCompleted(FileUploadEventArgs e) =>
        UploadCompleted?.Invoke(this, e);

    protected virtual void OnUploadFailed(string error) =>
        UploadFailed?.Invoke(this, error);
}

// TODO 2: 승인 프로세스
public class ApprovalProcess
{
    public event EventHandler<ApprovalEventArgs>? ApprovalRequested;
    public event EventHandler<ApprovalEventArgs>? ManagerApproval;
    public event EventHandler<ApprovalEventArgs>? DirectorApproval;
    public event EventHandler<ApprovalEventArgs>? FinalApproval;
    public event EventHandler<ApprovalEventArgs>? Rejected;

    public void SubmitRequest(int requestId)
    {
        OnApprovalRequested(new ApprovalEventArgs
        {
            RequestId = requestId,
            Status = ApprovalStatus.Pending,
            ApprovedBy = "System",
            Comments = "Request submitted"
        });

        // 시뮬레이션: 자동 승인 플로우
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            ApproveByManager(requestId, "Manager1", "Approved by manager");

            await Task.Delay(1000);
            ApproveByDirector(requestId, "Director1", "Approved by director");

            await Task.Delay(1000);
            FinalApprove(requestId, "CEO", "Final approval");
        });
    }

    private void ApproveByManager(int requestId, string approver, string comments)
    {
        OnManagerApproval(new ApprovalEventArgs
        {
            RequestId = requestId,
            Status = ApprovalStatus.ManagerApproved,
            ApprovedBy = approver,
            Comments = comments
        });
    }

    private void ApproveByDirector(int requestId, string approver, string comments)
    {
        OnDirectorApproval(new ApprovalEventArgs
        {
            RequestId = requestId,
            Status = ApprovalStatus.DirectorApproved,
            ApprovedBy = approver,
            Comments = comments
        });
    }

    private void FinalApprove(int requestId, string approver, string comments)
    {
        OnFinalApproval(new ApprovalEventArgs
        {
            RequestId = requestId,
            Status = ApprovalStatus.Approved,
            ApprovedBy = approver,
            Comments = comments
        });
    }

    protected virtual void OnApprovalRequested(ApprovalEventArgs e) =>
        ApprovalRequested?.Invoke(this, e);

    protected virtual void OnManagerApproval(ApprovalEventArgs e) =>
        ManagerApproval?.Invoke(this, e);

    protected virtual void OnDirectorApproval(ApprovalEventArgs e) =>
        DirectorApproval?.Invoke(this, e);

    protected virtual void OnFinalApproval(ApprovalEventArgs e) =>
        FinalApproval?.Invoke(this, e);
}

// TODO 3: 채팅 시스템
public class ChatRoom
{
    private MessageReceivedDelegate? _messageHandlers;
    private MessageFilterDelegate? _messageFilters;

    public void AddMessageHandler(MessageReceivedDelegate handler)
    {
        _messageHandlers += handler;
    }

    public void RemoveMessageHandler(MessageReceivedDelegate handler)
    {
        _messageHandlers -= handler;
    }

    public void AddMessageFilter(MessageFilterDelegate filter)
    {
        _messageFilters += filter;
    }

    public void SendMessage(string sender, string content)
    {
        var message = new ChatMessage
        {
            Sender = sender,
            Content = content,
            Timestamp = DateTime.Now
        };

        // 필터 체크
        if (_messageFilters != null)
        {
            foreach (MessageFilterDelegate filter in _messageFilters.GetInvocationList())
            {
                if (!filter(message))
                {
                    Console.WriteLine($"Message blocked by filter: {content}");
                    return;
                }
            }
        }

        // 핸들러 호출
        _messageHandlers?.Invoke(message);
    }
}

// TODO 4: 주식 시장
public class StockMarket
{
    private readonly Dictionary<string, decimal> _prices = new();

    public event EventHandler<StockPriceEventArgs>? PriceChanged;
    public event EventHandler<StockPriceEventArgs>? SignificantChange;
    public event EventHandler<StockPriceEventArgs>? ThresholdReached;

    public void UpdatePrice(string symbol, decimal newPrice)
    {
        var oldPrice = _prices.GetValueOrDefault(symbol, newPrice);
        _prices[symbol] = newPrice;

        var eventArgs = new StockPriceEventArgs
        {
            Symbol = symbol,
            OldPrice = oldPrice,
            NewPrice = newPrice
        };

        OnPriceChanged(eventArgs);

        if (Math.Abs(eventArgs.ChangePercentage) >= 5)
        {
            OnSignificantChange(eventArgs);
        }
    }

    protected virtual void OnPriceChanged(StockPriceEventArgs e) =>
        PriceChanged?.Invoke(this, e);

    protected virtual void OnSignificantChange(StockPriceEventArgs e) =>
        SignificantChange?.Invoke(this, e);

    protected virtual void OnThresholdReached(StockPriceEventArgs e) =>
        ThresholdReached?.Invoke(this, e);
}

*/
