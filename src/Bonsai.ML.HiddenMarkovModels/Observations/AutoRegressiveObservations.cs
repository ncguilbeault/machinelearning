using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace Bonsai.ML.HiddenMarkovModels.Observations
{
    /// <summary>
    /// Represents an autoregressive observations model.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class AutoRegressiveObservations : ObservationsModel
    {

        private int lags = 1;

        /// <summary>
        /// The lags of the observations for each state.
        /// </summary>
        public int Lags { get => lags; set {lags = value; UpdateString(); } }

        /// <summary>
        /// The As of the observations for each state.
        /// </summary>
        public double[,,] As { get; private set; } = null;

        /// <summary>
        /// The bs of the observations for each state.
        /// </summary>
        public double[,] Bs { get; private set; } = null;

        /// <summary>
        /// The Vs of the observations for each state.
        /// </summary>
        public double[,,] Vs { get; private set; } = null;

        /// <summary>
        /// The square root sigmas of the observations for each state.
        /// </summary>
        public double[,,] SqrtSigmas { get; private set; } = null;

        /// <inheritdoc/>
        [JsonProperty]
        [JsonConverter(typeof(ObservationsTypeJsonConverter))]
        public override ObservationsType ObservationsType => ObservationsType.AutoRegressive;

        /// <inheritdoc/>
        [JsonProperty]
        public override object[] Params
        {
            get =>[ As, Bs, Vs, SqrtSigmas ];
            set
            {
                As = (double[,,])value[0];
                Bs = (double[,])value[1];
                Vs = (double[,,])value[2];
                SqrtSigmas = (double[,,])value[3];
                UpdateString();
            }
        }

        /// <inheritdoc/>
        [JsonProperty]
        public override Dictionary<string, object> Kwargs => new Dictionary<string, object>
        {
            ["lags"] = Lags,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoRegressiveObservations"/> class.
        /// </summary>
        public AutoRegressiveObservations (params object[] args)
        {
            if (args is not null && args.Length == 1)
            {
                Lags = args[0] switch
                {
                    int lags => lags,
                    long lags => Convert.ToInt32(lags),
                    _ => 1
                };
                UpdateString();
            }
        }
    }
}