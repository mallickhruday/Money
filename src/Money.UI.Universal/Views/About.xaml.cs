﻿using Money.Services;
using Money.UI;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(AboutParameter))]
    public sealed partial class About : Page
    {
        private readonly IDevelopmentService developmentTools = ServiceProvider.DevelopmentTools;
        private readonly RestartService restartService = ServiceProvider.RestartService;
        private readonly INavigator navigator = ServiceProvider.Navigator;

        public About()
        {
            InitializeComponent();

            DatabaseSwitch.IsOn = developmentTools.IsTestDatabaseEnabled();
            DevelopmentToolsTitle.Visibility = DevelopmentTools.Visibility;
        }

        private void DatabaseSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (DatabaseSwitch.IsOn != developmentTools.IsTestDatabaseEnabled())
            {
                string state = DatabaseSwitch.IsOn ? "on" : "off";
                string message = $"Do you really want to turn test database {state}?{Environment.NewLine}{Environment.NewLine}No data will be lost and after switching back, you can continue where left off.{Environment.NewLine}{Environment.NewLine}Application needs a restart to complete the operation.";

                navigator
                    .Message(message)
                    .Button("Yes", new SwitchDatabaseCommand(developmentTools, restartService, DatabaseSwitch.IsOn))
                    .ButtonClose("No")
                    .Show();
            }
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
            => await DevelopmentTools.ExportDataAsync();

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private class SwitchDatabaseCommand : Command
        {
            private readonly IDevelopmentService developmentTools;
            private readonly RestartService restartService;
            private readonly bool isEnabled;

            public SwitchDatabaseCommand(IDevelopmentService developmentTools, RestartService restartService, bool isEnabled)
            {
                Ensure.NotNull(developmentTools, "developmentTools");
                Ensure.NotNull(restartService, "restartService");
                this.developmentTools = developmentTools;
                this.restartService = restartService;
                this.isEnabled = isEnabled;
            }

            public override bool CanExecute()
            {
                return true;
            }

            public override async void Execute()
            {
                developmentTools.IsTestDatabaseEnabled(isEnabled);
                await restartService.RestartAsync();
            }
        }
    }
}
