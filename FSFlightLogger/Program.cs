using System;
using System.Windows.Forms;
using SimConnect;
using SimConnect.Concrete;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace FSFlightLogger
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var container = CreateContainer();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(container.GetInstance<MainForm>());
        }


        public static Container CreateContainer()
        {
            var container = new Container();

            // Since the MainForm is registered as well, do not call Verify on the container as it would create an instance
            container.Options.EnableAutoVerification = false;

            container.Register<MainForm>();
            container.Register<ISimConnectClientFactory, SimConnectClientFactory>();

            return container;
        }
    }
}
