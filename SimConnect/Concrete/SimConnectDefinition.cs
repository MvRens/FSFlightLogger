using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SimConnect.Attribute;

namespace SimConnect.Concrete
{
    /// <summary>
    /// Defines a SimConnect variable.
    /// </summary>
    public struct SimConnectVariable
    {
        /// <summary>
        /// The name of the SimConnect variable
        /// </summary>
        public string VariableName;

        /// <summary>
        /// The SimConnect data type. If null, the SimConnect data type will be determined from the .NET type.
        /// </summary>
        public SimConnectDataType DataType;

        /// <summary>
        /// The requested units for the value. See the SimConnect documentation for available units
        /// </summary>
        public string UnitsName;

        /// <summary>
        /// For integer and floating point types, determines how much the value must change before an update is sent
        /// </summary>
        public float Epsilon;
    };


    /// <summary>
    /// Creates a SimConnect definition from properties annotated with the SimConnectVariable attribute.
    /// </summary>
    public class SimConnectDefinition
    {
        private readonly Type type;

        private delegate void SetterProc(object instance, BinaryReader data);

        private struct SimConnectVariableProperty
        {
            public SimConnectVariable Variable;
            public SetterProc Setter;
        }

        /// <summary>
        /// Provides access to the parsed variables.
        /// </summary>
        public IEnumerable<SimConnectVariable> Variables => variables.Select(v => v.Variable);

        private readonly List<SimConnectVariableProperty> variables;


        /// <summary>
        /// Creates an instance of a SimConnectDefinition based on the provided class.
        /// </summary>
        /// <param name="type">The class type used for the definition</param>
        public SimConnectDefinition(Type type)
        {
            if (!type.IsClass)
                throw new InvalidOperationException($"{type.FullName} is not a class type");

            this.type = type;
            variables = ParseVariables(type);

            if (variables.Count == 0)
                throw new InvalidOperationException($"At least one property of {type.FullName} should be annotated with the SimConnectVariable attribute");
        }


        /// <summary>
        /// Parses the SimConnect data stream to an object according to this definition.
        /// </summary>
        /// <param name="data">The SimConnect data stream</param>
        /// <returns>An instance of the type for this definition</returns>
        public object ParseData(Stream data)
        {
            var reader = new BinaryReader(data);
            var instance = Activator.CreateInstance(type);

            foreach (var variable in variables)
                variable.Setter(instance, reader);

            return instance;
        }


        private static List<SimConnectVariableProperty> ParseVariables(IReflect type)
        {
            var result = new List<SimConnectVariableProperty>();

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property))
            {
                var variableAttribute = member.GetCustomAttribute<SimConnectVariableAttribute>();
                if (variableAttribute == null)
                    continue;


                var dataType = variableAttribute.DataType;
                var setter = GetSetter(member, ref dataType);

                if (!dataType.HasValue)
                    throw new InvalidOperationException($"DataType could not be determined for member {member.Name}");

                result.Add(new SimConnectVariableProperty
                {
                    Variable = new SimConnectVariable
                    {
                        VariableName = variableAttribute.VariableName,
                        DataType = dataType.Value,
                        UnitsName = variableAttribute.UnitsName,
                        Epsilon = variableAttribute.Epsilon
                    },
                    Setter = setter
                });
            }

            return result;
        }


        private readonly struct SimConnectTypeMapping
        {
            public readonly Type Type;
            public readonly SimConnectDataType SimConnectDataType;
            public readonly bool IsDefaultForTypeForType;
            public readonly Func<BinaryReader, object> Converter;

            public SimConnectTypeMapping(Type type, SimConnectDataType simConnectDataType, bool isDefaultForType, Func<BinaryReader, object> converter)
            {
                Type = type;
                SimConnectDataType = simConnectDataType;
                IsDefaultForTypeForType = isDefaultForType;
                Converter = converter;
            }
        }


        private static readonly List<SimConnectTypeMapping> SimConnectTypeMappings = new List<SimConnectTypeMapping>
        {
            { new SimConnectTypeMapping(typeof(double), SimConnectDataType.Float64, true, reader => reader.ReadDouble()) },
            { new SimConnectTypeMapping(typeof(float), SimConnectDataType.Float32, true, reader => reader.ReadSingle()) },
            { new SimConnectTypeMapping(typeof(int), SimConnectDataType.Int32, true, reader => reader.ReadInt32()) },
            { new SimConnectTypeMapping(typeof(uint), SimConnectDataType.Int32, true, reader => reader.ReadUInt32()) },
            { new SimConnectTypeMapping(typeof(long), SimConnectDataType.Int64, true, reader => reader.ReadInt64()) },
            { new SimConnectTypeMapping(typeof(ulong), SimConnectDataType.Int64, true, reader => reader.ReadUInt64()) }
        };


        private static SetterProc GetSetter(MemberInfo member, ref SimConnectDataType? dataType)
        {
            Type valueType;
            Action<object, object> valueSetter;

            if (member.MemberType == MemberTypes.Field)
            {
                var fieldInfo = (FieldInfo)member;
                valueType = fieldInfo.FieldType;
                valueSetter = (instance, data) => fieldInfo.SetValue(instance, data);

            }
            else
            {
                var propertyInfo = (PropertyInfo)member;
                valueType = propertyInfo.PropertyType;
                valueSetter = (instance, data) => propertyInfo.SetValue(instance, data);
            }


            foreach (var mapping in SimConnectTypeMappings.Where(mapping => mapping.Type == valueType))
            {
                if (dataType.HasValue && mapping.SimConnectDataType != dataType)
                    continue;

                if (!dataType.HasValue && !mapping.IsDefaultForTypeForType)
                    continue;

                if (!dataType.HasValue)
                    dataType = mapping.SimConnectDataType;

                return (instance, reader) =>
                {
                    valueSetter(instance, mapping.Converter(reader));
                };
            }

            throw new InvalidOperationException($"No mapping exists for type {valueType.FullName} with SimConnect DataType {dataType} for member {member.Name}");
        }
    }
}
