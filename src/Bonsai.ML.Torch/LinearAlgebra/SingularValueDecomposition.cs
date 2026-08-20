using System;
using System.ComponentModel;
using System.Reactive.Linq;
using static TorchSharp.torch;

namespace Bonsai.ML.Torch.LinearAlgebra;

/// <summary>
/// Represents an operator that computes the singular value decomposition (SVD) of a matrix.
/// </summary>
[Combinator]
[Description("Computes the singular value decomposition (SVD) of a matrix.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class SingularValueDecomposition
{
    /// <summary>
    /// Gets or sets whether to compute the full or reduced SVD.
    /// </summary>
    [Description("Whether to compute the full or reduced SVD.")]
    public bool FullMatrices { get; set; } = false;

    /// <summary>
    /// Computes the singular value decomposition (SVD) of a matrix.
    /// </summary>
    public IObservable<SingularValueDecompositionResult> Process(IObservable<Tensor> source)
    {
        return source.Select(tensor => new SingularValueDecompositionResult(linalg.svd(tensor, fullMatrices: FullMatrices)));
    }
}
