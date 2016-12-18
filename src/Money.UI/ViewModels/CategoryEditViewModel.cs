﻿using Money.Services;
using Money.ViewModels.Commands;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Money.ViewModels
{
    public class CategoryEditViewModel : ObservableObject
    {
        public IKey Key { get; internal set; }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(ColorBrush));
                }
            }
        }

        public Brush ColorBrush
        {
            get { return new SolidColorBrush(Color); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand Rename { get; private set; }
        public ICommand ChangeColor { get; private set; }

        public CategoryEditViewModel(IDomainFacade domainFacade, IKey key)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(key, "key");
            Key = key;

            Rename = new CategoryRenameCommand(domainFacade, this);
            ChangeColor = new CategoryChangeColorCommand(domainFacade, this);
        }

        public CategoryEditViewModel(IDomainFacade domainFacade, IKey key, string name, string description, Color color)
            : this(domainFacade, key)
        {
            Name = name;
            Description = description;
            Color = color;
        }
    }
}
