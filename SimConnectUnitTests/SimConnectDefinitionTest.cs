using System;
using FluentAssertions;
using SimConnect.Attribute;
using SimConnect.Concrete;
using Xunit;

namespace SimConnectUnitTests
{
    public class SimConnectDefinitionTest
    {
        [Fact]
        public void EmptyObject()
        {
            Action test = () =>
            {
                var unused = new SimConnectDefinition(typeof(object));
            };

            test.Should().Throw<InvalidOperationException>("the object is empty");
        }


        // ReSharper disable UnusedMember.Global
        // ReSharper disable UnusedMember.Local
        #pragma warning disable 649
        private class NativeVariablesTest
        {
            [SimConnectVariable("INT")]
            public int IntField;

            [SimConnectVariable("UINT")]
            public uint UIntField;

            [SimConnectVariable("LONG")]
            public long LongField;

            [SimConnectVariable("ULONG")]
            public ulong ULongField;

            [SimConnectVariable("SINGLE")]
            public float SingleProperty { get; set; }

            [SimConnectVariable("DOUBLE")]
            public double DoubleProperty { get; set; }
        }
        #pragma warning restore 649
        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedMember.Global


        [Fact]
        public void NativeVariables()
        {
            var definition = new SimConnectDefinition(typeof(NativeVariablesTest));
            definition.Variables.Should().HaveCount(6);
        }
    }
}
