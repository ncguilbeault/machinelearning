using System.ComponentModel;
using System;
using System.Reactive.Linq;
using System.Xml.Serialization;
using System.Linq;
using Python.Runtime;
using Bonsai.ML.Python;

namespace Bonsai.ML.NeuralDecoder;

/// <summary>
/// Transforms the input sequence of Python objects into a sequence of <see cref="Posterior"/> instances.
/// </summary>
[Combinator]
[Description("Transforms the input sequence of Python objects into a sequence of Posterior instances.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class Posterior
{
    private double[] data;
    /// <summary>
    /// The data.
    /// </summary>
    public double[] Data => data;

    private int argMax;
    /// <summary>
    /// The argmax.
    /// </summary>
    public int ArgMax => argMax;

    private double[] valueRange;
    /// <summary>
    /// An optional mapping of the data to a value range.
    /// </summary>
    public double[] ValueRange => valueRange;

    private double[] valueCenters;
    /// <summary>
    /// The value centers.
    /// </summary>
    public double[] ValueCenters => valueCenters;

    /// <summary>
    /// Initializes a new instance of the <see cref="Posterior"/> class.
    /// </summary>
    public Posterior()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Posterior"/> class.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="argMax"></param>
    /// <param name="valueRange"></param>
    public Posterior(double[] data,
        int argMax, 
        double[] valueRange = null
    )
    {
        this.data = data;
        this.argMax = argMax;
        this.valueRange = valueRange ?? Enumerable.Range(0, data.Length).Select(i => (double)i).ToArray();
        var step = (valueRange[valueRange.Length-1] - valueRange[0]) / data.Length;
        valueCenters = Enumerable.Range(0, data.Length).Select(i => i * step).ToArray();
    }

    /// <summary>
    /// Transforms the input sequence of Python objects into a sequence of <see cref="Posterior"/> instances.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public IObservable<Posterior> Process(IObservable<PyObject> source)
    {
        return source.Select(value => {
            var posterior = value[0];
            var valueCenters = value[1];
            var data = (double[])PythonHelper.ConvertPythonObjectToCSharp(posterior);
            var argMax = Array.IndexOf(data, data.Max());
            var valueRange = (double[])PythonHelper.ConvertPythonObjectToCSharp(valueCenters);
            return new Posterior(
                data,
                argMax,
                valueRange
            );
        });
    }
}