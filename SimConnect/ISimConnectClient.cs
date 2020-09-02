using System;

namespace SimConnect
{
    /// <summary>
    /// Called when new data arrives from the SimConnect server.
    /// </summary>
    /// <param name="data">An instance of the data class as passed to AddDefinition containing the variable values</param>
    /// <typeparam name="T">The data class as passed to AddDefinition</typeparam>
    public delegate void SimConnectDataHandlerAction<in T>(T data) where T : class;


    /// <summary>
    /// Gets notified of changes to the SimConnect state.
    /// </summary>
    public interface ISimConnectClientObserver
    {
        /// <summary>
        /// Gets called when the SimConnect connection is lost. The client will not receive further notifications,
        /// a new connection should be attempted if desired.
        /// </summary>
        void OnQuit();
    }


    /// <summary>
    /// Provides access to the SimConnect library.
    /// </summary>
    public interface ISimConnectClient : IDisposable
    {
        /// <summary>
        /// Attaches the specified observer to receive status notifications.
        /// </summary>
        /// <param name="observer">The observer to receive status notifications</param>
        void AttachObserver(ISimConnectClientObserver observer);


        /// <summary>
        /// Registers a definition to receive updates from the SimConnect server.
        /// </summary>
        /// <param name="onData">A callback method which is called whenever a data update is received</param>
        /// <typeparam name="T">A class defining the variables to monitor annotated using the SimConnectVariable attribute</typeparam>
        /// <returns>An IDisposable which can be disposed to unregister the definition. Dispose is not required to be called when the client is disconnected, but will not throw an exception.</returns>
        IDisposable AddDefinition<T>(SimConnectDataHandlerAction<T> onData) where T : class;
    }
}
