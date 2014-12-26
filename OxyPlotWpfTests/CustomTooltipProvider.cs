using System;
using OxyPlot;

namespace OxyPlotWpfTests
{
    public class CustomTooltipProvider
    {
        public Func<TrackerHitResult, string> GetCustomTooltip { set; get; }
    }
}