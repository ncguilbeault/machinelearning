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
using System.Reflection;

[assembly: TypeVisualizer(typeof(Bonsai.ML.NeuralDecoder.Design.Posterior2DVisualizer),
    Target = typeof(Bonsai.ML.NeuralDecoder.Posterior2D))]

namespace Bonsai.ML.NeuralDecoder.Design
{
    /// <summary>
    /// Provides a mashup visualizer to display the posterior of the neural decoder.
    /// </summary>    
    public class Posterior2DVisualizer : MashupVisualizer
    {
        private MultidimensionalArrayVisualizer visualizer;
        private LineSeries lineSeries;
        private List<int[]> argMaxVals = new();

        /// <summary>
        /// Gets the underlying heatmap plot.
        /// </summary>
        public HeatMapSeriesOxyPlotBase Plot => visualizer.Plot;

        private int _capacity = 100;

        /// <summary>
        /// Gets or sets the integer value that determines how many data points should be shown along the x axis.
        /// </summary>
        public int Capacity 
        { 
            get => _capacity;
            set 
            {
                _capacity = value;
            } 
        }
        
        /// <inheritdoc/>
        public override void Load(IServiceProvider provider)
        {
            visualizer = new MultidimensionalArrayVisualizer()
            {
                PaletteSelectedIndex = 1,
                RenderMethodSelectedIndex = 1
            };
            
            visualizer.Load(provider);

            var capacityLabel = new ToolStripLabel
            {
                Text = "Capacity:",
                AutoSize = true
            };
            var capacityValue = new ToolStripLabel
            {
                Text = Capacity.ToString(),
                AutoSize = true
            };

            visualizer.Plot.StatusStrip.Items.AddRange(new ToolStripItem[] {
                capacityLabel,
                capacityValue
            });

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
            Posterior2D posterior = (Posterior2D)value;
            if (posterior == null)
            {
                return;
            }

            var data = posterior.Data;
            // var argMax = posterior.ArgMax;

            // while (argMaxVals.Count >= Capacity)
            // {
            //     argMaxVals.RemoveAt(0);
            // }

            // argMaxVals.Add(argMax);
            // lineSeries.Points.Clear();
            // var count = argMaxVals.Count;

            // for (int i = 0; i < count; i++)
            // {
            //     lineSeries.Points.Add(new DataPoint(argMaxVals[i][0], argMaxVals[i][1]));
            // }
            
            visualizer.Show(data);
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
