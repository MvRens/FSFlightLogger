using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SimConnect
{
    /// <summary>
    /// Corresponds to SIMCONNECT_DATAType.
    /// </summary>
    public enum SimConnectDataType
    {
        /// <summary>
        /// invalid data Type
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// 32-bit integer number
        /// </summary>
        Int32,

        /// <summary>
        /// 64-bit integer number
        /// </summary>
        Int64,

        /// <summary>
        /// 32-bit floating-point number (Single)
        /// </summary>
        Float32,

        /// <summary>
        /// 64-bit floating-point number (Double)
        /// </summary>
        Float64,

        /// <summary>
        /// 8-byte String
        /// </summary>
        String8,

        /// <summary>
        /// 32-byte String
        /// </summary>
        String32,

        /// <summary>
        /// 64-byte String
        /// </summary>
        String64,

        /// <summary>
        /// 128-byte String
        /// </summary>
        String128,

        /// <summary>
        /// 256-byte String
        /// </summary>
        String256,

        /// <summary>
        /// 260-byte String
        /// </summary>
        String260,

        /// <summary>
        /// variable-length String
        /// </summary>
        StringV,

        /// <summary>
        /// see SIMCONNECT_DATA_INITPOSITION
        /// </summary>
        InitPosition,

        /// <summary>
        /// see SIMCONNECT_DATA_MARKERSTATE
        /// </summary>
        MarkerState,

        /// <summary>
        /// see SIMCONNECT_DATA_WAYPOINT
        /// </summary>
        Waypoint,

        /// <summary>
        /// see SIMCONNECT_DATA_LATLONALT
        /// </summary>
        LatLonAlt,

        /// <summary>
        /// see SIMCONNECT_DATA_XYZ
        /// </summary>
        XYZ
    }


    // see SimConnect documentation
    #pragma warning disable 1591
    public enum SimConnectPeriod
    {
        Never = 0,
        Once,
        VisualFrame,
        SimFrame,
        Second
    };


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SimConnectRecv
    {
        public uint dwSize;
        public uint dwVersion;
        public uint dwID;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SimConnectRecvException
    {
        public SimConnectRecv Recv;

        public uint dwException;
        public uint dwSendID;
        public uint dwIndex;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SimConnectRecvEvent
    {
        public SimConnectRecv Recv;

        public uint uGroupID;
        public uint uEventID;
        public uint dwData;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SimConnectRecvSimobjectData
    {
        public SimConnectRecv Recv;

        public uint dwRequestID;
        public uint dwObjectID;
        public uint dwDefineID;
        public uint dwFlags; // SIMCONNECT_DATA_REQUEST_FLAG
        public uint dwentrynumber;
        // if multiple objects returned, this is number <entrynumber> out of <outof>.
        public uint dwoutof; // note: starts with 1, not 0.
        public uint dwDefineCount; // data count (number of datums, *not* byte count)
        public uint dwData; // data begins here, dwDefineCount data items
    }


    /// <summary>
    /// Provides a low-level interface to a compatible SimConnect.dll.
    /// </summary>
    public interface ISimConnectLibrary : IAsyncDisposable
    {
        uint SimConnect_Open(out IntPtr phSimConnect, string szName, IntPtr hwnd, uint userEventWin32, IntPtr hEventHandle, uint configIndex);
        uint SimConnect_Close(IntPtr hSimConnect);

        uint SimConnect_AddToDataDefinition(IntPtr hSimConnect, uint defineID, string datumName, string unitsName, SimConnectDataType datumType = SimConnectDataType.Float64, float epsilon = 0, uint datumID = uint.MaxValue);
        uint SimConnect_ClearDataDefinition(IntPtr hSimConnect, uint defineID);

        uint SimConnect_RequestDataOnSimObject(IntPtr hSimConnect, uint requestID, uint defineID, uint objectID, SimConnectPeriod period, uint flags, uint origin = 0, uint interval = 0, uint limit = 0);
        uint SimConnect_GetNextDispatch(IntPtr hSimConnect, out IntPtr ppData, out uint pcbData);

        uint SimConnect_SubscribeToSystemEvent(IntPtr hSimConnect, uint eventID, string systemEventName);
        uint SimConnect_UnsubscribeFromSystemEvent(IntPtr hSimConnect, uint eventID);
    }



    public enum SimConnectRecvID
    {
        Null = 0,
        Exception,
        Open,
        Quit,
        Event,
        EventObjectAddRemove,
        EventFilename,
        EventFrame,
        SimobjectData,
        SimobjectDataByType,
        WeatherObservation,
        CloudState,
        AssignedObjectID,
        ReservedKey,
        CustomAction,
        SystemState,
        ClientData
    };
    #pragma warning restore 1591
}
