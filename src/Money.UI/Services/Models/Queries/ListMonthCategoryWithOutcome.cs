﻿using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Models.Queries
{
    /// <summary>
    /// A query for getting list of categories with month outcome summary.
    /// </summary>
    public class ListMonthCategoryWithOutcome : IQuery<IEnumerable<CategoryWithAmountModel>>
    {
        public MonthModel Month { get; private set; }

        public ListMonthCategoryWithOutcome(MonthModel month)
        {
            Ensure.NotNull(month, "month");
            Month = month;
        }
    }
}
