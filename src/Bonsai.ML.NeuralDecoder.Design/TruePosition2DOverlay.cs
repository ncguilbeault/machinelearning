using Bonsai;
using Bonsai.Design;
using Bonsai.Design.Visualizers;
using System;
using System.Collections.Generic;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot;
using Bonsai.ML.Design;
using Bonsai.Vision.Design;
using OpenCV.Net;
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
        internal LineSeries lineSeries;
        internal ScatterSeries scatterSeries;
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
                Color = OxyColors.LimeGreen,
                StrokeThickness = 2
            };

            var colorAxis = new LinearColorAxis()
            {
                IsAxisVisible = false,
                Key = "TruePositionColorAxis"
            };

            scatterSeries = new ScatterSeries()
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 10,
                MarkerFill = OxyColors.LimeGreen,
                Title = "Current Position",
                ColorAxisKey = "TruePositionColorAxis"
            };

            plot.Model.Series.Add(scatterSeries);
            plot.Model.Series.Add(lineSeries);
            plot.Model.Axes.Add(colorAxis);

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
            dynamic position = value;

            dataCount++;
            lineSeries.Points.Add(new DataPoint(position.X, position.Y));
            scatterSeries.Points.Clear();
            scatterSeries.Points.Add(new ScatterPoint(position.X, position.Y, value: 1));

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