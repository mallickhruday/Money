﻿using Microsoft.AspNetCore.Components;
using Money.Models.Loading;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class LoadingBase : ComponentBase
    {
        [Inject]
        internal ILog<LoadingBase> Log { get; set; }

        [Parameter]
        protected LoadingContext Context { get; set; }

        [Parameter]
        protected bool IsOverlay { get; set; }

        protected bool IsLoading { get; set; }

        [Parameter]
        protected RenderFragment ChildContent { get; set; }

        [Parameter]
        protected RenderFragment LoadingContent { get; set; } = r => r.AddMarkupContent(1, "<i>Loading...</i>");

        public override async Task SetParametersAsync(ParameterCollection parameters)
        {
            if (Context != null)
                Context.LoadingChanged -= OnLoadingChanged;

            await base.SetParametersAsync(parameters);

            if (Context != null)
            {
                Context.LoadingChanged += OnLoadingChanged;
                OnLoadingChanged(Context);
            }
        }

        protected void OnLoadingChanged(LoadingContext context)
        {
            Log.Debug($"Loading changed to '{context.IsLoading}'.");
            IsLoading = Context.IsLoading;
            StateHasChanged();
        }
    }
}
