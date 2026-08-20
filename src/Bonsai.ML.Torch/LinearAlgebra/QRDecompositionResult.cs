using static TorchSharp.torch;

namespace Bonsai.ML.Torch.LinearAlgebra;

/// <summary>
/// Represents the result of a QR decomposition.
/// </summary>
/// <param name="result"></param>
public readonly struct QRDecompositionResult((Tensor Q, Tensor R) result)
{
    /// <summary>
    /// The orthogonal matrix Q.
    /// </summary>
    public Tensor Q => result.Q;

    /// <summary>
    /// The upper triangular matrix R.
    /// </summary>
    public Tensor R => result.R;
}
