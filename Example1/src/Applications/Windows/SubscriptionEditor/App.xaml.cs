using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;

namespace SubscriptionEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var needUpdate = true;

            try
            {
                needUpdate = Task.Run(NeedUpdateAsync).Result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            if (needUpdate==false)
            {
                StartCore();
            }
        }

        private void StartCore()
        {
            MessageBox.Show("StartApplicationCore");
        }

        private async Task<bool> NeedUpdateAsync()
        {
            // Check for Squirrel application update
            ReleaseEntry release = null;
            using (var mgr = new UpdateManager(ConfigurationManager.AppSettings["SquirellUpdatePath"]))
            {
                //
                var updateInfo = await mgr.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Any()) // Check if we have any update
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

                    var msg = "New version available!" +
                                 "\n\nCurrent version: " + updateInfo.CurrentlyInstalledVersion.Version +
                                 "\nNew version: " + updateInfo.FutureReleaseEntry.Version;
                    MessageBox.Show(msg, fvi.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                    // Do the update
                    release = await mgr.UpdateApp();
                }
            }

            // Restart the app
            if (release == null)
            {
                return false;
            }

            UpdateManager.RestartApp();
            return true;
        }
    }
}
