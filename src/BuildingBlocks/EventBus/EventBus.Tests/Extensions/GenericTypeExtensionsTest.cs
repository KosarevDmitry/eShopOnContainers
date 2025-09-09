using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
using Xunit;

namespace EventBus.Tests.Extensions;

//[TestSubject(typeof(GenericTypeExtensions))]
public class GenericTypeExtensionsTest
{
    [Fact]
    public void GetGenericTypeNameTest()
    {
        var genericTester = new GenericTester<int, double, StringBuilder>();
       var name          = genericTester.GetGenericTypeName();
        Assert.Equal("GenericTester<Int32,Double,StringBuilder>",name); // без пробелов между аргументами, названия типов иные
    }
}

public class GenericTester<T1, T2, T3> where T1 : struct
                                        where T2 : struct
                                        where T3 : new()
{
//    public T3 METHOD(T1 a, T2 b)
//    {
//        return new T3();
//    }
}