﻿using Money.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Controls
{
    public sealed partial class CategoryEdit : UserControl
    {
        public CategoryEdit()
        {
            InitializeComponent();
        }

        private void grvColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if ((Color)grvColors.SelectedItem != ((CategoryListItemViewModel)DataContext).Color)
            //    flyColor.Hide();
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            CategoryEditViewModel viewModel = e.NewValue as CategoryEditViewModel;
            if (viewModel != null)
            {
                if (viewModel.Key.IsEmpty)
                    flyRename.ShowAt(btnRename);
            }
        }
    }
}