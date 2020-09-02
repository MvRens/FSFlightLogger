﻿using System;
using SimConnect.Lib;

#pragma warning disable 1591
namespace SimConnect.Unmanaged
{
    public class SimConnectDLLLibrary : ISimConnectLibrary, IDisposable
    {
        private delegate uint SimConnectOpenProc(out IntPtr phSimConnect, string szName, IntPtr hwnd, uint userEventWin32, IntPtr hEventHandle, uint configIndex);
        private delegate uint SimConnectCloseProc(IntPtr hSimConnect);
        private delegate uint SimConnectAddToDataDefinitionProc(IntPtr hSimConnect, uint defineID, string datumName, string unitsName, SimConnectDataType datumType = SimConnectDataType.Float64, float epsilon = 0, uint datumID = uint.MaxValue);
        private delegate uint SimConnectClearDataDefinitionProc(IntPtr hSimConnect, uint defineID);
        private delegate uint SimConnectRequestDataOnSimObjectProc(IntPtr hSimConnect, uint requestID, uint defineID, uint objectID, SimConnectPeriod period, uint flags, uint origin = 0, uint interval = 0, uint limit = 0);
        private delegate uint SimConnectGetNextDispatchProc(IntPtr hSimConnect, out IntPtr ppData, out uint pcbData);

        private readonly UnmanagedLibrary library;

        private readonly SimConnectOpenProc simConnectOpen;
        private readonly SimConnectCloseProc simConnectClose;
        private readonly SimConnectAddToDataDefinitionProc simConnectAddToDataDefinition;
        private readonly SimConnectClearDataDefinitionProc simConnectClearDataDefinition;
        private readonly SimConnectRequestDataOnSimObjectProc simConnectRequestDataOnSimObject;
        private readonly SimConnectGetNextDispatchProc simConnectGetNextDispatch;



        public SimConnectDLLLibrary(string libraryFilename)
        {
            library = new UnmanagedLibrary(libraryFilename);
            simConnectOpen = library.GetUnmanagedFunction<SimConnectOpenProc>("SimConnect_Open");
            simConnectClose = library.GetUnmanagedFunction<SimConnectCloseProc>("SimConnect_Close");
            simConnectAddToDataDefinition = library.GetUnmanagedFunction<SimConnectAddToDataDefinitionProc>("SimConnect_AddToDataDefinition");
            simConnectClearDataDefinition = library.GetUnmanagedFunction<SimConnectClearDataDefinitionProc>("SimConnect_ClearDataDefinition");
            simConnectRequestDataOnSimObject = library.GetUnmanagedFunction<SimConnectRequestDataOnSimObjectProc>("SimConnect_RequestDataOnSimObject");
            simConnectGetNextDispatch = library.GetUnmanagedFunction<SimConnectGetNextDispatchProc>("SimConnect_GetNextDispatch");
        }

        public void Dispose()
        {
            library?.Dispose();
        }


        public uint SimConnect_Open(out IntPtr phSimConnect, string szName, IntPtr hwnd, uint userEventWin32, IntPtr hEventHandle, uint configIndex)
        {
            return simConnectOpen(out phSimConnect, szName, hwnd, userEventWin32, hEventHandle, configIndex);
        }

        public uint SimConnect_Close(IntPtr hSimConnect)
        {
            return simConnectClose(hSimConnect);
        }


        public uint SimConnect_AddToDataDefinition(IntPtr hSimConnect, uint defineID, string datumName, string unitsName, SimConnectDataType datumType = SimConnectDataType.Float64, float epsilon = 0, uint datumID = uint.MaxValue)
        {
            return simConnectAddToDataDefinition(hSimConnect, defineID, datumName, unitsName, datumType, epsilon, datumID);
        }

        public uint SimConnect_ClearDataDefinition(IntPtr hSimConnect, uint defineID)
        {
            return simConnectClearDataDefinition(hSimConnect, defineID);
        }


        public uint SimConnect_RequestDataOnSimObject(IntPtr hSimConnect, uint requestID, uint defineID, uint objectID, SimConnectPeriod period, uint flags, uint origin = 0, uint interval = 0, uint limit = 0)
        {
            return simConnectRequestDataOnSimObject(hSimConnect, requestID, defineID, objectID, period, flags, origin, interval, limit);
        }

        public uint SimConnect_GetNextDispatch(IntPtr hSimConnect, out IntPtr ppData, out uint pcbData)
        {
            return simConnectGetNextDispatch(hSimConnect, out ppData, out pcbData);
        }
    }
}
#pragma warning restore 1591
