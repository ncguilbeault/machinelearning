using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Xml.Serialization;
using static TorchSharp.torch;

namespace Bonsai.ML.Pca.Torch;

/// <summary>
/// Fits the PCA model to the input data and transforms it.
/// </summary>
[Combinator]
[Description("Fits a PCA model and transforms the input data.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class FitAndTransform
{
    private static void FitModelAndTransformData(PcaBaseModel model, Tensor data)
    {
        model.FitAndTransform(data);
    }

    /// <summary>
    /// Fits the PCA model to the input data and transforms it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public IObservable<Tuple<T, Tensor>> Process<T>(IObservable<Tuple<T, Tensor>> source) where T : PcaBaseModel
    {
        return source.Do((value) =>
        {
            FitModelAndTransformData(value.Item1, value.Item2);
        });
    }

    /// <summary>
    /// Fits the PCA model to the input data and transforms it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public IObservable<Tuple<Tensor, T>> Process<T>(IObservable<Tuple<Tensor, T>> source) where T : PcaBaseModel
    {
        return source.Do((value) =>
        {
            FitModelAndTransformData(value.Item2, value.Item1);
        });
    }
}
