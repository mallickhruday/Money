﻿using Money.UI;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OutcomePage : Page
    {
        public OutcomeViewModel ViewModel
        {
            get { return (OutcomeViewModel)DataContext; }
        }

        public OutcomePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            OutcomeViewModel viewModel = null;

            Guid? id = e.Parameter as Guid?;
            if (id != null)
            {
                // TODO: Load existing.
                viewModel = new OutcomeViewModel(App.Current.DomainFacade, id.Value);
            }
            else
            {
                viewModel = new OutcomeViewModel(App.Current.DomainFacade);
                OutcomeDefaultsModel defaults = e.Parameter as OutcomeDefaultsModel;
                if (defaults != null)
                {
                    if (defaults.Amount != null)
                        viewModel.Amount = (float)defaults.Amount.Value;

                    if (defaults.Description != null)
                        viewModel.Description = defaults.Description;
                }
            }

            viewModel.Categories.AddRange(new DesignData.ViewModelLocator().CreateOutcome.Categories);
            DataContext = viewModel;
        }

        private void gvwCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CategoryViewModel item in e.RemovedItems)
                ViewModel.SelectedCategories.Remove(item.Id);

            foreach (CategoryViewModel item in e.AddedItems)
                ViewModel.SelectedCategories.Add(item.Id);
        }
    }
}