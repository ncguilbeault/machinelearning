using System.ComponentModel;
using System;
using System.Reactive.Linq;
using System.Xml.Serialization;
using System.Linq;
using Python.Runtime;
using Bonsai.ML.Python;

namespace Bonsai.ML.NeuralDecoder;

/// <summary>
/// Transforms the input sequence of Python objects into a sequence of <see cref="Posterior2D"/> instances.
/// </summary>
[Combinator]
[Description("Transforms the input sequence of Python objects into a sequence of Posterior2D instances.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class Posterior2D
{
    private double[,] data;
    /// <summary>
    /// The data.
    /// </summary>
    public double[,] Data => data;

    private int[] argMax;
    /// <summary>
    /// The argmax.
    /// </summary>
    public int[] ArgMax => argMax;

    /// <summary>
    /// Initializes a new instance of the <see cref="Posterior2D"/> class.
    /// </summary>
    public Posterior2D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Posterior"/> class.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="argMax"></param>
    public Posterior2D(double[,] data,
        int[] argMax
    )
    {
        this.data = data;
        this.argMax = argMax;
    }

    /// <summary>
    /// Transforms the input sequence of Python objects into a sequence of <see cref="Posterior"/> instances.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public IObservable<Posterior2D> Process(IObservable<PyObject> source)
    {
        return source.Select(value => {
            var posterior = value;
            var data = (double[,])PythonHelper.ConvertPythonObjectToCSharp(posterior);
            var argMax = GetArgMax2D(data);
            return new Posterior2D(
                data,
                argMax
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