namespace SimConnect.Attribute
{
    /// <summary>
    /// Indicates the property should be registered as a SimConnect variable in the definition to receive updates.
    /// </summary>
    public class SimConnectVariableAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the SimConnect variable
        /// </summary>
        public string VariableName { get; }

        /// <summary>
        /// The SimConnect data type. If null, the SimConnect data type will be determined from the .NET type.
        /// </summary>
        public SimConnectDataType? DataType { get; }

        /// <summary>
        /// The requested units for the value. See the SimConnect documentation for available units
        /// </summary>
        public string UnitsName { get; }

        /// <summary>
        /// For integer and floating point types, determines how much the value must change before an update is sent
        /// </summary>
        public float Epsilon { get; }


        /// <summary>
        /// Indicates the property should be registered as a SimConnect variable in the definition to receive updates.
        /// </summary>
        /// <param name="variableName">The name of the SimConnect variable</param>
        /// <param name="dataType">The SimConnect data type. If null, the SimConnect data type will be determined from the .NET type.</param>
        /// <param name="unitsName">The requested units for the value. See the SimConnect documentation for available units</param>
        /// <param name="epsilon">For integer and floating point types, determines how much the value must change before an update is sent</param>
        public SimConnectVariableAttribute(string variableName, SimConnectDataType dataType, string unitsName = "", float epsilon = 0)
        {
            VariableName = variableName;
            DataType = dataType;
            UnitsName = unitsName;
            Epsilon = epsilon;
        }


        /// <summary>
        /// Indicates the property should be registered as a SimConnect variable in the definition to receive updates.
        /// </summary>
        /// <param name="variableName">The name of the SimConnect variable</param>
        /// <param name="unitsName">The requested units for the value. See the SimConnect documentation for available units</param>
        /// <param name="epsilon">For integer and floating point types, determines how much the value must change before an update is sent</param>
        public SimConnectVariableAttribute(string variableName, string unitsName = "", float epsilon = 0)
        {
            VariableName = variableName;
            DataType = null;
            UnitsName = unitsName;
            Epsilon = epsilon;
        }

    }
}
