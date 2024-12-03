using System;
using System.ComponentModel;
using System.Reactive.Linq;
using static TorchSharp.torch;

namespace Bonsai.ML.Torch
{
    [Combinator]
    [Description("")]
    [WorkflowElementCategory(ElementCategory.Sink)]
    public class SaveTensor
    {
        public string TensorPath { get; set; }
        public IObservable<Tensor> Process(IObservable<Tensor> source)
        {
            return source.Do(tensor => tensor.save(TensorPath));
        }
    }
}