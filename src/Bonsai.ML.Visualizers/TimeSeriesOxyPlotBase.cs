using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Drawing;
using System;
using OxyPlot.Axes;
using System.Collections;
using Bonsai.Design;

namespace Bonsai.ML.Visualizers
{
    public class TimeSeriesOxyPlotBase : UserControl
    {
        private PlotView view;
        private PlotModel model;
        private OxyColor defaultLineSeriesColor = OxyColors.Blue;
        private OxyColor defaultAreaSeriesColor = OxyColors.LightBlue;

        // private LineSeries lineSeries;
        // private AreaSeries areaSeries;
        private Axis xAxis;
        private Axis yAxis;

        private StatusStrip statusStrip;

        /// <summary>
        /// Event handler which can be used to hook into events generated when the combobox values have changed.
        /// </summary>
        public event EventHandler ComboBoxValueChanged;

        /// <summary>
        /// DateTime value that determines the starting time of the data values.
        /// </summary>
        public DateTime StartTime {get;set;}

        /// <summary>
        /// Integer value that determines how many data points should be shown along the x axis.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Constructor of the TimeSeriesOxyPlotBase class.
        /// Requires a line series name and an area series name.
        /// Data source is optional, since pasing it to the constructor will populate the combobox and leave it empty otherwise.
        /// The selected index is only needed when the data source is provided.
        /// </summary>
        public TimeSeriesOxyPlotBase()
        {
            // _lineSeriesName = lineSeriesName;
            // _areaSeriesName = areaSeriesName;
            Initialize();
        }

        private void Initialize()
        {
            view = new PlotView
            {
                Size = Size,
                Dock = DockStyle.Fill,
            };

            model = new PlotModel();

            // lineSeries = new LineSeries {
            //     Title = _lineSeriesName,
            //     Color = OxyColors.Blue
            // };

            // areaSeries = new AreaSeries {
            //     Title = _areaSeriesName,
            //     Color = OxyColors.LightBlue,
            //     Fill = OxyColor.FromArgb(100, 173, 216, 230)
            // };

            xAxis = new DateTimeAxis {
                Position = AxisPosition.Bottom,
                Title = "Time",
                StringFormat = "HH:mm:ss",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MinorIntervalType = DateTimeIntervalType.Auto,
                IntervalType = DateTimeIntervalType.Auto,
                FontSize = 9
            };

            yAxis = new LinearAxis {
                Position = AxisPosition.Left,
                Title = "Value",
            };

            model.Axes.Add(xAxis);
            model.Axes.Add(yAxis);

            // model.Series.Add(lineSeries);
            // model.Series.Add(areaSeries);

            view.Model = model;
            Controls.Add(view);

            statusStrip = new StatusStrip
            {
                Visible = false
            };

            view.MouseClick += new MouseEventHandler(onMouseClick);
            Controls.Add(statusStrip);

            AutoScaleDimensions = new SizeF(6F, 13F);
        }

        private void onMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                statusStrip.Visible = !statusStrip.Visible;
            }
        }

        public void AddComboBoxWithLabel(string label, IEnumerable dataSource, int selectedIndex, EventHandler onComboBoxSelectionChanged)
        {
            ToolStripLabel toolStripLabel = new ToolStripLabel(label) { AutoSize = true };
            ToolStripComboBox toolStripComboBox = new ToolStripComboBox() { AutoSize = true };

            foreach (var value in dataSource)
            {
                toolStripComboBox.Items.Add(value);
            }

            toolStripComboBox.SelectedIndexChanged += onComboBoxSelectionChanged;
            toolStripComboBox.SelectedIndex = selectedIndex;

            statusStrip.Items.AddRange(new ToolStripItem[] {
                toolStripLabel,
                toolStripComboBox
            });
       
        }

        public LineSeries AddNewLineSeries(string lineSeriesName, OxyColor? color = null)
        {
            OxyColor _color = color.HasValue ? color.Value : defaultLineSeriesColor;
            LineSeries lineSeries = new LineSeries {
                Title = lineSeriesName,
                Color = _color
            };
            model.Series.Add(lineSeries);
            return lineSeries;
        }

        public AreaSeries AddNewAreaSeries(string areaSeriesName, OxyColor? color = null, OxyColor? fill = null, byte opacity = 100)
        {
            OxyColor _color = color.HasValue ? color.Value : defaultAreaSeriesColor;
            OxyColor _fill = fill.HasValue? fill.Value : OxyColor.FromArgb(opacity, _color.R, _color.G, _color.B);
            AreaSeries areaSeries = new AreaSeries {
                Title = areaSeriesName,
                Color = _color,
                Fill = _fill
            };
            model.Series.Add(areaSeries);
            return areaSeries;
        }

        public void AddToLineSeries(LineSeries lineSeries, DateTime time, double mean)
        {
            lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), mean));
        }

        public void AddToAreaSeries(AreaSeries areaSeries, DateTime time, double mean, double variance)
        {
            areaSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), mean + variance));
            areaSeries.Points2.Add(new DataPoint(DateTimeAxis.ToDouble(time), mean - variance));
        }

        public void SetAxes(DateTime minTime, DateTime maxTime)
        {
            xAxis.Minimum = DateTimeAxis.ToDouble(minTime);
            xAxis.Maximum = DateTimeAxis.ToDouble(maxTime);
        }

        public void UpdatePlot()
        {
            model.InvalidatePlot(true);
        }

        public void ResetLineSeries(LineSeries lineSeries)
        {
            lineSeries.Points.Clear();
        }

        public void ResetAreaSeries(AreaSeries areaSeries)
        {
            areaSeries.Points.Clear();
            areaSeries.Points2.Clear();
        }

        public void ResetModelSeries()
        {
            model.Series.Clear();
        }

        public void ResetAxes()
        {
            xAxis.Reset();
            yAxis.Reset();

            xAxis.Minimum = DateTimeAxis.ToDouble(StartTime.AddSeconds(-Capacity));
            xAxis.Maximum = DateTimeAxis.ToDouble(StartTime);
        }
    }
}
