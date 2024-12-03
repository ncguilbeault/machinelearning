using System;
using System.ComponentModel;
using System.Reactive.Linq;
using static TorchSharp.torch;

namespace Bonsai.ML.Torch
{
    [Combinator]
    [Description("")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class Clone
    {
        /// <summary>
        /// Returns an empty tensor with the given data type and size.
        /// </summary>
        public IObservable<Tensor> Process(IObservable<Tensor> source)
        {
            return source.Select(tensor => tensor.clone());
        }
    }
}