using Bonsai;
using Bonsai.Design;
using System;
using System.Collections.Generic;
using OxyPlot.Series;
using OxyPlot;
using Bonsai.ML.Design;
using System.Windows.Forms;
using System.Reactive.Linq;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;

[assembly: TypeVisualizer(typeof(Bonsai.ML.NeuralDecoder.Design.Posterior1DVisualizer),
    Target = typeof(Bonsai.ML.NeuralDecoder.Posterior))]

namespace Bonsai.ML.NeuralDecoder.Design
{
    /// <summary>
    /// Provides a mashup visualizer to display the posterior of the neural decoder.
    /// </summary>    
    public class Posterior1DVisualizer : MashupVisualizer
    {
        private UnidimensionalArrayTimeSeriesVisualizer visualizer;
        private LineSeries lineSeries;
        private List<double> argMaxVals = new();
        private double[] valueCenters = null;
        private double[] valueRange = null;

        /// <summary>
        /// Gets the underlying heatmap plot.
        /// </summary>
        public HeatMapSeriesOxyPlotBase Plot => visualizer.Plot;
        
        /// <summary>
        /// Gets the capacity of the visualizer.
        /// </summary>
        public int Capacity => visualizer.Capacity;

        /// <summary>
        /// Gets the current count of data points.
        /// </summary>
        public int CurrentCount => visualizer.CurrentCount;

        /// <summary>
        /// Gets the values of the Y axis.
        /// </summary>
        [XmlIgnore]
        public double[] ValueCenters => valueCenters;

        /// <summary>
        /// Gets the range of values mapped to the values of the Y axis.
        /// </summary>
        [XmlIgnore]
        public double[] ValueRange => valueRange;

        /// <inheritdoc/>
        public override void Load(IServiceProvider provider)
        {
            visualizer = new UnidimensionalArrayTimeSeriesVisualizer()
            {
                PaletteSelectedIndex = 1,
                RenderMethodSelectedIndex = 1
            };
            
            visualizer.Load(provider);

            // lineSeries = new LineSeries()
            // {
            //     Title = "Maximum Posterior",
            //     Color = OxyColors.SkyBlue
            // };
            // visualizer.Plot.Model.Series.Add(lineSeries);

            base.Load(provider);
        }

        /// <inheritdoc/>
        public override void Show(object value)
        {
            Posterior posterior = (Posterior)value;
            if (posterior == null)
            {
                return;
            }

            if (valueCenters == null)
            {
                valueCenters = posterior.ValueCenters;
            }

            if (valueRange == null)
            {
                valueRange = posterior.ValueRange;
            }

            var data = posterior.Data;
            // var argMax = posterior.ArgMax;

            // while (argMaxVals.Count >= Capacity)
            // {
            //     argMaxVals.RemoveAt(0);
            // }

            // argMaxVals.Add(valueCenters[argMax]);
            // lineSeries.Points.Clear();
            // var count = argMaxVals.Count;

            // for (int i = 0; i < count; i++)
            // {
            //     lineSeries.Points.Add(new DataPoint(i, argMaxVals[i]));
            // }
            
            visualizer.Show(data);
            Plot.UpdateHeatMapYAxis(valueRange[0], valueRange[valueRange.Length - 1]);
        }

        /// <inheritdoc/>
        public override void Unload()
        {
            visualizer.Unload();
            base.Unload();
        }

        /// <inheritdoc/>
        public override IObservable<object> Visualize(IObservable<IObservable<object>> source, IServiceProvider provider)
        {
            if (provider.GetService(typeof(IDialogTypeVisualizerService)) is not Control visualizerControl)
            {
                return source;
            }

            var timer = Observable.Interval(TimeSpan.FromMilliseconds(100));

            IObservable<object> SynchronizeAndShow(IObservable<object> stream, Action<object> showAction)
            {
                return stream
                    .Buffer(timer)
                    .Where(buffer => buffer.Count > 0)
                    .SelectMany(buffer =>
                    {
                        foreach (var item in buffer)
                        {
                            showAction(item);
                        }
                        return buffer;
                    });
            }

            var mergedSource = source.SelectMany(xs =>
                SynchronizeAndShow(xs, value => Show(value)));

            var mashupSourceStreams = Observable.Merge(
                MashupSources.Select(mashupSource =>
                    SynchronizeAndShow(
                        mashupSource.Source.Output.SelectMany(xs => xs),
                        value => mashupSource.Visualizer.Show(value))));

            return Observable.Merge(mergedSource, mashupSourceStreams)
                .ObserveOn(visualizerControl);
        }
    }
}
