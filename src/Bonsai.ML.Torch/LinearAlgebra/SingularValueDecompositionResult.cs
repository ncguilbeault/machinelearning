using static TorchSharp.torch;

namespace Bonsai.ML.Torch.LinearAlgebra;

/// <summary>
/// Represents the result of a singular value decomposition.
/// </summary>
public readonly struct SingularValueDecompositionResult((
    Tensor u,
    Tensor s,
    Tensor vh
) result)
{
    /// <summary>
    /// The U tensor.
    /// </summary>
    public Tensor U => result.u;

    /// <summary>
    /// The singular values.
    /// </summary>
    public Tensor S => result.s;

    /// <summary>
    /// The Vh tensor.
    /// </summary>
    public Tensor Vh => result.vh;
}
