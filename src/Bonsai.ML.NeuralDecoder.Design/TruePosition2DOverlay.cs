using Bonsai;
using Bonsai.Design;
using Bonsai.Design.Visualizers;
using System;
using System.Collections.Generic;
using OxyPlot.Series;
using OxyPlot;
using Bonsai.ML.Design;
using Bonsai.Vision.Design;
using System.Linq;

[assembly: TypeVisualizer(typeof(Bonsai.ML.NeuralDecoder.Design.TruePosition2DOverlay),
    Target = typeof(MashupSource<Bonsai.ML.NeuralDecoder.Design.Posterior2DVisualizer, PointVisualizer>))]

namespace Bonsai.ML.NeuralDecoder.Design
{
    /// <summary>
    /// Class that overlays the true 
    /// </summary>
    public class TruePosition2DOverlay : DialogTypeVisualizer
    {
        private Posterior2DVisualizer visualizer;
        private LineSeries lineSeries;
        private List<double[]> data = new();
        private string defaultYAxisTitle;
        private HeatMapSeriesOxyPlotBase plot;
        private int dataCount;

        /// <inheritdoc/>
        public override void Load(IServiceProvider provider)
        {
            var service = provider.GetService(typeof(MashupVisualizer));
            visualizer = (Posterior2DVisualizer)service;
            plot = visualizer.Plot;

            lineSeries = new LineSeries()
            {
                Title = "True Position",
                Color = OxyColors.LimeGreen
            };

            plot.Model.Series.Add(lineSeries);

#pragma warning disable CS0618 // Type or member is obsolete
            plot.Model.Updated += (sender, e) =>
            {
                defaultYAxisTitle = plot.Model.DefaultYAxis.Title;
                plot.Model.DefaultYAxis.Title = "Position";
            };
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <inheritdoc/>
        public override void Show(object value)
        {
            var position = (double[])value;
            if (position.Any(double.IsNaN))
            {
                return;
            }

            dataCount++;
            lineSeries.Points.Add(new DataPoint(position[0], position[1]));

            while (dataCount > visualizer.Capacity)
            {
                lineSeries.Points.RemoveAt(0);
                dataCount--;
            }
        }

        /// <inheritdoc/>
        public override void Unload()
        {
            plot.Model.DefaultYAxis.Title = defaultYAxisTitle;
        }
    }
}