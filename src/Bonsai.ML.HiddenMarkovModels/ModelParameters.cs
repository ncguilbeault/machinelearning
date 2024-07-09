using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Xml.Serialization;
using Python.Runtime;
using Bonsai.ML.HiddenMarkovModels.Observations;
using static Bonsai.ML.HiddenMarkovModels.Observations.ObservationsLookup;

namespace Bonsai.ML.HiddenMarkovModels
{
    [Combinator]
    [Description("")]
    [WorkflowElementCategory(ElementCategory.Source)]
    public class ModelParameters
    {

        private int numStates;
        private string numStatesStr = "";

        /// <summary>
        /// The number of states of the HMM model.
        /// </summary>
        [JsonProperty("num_states")]
        [Description("The number of discrete latent states of the HMM model")]
        [Category("InitialParameters")]
        public int NumStates
        {
            get => numStates;
            set
            {
                numStates = value;
                numStatesStr = numStates.ToString();
            }
        }


        private int dimensions;
        private string dimensionsStr = "";

        /// <summary>
        /// The dimensionality of the observations into the HMM model.
        /// </summary>
        [JsonProperty("dimensions")]
        [Description("The dimensionality of the observations into the HMM model")]
        [Category("InitialParameters")]
        public int Dimensions
        {
            get => dimensions;
            set
            {
                dimensions = value;
                dimensionsStr = dimensions.ToString();
            }
        }


        private ObservationsType observationsType;
        private string observationsTypeStr = "";

        /// <summary>
        /// The type of distribution that the HMM will use to model the emission of data observations.
        /// </summary>
        [JsonProperty("observation_type")]
        [JsonConverter(typeof(ObservationsTypeJsonConverter))]
        [Description("The type of distribution that the HMM will use to model the emission of data observations.")]
        [Category("InitialParameters")]
        public ObservationsType ObservationsType
        {
            get => observationsType;
            set
            {
                observationsType = value;
                observationsTypeStr = GetString(observationsType);
            }
        }


        private StateParameters stateParameters = null;
        private string stateParametersStr = "";

        /// <summary>
        /// The state parameters of the HMM model.
        /// </summary>
        [XmlIgnore]
        [Description("The state parameters of the HMM model.")]
        [Category("ModelState")]
        public StateParameters StateParameters
        {
            get => stateParameters;
            set
            {
                stateParameters = value;
                stateParametersStr = stateParameters == null ? "" : stateParameters.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelParameters"/> class.
        /// </summary>
        public ModelParameters()
        {
            NumStates = 2;
            Dimensions = 2;
            ObservationsType = ObservationsType.Gaussian;
        }

        public IObservable<ModelParameters> Process()
        {
            return Observable.Return(
                new ModelParameters()
                {
                    NumStates = NumStates,
                    Dimensions = Dimensions,
                    ObservationsType = ObservationsType,
                    StateParameters = StateParameters
                });
        }

        public IObservable<ModelParameters> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Select(source, item =>
            {
                return new ModelParameters()
                {
                    NumStates = NumStates,
                    Dimensions = Dimensions,
                    ObservationsType = ObservationsType,
                    StateParameters = StateParameters
                };
            });
        }

        public IObservable<ModelParameters> Process(IObservable<PyObject> source)
        {
            var sharedSource = source.Publish().RefCount();
            var stateParametersObservable = new StateParameters() { ObservationsType = ObservationsType }.Process(sharedSource);
            return sharedSource.Select(pyObject =>
            {
                var numStatesPyObj = pyObject.GetAttr<int>("num_states");
                var dimensionsPyObj = pyObject.GetAttr<int>("dimensions");
                var observationsTypeStrPyObj = pyObject.GetAttr<string>("observation_type");

                var observationsTypePyObj = GetFromString(observationsTypeStrPyObj);

                return new ModelParameters()
                {
                    NumStates = numStatesPyObj,
                    Dimensions = dimensionsPyObj,
                    ObservationsType = observationsTypePyObj,
                };
            }).Zip(stateParametersObservable, (modelParameters, stateParameters) =>
            {
                modelParameters.StateParameters = stateParameters;
                return modelParameters;
            });
        }

        public override string ToString()
        {
            return $"num_states={numStatesStr}, dimensions={dimensionsStr}, observation_type=\"{observationsTypeStr}\", {stateParametersStr}";
        }
    }
}
