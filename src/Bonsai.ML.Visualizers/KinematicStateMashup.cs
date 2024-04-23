using Bonsai.Design;
using Bonsai;
using Bonsai.ML.LinearDynamicalSystems;
using Bonsai.ML.LinearDynamicalSystems.Kinematics;
using Bonsai.ML.Visualizers;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Reactive.Linq;
using OxyPlot.Series;
using System.Reactive;
using System.Threading;
using System.Linq;
using System.Drawing;
using System.Reflection;

[assembly: TypeVisualizer(typeof(KinematicStateMashup), Target = typeof(KinematicState))]

namespace Bonsai.ML.Visualizers
{
    public class KinematicStateMashup : MashupVisualizer
    {
        [XmlIgnore()]
        public PropertyInfo stateComponentProperty { get; private set; }

        [XmlIgnore()]
        public PropertyInfo kinematicComponentProperty { get; private set; }

        private int selectedStateIndex = 0;

        private int selectedKinematicIndex = 0;

        private DateTime? _startTime;
        private TimeSpan updateFrequency = TimeSpan.FromSeconds(1/30);
        private DateTime? lastUpdate = null;

        private LineSeries lineSeries;

        private AreaSeries areaSeries;

        [XmlIgnore()]
        public TimeSeriesOxyPlotBase Plot { get; private set; }

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
                Dock = DockStyle.Fill,
                StartTime = DateTime.Now,
                Capacity = Capacity
            };

            lineSeries = Plot.AddNewLineSeries("Mean");
            areaSeries = Plot.AddNewAreaSeries("Variance");

            Plot.ResetLineSeries(lineSeries);
            Plot.ResetAreaSeries(areaSeries);
            Plot.ResetAxes();

            List<string> stateComponents = LinearDynamicalSystemsHelper.GetStateComponents();
            List<string> kinematicComponents = LinearDynamicalSystemsHelper.GetKinematicComponents();

            Plot.AddComboBoxWithLabel("State component:", stateComponents, selectedStateIndex, StateComponentChanged);
            Plot.AddComboBoxWithLabel("Kinematic component:", kinematicComponents, selectedKinematicIndex, KinematicComponentChanged);

            stateComponentProperty = typeof(KinematicComponent).GetProperty(stateComponents[selectedStateIndex]);
            kinematicComponentProperty = typeof(KinematicState).GetProperty(kinematicComponents[selectedKinematicIndex]);

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(Plot);
            }
            _startTime = null;

            base.Load(provider);
        }


        /// <inheritdoc/>
        public override void Show(object value)
        {
            var time = DateTime.Now;
            if (!_startTime.HasValue)
            {
                _startTime = time;
                Plot.StartTime = _startTime.Value;
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

            if (MashupSources.Count == 0) Plot.SetAxes(minTime: time.AddSeconds(-Capacity), maxTime: time);

            if (lastUpdate is null || time - lastUpdate > updateFrequency)
            {
                lastUpdate = time;
                Plot.UpdatePlot();
            }
        }

        /// <inheritdoc/>
        public override IObservable<object> Visualize(IObservable<IObservable<object>> source, IServiceProvider provider)
        {
            var mashupSourceStreams = MashupSources.Select(mashupSource =>
                mashupSource.Visualizer.Visualize(mashupSource.Source.Output, provider)
                    .Do(value => mashupSource.Visualizer.Show(value)));

            var mergedMashupSources = Observable.Merge(mashupSourceStreams);

            var processedSource = base.Visualize(source, provider);

            return Observable.Merge(mergedMashupSources, processedSource);
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
