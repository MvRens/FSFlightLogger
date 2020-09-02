using System;
using System.Threading.Tasks;
using SimConnect.Unmanaged;

namespace SimConnect.Concrete
{
    /// <summary>
    /// Default implementation of ISimConnectClientFactory
    /// </summary>
    public class SimConnectClientFactory : ISimConnectClientFactory
    {
        /// <inheritdoc />
        public async Task<ISimConnectClient> TryConnect(string appName)
        {
            // FS 2020 SimConnect.dll is 64-bits, the others are 32-bits
            if (Environment.Is64BitProcess)
                return await TryDefaultClient(appName, "FS2020-SimConnect.dll");

            // This order prevents a version mismatch, but perhaps an option to explicitly set the version might be nice as well
            return await TryDefaultClient(appName, "FSX-SimConnect.dll")
                ?? await TryDefaultClient(appName, "FSXSP2-SimConnect.dll")
                ?? await TryDefaultClient(appName, "FSX-SE-SimConnect.dll");
        }


        private static async Task<ISimConnectClient> TryDefaultClient(string appName, string libraryFilename)
        {
            var library = new SimConnectDLLLibrary(libraryFilename);
            var client = new DefaultSimConnectClient(library);

            return await client.TryOpen(appName) ? client : null;
        }
    }
}
