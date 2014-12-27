using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace OxyPlotWpfTests
{
    public class MainWindowViewModel
    {
        private readonly LineSeries _lineSeries1 = new LineSeries();
        private readonly LineSeries _lineSeries2 = new LineSeries();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly LinearAxis _xAxis = new LinearAxis();
        private readonly LinearAxis _yAxis = new LinearAxis();
        private IPlotController _controller;
        private bool _haveNewPoints;
        private long _lastUpdateMilliseconds;
        private int _xMax;
        private int _yMax;

        public MainWindowViewModel()
        {
            initPlot();
            addPoints();
            updatePlot();
            _stopwatch.Start();
        }

        /// <summary>
        /// show tracker with mouse move
        /// </summary>
        public IPlotController Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = new PlotController();
                    _controller.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);
                }
                return _controller;
            }
        }

        public CustomTooltipProvider CustomTooltipProvider { set; get; }

        public PlotModel PlotModel { get; set; }

        private void addLineSeries1()
        {
            _lineSeries1.MarkerType = MarkerType.Circle;
            _lineSeries1.StrokeThickness = 2;
            _lineSeries1.MarkerSize = 3;
            _lineSeries1.Title = "Start";
            _lineSeries1.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    PlotModel.Subtitle = "Index of nearest point in LineSeries: " + Math.Round(e.HitTestResult.Index);
                    PlotModel.InvalidatePlot(false);
                }
            };
            PlotModel.Series.Add(_lineSeries1);
        }

        private void addLineSeries2()
        {
            _lineSeries2.MarkerType = MarkerType.Circle;
            _lineSeries2.Title = "End";
            _lineSeries2.StrokeThickness = 2;
            _lineSeries2.MarkerSize = 3;
            _lineSeries2.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    PlotModel.Subtitle = "Index of nearest point in LineSeries: " + Math.Round(e.HitTestResult.Index);
                    PlotModel.InvalidatePlot(false);
                }
            };
            PlotModel.Series.Add(_lineSeries2);
        }

        private void addPoints()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            var rnd = new Random();
            var x = 1;
            updateXMax(x);
            timer.Tick += (sender, args) =>
            {
                var y1 = rnd.Next(100);
                updateYMax(y1);
                _lineSeries1.Points.Add(new DataPoint(x, y1));

                var y2 = rnd.Next(100);
                updateYMax(y2);
                _lineSeries2.Points.Add(new DataPoint(x, rnd.Next(y2)));

                x++;

                updateXMax(x);
                _haveNewPoints = true;
            };
            timer.Start();
        }

        private void addXAxis()
        {
            _xAxis.Minimum = 0;
            _xAxis.MaximumPadding = 1;
            _xAxis.MinimumPadding = 1;
            _xAxis.Position = AxisPosition.Bottom;
            _xAxis.Title = "X axis";
            _xAxis.MajorGridlineStyle = LineStyle.Solid;
            _xAxis.MinorGridlineStyle = LineStyle.Dot;
            PlotModel.Axes.Add(_xAxis);
        }

        private void addYAxis()
        {
            _yAxis.Minimum = 0;
            _yAxis.Title = "Y axis";
            _yAxis.MaximumPadding = 1;
            _yAxis.MinimumPadding = 1;
            _yAxis.MajorGridlineStyle = LineStyle.Solid;
            _yAxis.MinorGridlineStyle = LineStyle.Dot;
            PlotModel.Axes.Add(_yAxis);
        }

        private void createPlotModel()
        {
            CustomTooltipProvider = new CustomTooltipProvider { GetCustomTooltip = getCustomTooltip };
            PlotModel = new PlotModel
                {
                    Title = "سرى خطوط",
                    Subtitle = "Pan (right click and drag)/Zoom (Middle click and drag)/Reset (double-click)"
                };
            PlotModel.MouseDown += (sender, args) =>
            {
                if (args.ChangedButton == OxyMouseButton.Left && args.ClickCount == 2)
                {
                    foreach (var axis in PlotModel.Axes)
                        axis.Reset();

                    PlotModel.InvalidatePlot(false);
                }
            };
        }

        private string getCustomTooltip(TrackerHitResult hitResult)
        {
            var lineSeries = hitResult.Series as LineSeries;
            var nearestPointIndex = hitResult.Index;
            return "nearestPointIndex: " + nearestPointIndex;
        }

        private void initPlot()
        {
            createPlotModel();
            addXAxis();
            addYAxis();
            addLineSeries1();
            addLineSeries2();
        }

        private void updatePlot()
        {
            CompositionTarget.Rendering += (sender, args) =>
            {
                if (_stopwatch.ElapsedMilliseconds > _lastUpdateMilliseconds + 2000 && _haveNewPoints)
                {
                    if (_yMax > 0 && _xMax > 0)
                    {
                        _yAxis.Maximum = _yMax + 3;
                        _xAxis.Maximum = _xMax + 1;
                    }

                    PlotModel.InvalidatePlot(false);

                    _haveNewPoints = false;
                    _lastUpdateMilliseconds = _stopwatch.ElapsedMilliseconds;
                }
            };
        }

        private void updateXMax(int value)
        {
            if (value > _xMax)
            {
                _xMax = value;
            }
        }

        private void updateYMax(int value)
        {
            if (value > _yMax)
            {
                _yMax = value;
            }
        }
    }
}