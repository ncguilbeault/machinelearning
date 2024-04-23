using Bonsai.Design;
using Bonsai;
using Bonsai.ML.LinearDynamicalSystems;
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

[assembly: TypeVisualizer(typeof(StateComponentMashup), Target = typeof(StateComponent))]

namespace Bonsai.ML.Visualizers
{
    public class StateComponentMashup : MashupVisualizer
    {
        private DateTime? _startTime;
        private TimeSpan updateFrequency = TimeSpan.FromSeconds(1/30);
        private DateTime? lastUpdate = null;

        private LineSeries lineSeries;

        private AreaSeries areaSeries;

        [XmlIgnore()]
        public TimeSeriesOxyPlotBase Plot { get; private set; }

        private int Capacity = 10;     

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

            StateComponent stateComponent = (StateComponent)value;
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

            var processedSource = source
                .SelectMany(innerSource => innerSource)
                .Do(value => Show(value));

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
    }
}
