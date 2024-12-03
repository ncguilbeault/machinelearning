using System;
using System.ComponentModel;
using System.Reactive.Linq;
using static TorchSharp.torch;

namespace Bonsai.ML.Torch
{
    [Combinator]
    [Description("")]
    [WorkflowElementCategory(ElementCategory.Source)]
    public class LoadTensor
    {
        public string TensorPath { get; set; }
        public IObservable<Tensor> Process()
        {
            return Observable.Return(Tensor.Load(TensorPath));
        }
    }
}