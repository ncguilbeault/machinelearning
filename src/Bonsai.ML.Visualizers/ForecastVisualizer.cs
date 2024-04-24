using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Bonsai;
using Bonsai.Design;
using Bonsai.ML.Visualizers;
// using Bonsai.ML.LinearDynamicalSystems;
using Bonsai.ML.LinearDynamicalSystems.Kinematics;
using System.Drawing;
using System.Reactive;
using OxyPlot;
using OxyPlot.Series;

[assembly: TypeVisualizer(typeof(ForecastVisualizer), Target = typeof(Forecast))]

namespace Bonsai.ML.Visualizers
{
    /// <summary>
    /// Provides a type visualizer to display the state components of a Kalman Filter kinematics model.
    /// </summary>
    public class ForecastVisualizer : DialogTypeVisualizer
    {

        private DateTime? _startTime;
        private TimeSpan UpdateFrequency = TimeSpan.FromSeconds(1/60);
        private DateTime? lastUpdate = null;

        private TimeSeriesOxyPlotBase Plot;

        private LineSeries lineSeries;

        private AreaSeries areaSeries;

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

            lineSeries = Plot.AddNewLineSeries("Forecast Mean", color: OxyColors.Yellow);
            areaSeries = Plot.AddNewAreaSeries("Forecast Variance", color: OxyColors.Yellow);

            // Plot.ResetLineSeries(lineSeries);
            // Plot.ResetAreaSeries(areaSeries);
            Plot.ResetAxes();

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(Plot);
            }
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

            Plot.ResetLineSeries(lineSeries);
            Plot.ResetAreaSeries(areaSeries);

            Forecast forecast = (Forecast)value;
            List<ForecastResult> forecastResults = forecast.ForecastResults;
            DateTime forecastTime = time;

            for (int i = 0; i < forecastResults.Count; i++)
            {
                var forecastResult = forecastResults[i];
                var kinematicState = forecastResult.KinematicState;

                forecastTime = time + forecastResult.Timestep;
                double mean = kinematicState.Position.X.Mean;
                double variance = kinematicState.Position.X.Variance;

                Plot.AddToLineSeries(
                    lineSeries: lineSeries,
                    time: forecastTime,
                    mean: mean
                );

                Plot.AddToAreaSeries(
                    areaSeries: areaSeries,
                    time: forecastTime,
                    mean: mean,
                    variance: variance
                );
            }

            Plot.SetAxes(minTime: forecastTime.AddSeconds(-Capacity), maxTime: forecastTime);

            if (lastUpdate is null || time - lastUpdate > UpdateFrequency)
            {
                lastUpdate = time;
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
    }
}