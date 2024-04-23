using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using Bonsai;
using Bonsai.Design;
using Bonsai.ML.Visualizers;
using Bonsai.ML.LinearDynamicalSystems;
using Bonsai.ML.LinearDynamicalSystems.Kinematics;
using System.Drawing;
using System.Reactive;
using OxyPlot.Series;

[assembly: TypeVisualizer(typeof(KinematicStateVisualizer), Target = typeof(KinematicState))]

namespace Bonsai.ML.Visualizers
{
    /// <summary>
    /// Provides a type visualizer to display the state components of a Kalman Filter kinematics model.
    /// </summary>
    public class KinematicStateVisualizer : BufferedVisualizer
    {
        private PropertyInfo stateComponentProperty;

        private PropertyInfo kinematicComponentProperty;

        private int selectedStateIndex = 0;

        private int selectedKinematicIndex = 0;

        private DateTime? _startTime;

        private TimeSeriesOxyPlotBase Plot;

        private LineSeries lineSeries;

        private AreaSeries areaSeries;

        /// <summary>
        /// The selected index of the state component to be visualized
        /// </summary>
        public int SelectedStateIndex { get => selectedStateIndex; set => selectedStateIndex = value; }

        /// <summary>
        /// The selected index of the kinematic component to be visualized
        /// </summary>
        public int SelectedKinematicIndex { get => selectedKinematicIndex; set => selectedKinematicIndex = value; }

        /// <summary>
        /// Size of the window when loaded
        /// </summary>
        public Size Size { get; set; } = new Size(320, 240);

        /// <summary>
        /// Capacity or length of time shown along the x axis of the plot during automatic updating
        /// </summary>
        public int Capacity { get; set; } = 10;

        /// <inheritdoc/>
        public override void Load(IServiceProvider provider)
        {
            Plot = new TimeSeriesOxyPlotBase()
            {
                Size = Size,
                Capacity = Capacity,
                Dock = DockStyle.Fill,
                StartTime = DateTime.Now
            };

            lineSeries = Plot.AddNewLineSeries("Mean");
            areaSeries = Plot.AddNewAreaSeries("Variance");

            Plot.ResetLineSeries(lineSeries);
            Plot.ResetAreaSeries(areaSeries);
            Plot.ResetAxes();

            Plot.AddComboBoxWithLabel("State component:", LinearDynamicalSystemsHelper.GetStateComponents(), selectedStateIndex, StateComponentChanged);
            Plot.AddComboBoxWithLabel("Kinematic component:", LinearDynamicalSystemsHelper.GetKinematicComponents(), selectedKinematicIndex, KinematicComponentChanged);

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(Plot);
            }
        }

        /// <inheritdoc/>
        public override void Show(object value)
        {
        }

        /// <inheritdoc/>
        protected override void Show(DateTime time, object value)
        {
            if (!_startTime.HasValue)
            {
                _startTime = time;
                Plot.StartTime = _startTime.Value;
                // Plot.ResetSeries();
                Plot.ResetAxes();
            }

            KinematicState kinematicState = (KinematicState)value;
            KinematicComponent kinematicComponent = (KinematicComponent)kinematicComponentProperty.GetValue(kinematicState);
            StateComponent stateComponent = (StateComponent)stateComponentProperty.GetValue(kinematicComponent);

            double mean = stateComponent.Mean;
            double variance = stateComponent.Variance;

            Plot.AddToLineSeries(
                lineSeries: lineSeries,
                time: time,
                mean: mean
            );

            Plot.AddToAreaSeries(
                areaSeries: areaSeries,
                time: time,
                mean: mean,
                variance: variance
            );

            Plot.SetAxes(minTime: time.AddSeconds(-Capacity), maxTime: time);

        }

        /// <inheritdoc/>
        protected override void ShowBuffer(IList<Timestamped<object>> values)
        {
            base.ShowBuffer(values);
            if (values.Count > 0)
            {
                Plot.UpdatePlot();
            }
        }

        /// <inheritdoc/>
        public override void Unload()
        {
            _startTime = null;
            if (!Plot.IsDisposed)
            {
                Plot.Dispose();
            }
        }

        /// <summary>
        /// Callback function to update the visualizer when the selected state component has changed
        /// </summary>
        private void StateComponentChanged(object sender, EventArgs e)
        {
            ToolStripComboBox comboBox = sender as ToolStripComboBox;
            selectedStateIndex = comboBox.SelectedIndex;
            var selectedName = comboBox.SelectedItem.ToString();
            stateComponentProperty = typeof(KinematicComponent).GetProperty(selectedName);
            _startTime = null;

            Plot.ResetLineSeries(lineSeries);
            Plot.ResetAreaSeries(areaSeries);
            Plot.ResetAxes();
        }

        /// <summary>
        /// Callback function to update the visualizer when the selected kinematic component has changed
        /// </summary>
        private void KinematicComponentChanged(object sender, EventArgs e)
        {
            ToolStripComboBox comboBox = sender as ToolStripComboBox;
            selectedKinematicIndex = comboBox.SelectedIndex;
            var selectedName = comboBox.SelectedItem.ToString();
            kinematicComponentProperty = typeof(KinematicState).GetProperty(selectedName);
            _startTime = null;

            Plot.ResetLineSeries(lineSeries);
            Plot.ResetAreaSeries(areaSeries);
            Plot.ResetAxes();
        }
    }
}