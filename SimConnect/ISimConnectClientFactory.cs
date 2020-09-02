using System.Threading.Tasks;

namespace SimConnect
{
    /// <summary>
    /// Provides a factory for creating a SimConnect client instance.
    /// </summary>
    public interface ISimConnectClientFactory
    {
        /// <summary>
        /// Tries to connect to any of the compatible running SimConnect servers.
        /// </summary>
        /// <param name="appName">The application name passed to the SimConnect server.</param>
        /// <returns>A client interface if succesful or nil if no connection could be made.</returns>
        Task<ISimConnectClient> TryConnect(string appName);
    }
}
