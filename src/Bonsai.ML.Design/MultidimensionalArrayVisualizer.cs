using System;
using System.Windows.Forms;
using Bonsai;
using Bonsai.Design;

[assembly: TypeVisualizer(typeof(Bonsai.ML.Design.MultidimensionalArrayVisualizer),
    Target = typeof(double[,]))]

namespace Bonsai.ML.Design
{
    /// <summary>
    /// Provides a type visualizer to display multi dimensional array data as a heatmap.
    /// </summary>
    public class MultidimensionalArrayVisualizer : DialogTypeVisualizer
    {
        /// <summary>
        /// Gets or sets the selected index of the color palette to use.
        /// </summary>
        public int PaletteSelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the selected index of the render method to use.
        /// </summary>
        public int RenderMethodSelectedIndex { get; set; }

        private HeatMapSeriesOxyPlotBase plot;

        /// <summary>
        /// Gets the underlying heatmap plot.
        /// </summary>
        public HeatMapSeriesOxyPlotBase Plot => plot;

        /// <inheritdoc/>
        public override void Load(IServiceProvider provider)
        {
            plot = new HeatMapSeriesOxyPlotBase(PaletteSelectedIndex, RenderMethodSelectedIndex)
            {
                Dock = DockStyle.Fill,
            };

            plot.PaletteComboBoxValueChanged += PaletteIndexChanged;
            plot.RenderMethodComboBoxValueChanged += RenderMethodIndexChanged;

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(plot);
            }
        }

        /// <inheritdoc/>
        public override void Show(object value)
        {
            var mdarray = (double[,])value;
            var shape = new int[] {mdarray.GetLength(0), mdarray.GetLength(1)};

            plot.UpdateHeatMapSeries(
                -0.5,
                shape[0] - 0.5,
                -0.5,
                shape[1] - 0.5,
                mdarray
            );

            plot.UpdatePlot();
        }

        /// <inheritdoc/>
        public override void Unload()
        {
            if (!plot.IsDisposed)
            {
                plot.Dispose();
            }
        }

        private void PaletteIndexChanged(object sender, EventArgs e)
        {
            PaletteSelectedIndex = plot.PaletteComboBox.SelectedIndex;
        }
        
        private void RenderMethodIndexChanged(object sender, EventArgs e)
        {
            RenderMethodSelectedIndex = plot.RenderMethodComboBox.SelectedIndex;
        }
    }
}
