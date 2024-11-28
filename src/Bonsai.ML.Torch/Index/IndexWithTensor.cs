using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using static TorchSharp.torch;
using System.Xml.Serialization;
using Bonsai.Expressions;
using System.Linq.Expressions;
using TorchSharp;

namespace Bonsai.ML.Torch;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Source)]
public class IndexWithTensor
{   
    [XmlIgnore] 
    public Tensor Tensor { get; set; }

    public IObservable<TensorIndex> Process()
    {
        return Observable.Return(TensorIndex.Tensor(Tensor));
    }
    public IObservable<TensorIndex> Process<T>(IObservable<T> source)
    {
        return source.Select((_) => TensorIndex.Tensor(Tensor));
    }
}