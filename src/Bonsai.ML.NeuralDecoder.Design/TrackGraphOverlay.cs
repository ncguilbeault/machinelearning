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

[assembly: TypeVisualizer(typeof(Bonsai.ML.NeuralDecoder.Design.TrackGraphOverlay),
    Target = typeof(MashupSource<Bonsai.ML.NeuralDecoder.Design.Posterior2DVisualizer, MultidimensionalArrayVisualizer>))]

namespace Bonsai.ML.NeuralDecoder.Design
{
    /// <summary>
    /// Class that overlays the true 
    /// </summary>
    public class TrackGraphOverlay : DialogTypeVisualizer
    {
        private Posterior2DVisualizer visualizer;
        internal LineSeries lineSeries;
        internal ScatterSeries scatterSeries;
        private HeatMapSeriesOxyPlotBase plot;

        /// <inheritdoc/>
        public override void Load(IServiceProvider provider)
        {
            var service = provider.GetService(typeof(MashupVisualizer));
            visualizer = (Posterior2DVisualizer)service;
            plot = visualizer.Plot;

            lineSeries = new LineSeries()
            {
                Title = "Track Graph Edges",
                Color = OxyColors.Red,
                StrokeThickness = 2
            };

            var colorAxis = new LinearColorAxis()
            {
                IsAxisVisible = false,
                Key = "TrackGraphColorAxis"
            };

            scatterSeries = new ScatterSeries()
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 10,
                MarkerFill = OxyColors.Red,
                Title = "Track Graph Nodes",
                ColorAxisKey = "TrackGraphColorAxis"
            };

            plot.Model.Series.Add(scatterSeries);
            plot.Model.Series.Add(lineSeries);
            plot.Model.Axes.Add(colorAxis);
        }

        /// <inheritdoc/>
        public override void Show(object value)
        {
            var nodes = (double[,])value;

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                var position = new DataPoint(nodes[i, 0], nodes[i, 1]);
                scatterSeries.Points.Add(new ScatterPoint(position.X, position.Y, value: 1));
                lineSeries.Points.Add(new DataPoint(position.X, position.Y));
            }
        }

        /// <inheritdoc/>
        public override void Unload()
        {
            scatterSeries.Points.Clear();
            lineSeries.Points.Clear();
        }
    }
}