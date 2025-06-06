namespace AdnTestingService.EventBus.Events;

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
    
    [JsonInclude]
    public Guid Id { get; private set; }
    
    [JsonInclude]
    public DateTime CreationDate { get; private set; }
}