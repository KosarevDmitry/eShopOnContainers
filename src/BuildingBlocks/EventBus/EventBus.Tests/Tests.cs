using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
using Xunit;

namespace EventBus.Tests;

public class Tests
{
    [Fact]
    public void Test1()
    {
        string conn="conn";
        DefaultServiceBusPersisterConnection p = new(conn);
        
    }
    
}