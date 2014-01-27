﻿ 
using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace Player.Installer
{
    public class PlayerInstaller : BootstrapperApplication
    {

        // global dispatcher
        static public Dispatcher BootstrapperDispatcher { get; private set; }

        // entry point for our custom UI
        protected override void Run()
        {
            this.Engine.Log(LogLevel.Verbose, "Launching custom Installer UX");
            BootstrapperDispatcher = Dispatcher.CurrentDispatcher;

            MainViewModel viewModel = new MainViewModel(this);
            viewModel.Bootstrapper.Engine.Detect();

            MainView view = new MainView();
            view.DataContext = viewModel;
            view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
            view.Show();

            Dispatcher.Run();

            this.Engine.Quit(0);
        }
    }
}
