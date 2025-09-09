namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

/// <summary>
/// только guid и дата создания 
/// </summary>
public record IntegrationEvent
{        
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    [JsonConstructor] // нужно посмотреть что какие аттрибуты еще есть
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }

    [JsonInclude]
    public Guid Id { get; private init; }

    [JsonInclude]
    public DateTime CreationDate { get; private init; }
}
