using System;
using System.ComponentModel;
using System.Reactive.Linq;
using static TorchSharp.torch;

namespace Bonsai.ML.Torch
{
    [Combinator]
    [Description("Takes the mean of the tensor along the specified dimensions.")]
    [WorkflowElementCategory(ElementCategory.Transform)]

    public class Mean
    {
        public long[] Dimensions { get; set; }

        /// <summary>
        /// Generates an observable sequence of 1-D tensors created with the <see cref="linspace"/> function.
        /// </summary>
        /// <returns></returns>
        public IObservable<Tensor> Process(IObservable<Tensor> source)
        {
            return source.Select(input => input.mean(Dimensions));
        }
    }
}