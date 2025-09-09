namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
// in  должен быть инициализирован и не должен меняться в теле функции
public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler
{
}
