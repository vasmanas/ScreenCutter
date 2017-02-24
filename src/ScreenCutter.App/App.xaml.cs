using System.Windows;
using System.Windows.Threading;
using log4net;

namespace ScreenCutter.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public App()
        {
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

        }

        public void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Prevent default unhandled exception processing
            //e.Handled = true;

            Log.ErrorFormat(
                "Error occured: {0}{1}",
                e.Exception,
                (e.Exception.InnerException != null ? ", " + e.Exception.InnerException.Message : string.Empty));
        }
    }
}
