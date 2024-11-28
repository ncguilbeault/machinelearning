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
    /// <summary>
    /// The data in 1D.
    /// </summary>
    public double[] Data1D { get; set; }

    /// <summary>
    /// The data in 2D.
    /// </summary>
    [XmlIgnore]
    public double[,] Data2D { get; set; }

    /// <summary>
    /// The argmax in 1D.
    /// </summary>
    public int ArgMax1D { get; set; }

    /// <summary>
    /// The argmax in 2D.
    /// </summary>
    public int[] ArgMax2D { get; set; }

    /// <summary>
    /// An optional mapping of the data to a value range.
    /// </summary>
    public double[] ValueRange1D { get; set; }

    /// <summary>
    /// The value centers.
    /// </summary>
    public double[] ValueCenters1D { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Posterior"/> class.
    /// </summary>
    public Posterior()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Posterior"/> class.
    /// </summary>
    /// <param name="data1D"></param>
    /// <param name="data2D"></param>
    /// <param name="argMax1D"></param>
    /// <param name="valueRange1D"></param>
    /// <param name="argMax2D"></param>
    public Posterior(double[] data1D, 
        double[,] data2D, 
        int argMax1D, 
        double[] valueRange1D = null,
        int[] argMax2D = null
    )
    {
        Data1D = data1D;
        Data2D = data2D;
        ArgMax1D = argMax1D;
        ValueRange1D = valueRange1D ?? Enumerable.Range(0, data1D.Length).Select(i => (double)i).ToArray();
        var step = (valueRange1D[valueRange1D.Length-1] - valueRange1D[0]) / data1D.Length;
        ValueCenters1D = Enumerable.Range(0, data1D.Length).Select(i => i * step).ToArray();
        ArgMax2D = argMax2D;
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
            var posterior2D = value[2];
            var data1D = (double[])PythonHelper.ConvertPythonObjectToCSharp(posterior);
            var argMax1D = Array.IndexOf(data1D, data1D.Max());
            var valueRange1D = (double[])PythonHelper.ConvertPythonObjectToCSharp(valueCenters);
            var data2D = (double[,])PythonHelper.ConvertPythonObjectToCSharp(posterior2D);
            var argMax2D = GetArgMax2D(data2D);
            return new Posterior(
                data1D,
                data2D,
                argMax1D,
                valueRange1D,
                argMax2D
            );
        });
    }

    private int[] GetArgMax2D(double[,] data2D)
    {
        var argMax2D = new int[2];
        var max = double.MinValue;
        for (int i = 0; i < data2D.GetLength(0); i++)
        {
            for (int j = 0; j < data2D.GetLength(1); j++)
            {
                if (data2D[i, j] > max)
                {
                    max = data2D[i, j];
                    argMax2D[0] = i;
                    argMax2D[1] = j;
                }
            }
        }
        return argMax2D;
    }
}