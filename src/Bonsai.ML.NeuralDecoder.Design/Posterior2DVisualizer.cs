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

[assembly: TypeVisualizer(typeof(Bonsai.ML.NeuralDecoder.Design.Posterior2DVisualizer),
    Target = typeof(Bonsai.ML.NeuralDecoder.Posterior))]

namespace Bonsai.ML.NeuralDecoder.Design
{
    /// <summary>
    /// Provides a mashup visualizer to display the posterior of the neural decoder.
    /// </summary>    
    public class Posterior2DVisualizer : MashupVisualizer
    {
        private MultidimensionalArrayVisualizer visualizer = null;
        private LineSeries lineSeries;
        private List<int[]> argMaxVals = new();

        /// <summary>
        /// Gets the underlying heatmap plot.
        /// </summary>
        public HeatMapSeriesOxyPlotBase Plot => visualizer?.Plot;

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
            Console.WriteLine("here1");
            visualizer = new MultidimensionalArrayVisualizer()
            {
                PaletteSelectedIndex = 1,
                RenderMethodSelectedIndex = 1
            };
            Console.WriteLine("here2");
            
            visualizer.Load(provider);
            Console.WriteLine("here3");
            lineSeries = new LineSeries()
            {
                Title = "Maximum Posterior",
                Color = OxyColors.SkyBlue
            };
            visualizer.Plot.Model.Series.Add(lineSeries);
            Console.WriteLine("here4");
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

            var data = posterior.Data2D;
            var argMax = posterior.ArgMax2D;

            while (argMaxVals.Count >= Capacity)
            {
                argMaxVals.RemoveAt(0);
            }

            argMaxVals.Add(argMax);
            lineSeries.Points.Clear();
            var count = argMaxVals.Count;

            for (int i = 0; i < count; i++)
            {
                lineSeries.Points.Add(new DataPoint(argMaxVals[i][0], argMaxVals[i][1]));
            }
            
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

            var mergedSource = source.SelectMany(xs => xs
                .Do(value => Show(value)));

            var mashupSourceStreams = Observable.Merge(
                MashupSources.Select(mashupSource =>
                    mashupSource.Source.Output.SelectMany(xs => xs
                        .Do(value => mashupSource.Visualizer.Show(value)))));

            return Observable.Merge(mergedSource, mashupSourceStreams)
                .ObserveOn(visualizerControl);

        }
    }
}
