using System;
using System.ComponentModel;
using System.Reactive.Linq;
using static TorchSharp.torch;
using static TorchSharp.torch.linalg;

namespace Bonsai.ML.Torch.LinearAlgebra;

/// <summary>
/// Represents an operator that computes the solution to a triangular system of linear equations with a unique solution.
/// </summary>
[Combinator]
[Description("Computes the solution to a triangular system of linear equations with a unique solution.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class SolveTriangular
{
    /// <summary>
    /// Gets or sets whether the first matrix is upper triangular.
    /// </summary>
    [Description("Whether the first matrix is upper triangular.")]
    public bool Upper { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to solve the system with the first matrix on the left or right (AX = B or XA = B).
    /// </summary>
    [Description("Whether to solve the system with the first matrix on the left or right (AX = B or XA = B).")]
    public bool Left { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the first matrix has a unit diagonal, i.e., all diagonal elements are assumed to be 1.
    /// </summary>
    [Description("Whether the first matrix has a unit diagonal, i.e., all diagonal elements are assumed to be 1.")]
    public bool UnitDiagonal { get; set; }

    /// <summary>
    /// Computes the solution to a triangular system of linear equations for each pair of input tensors.
    /// </summary>
    public IObservable<Tensor> Process(IObservable<Tuple<Tensor, Tensor>> source)
    {
        return source.Select(value =>
        {
            return solve_triangular(value.Item1, value.Item2, upper: Upper, left: Left, unitriangular: UnitDiagonal);
        });
    }
}
