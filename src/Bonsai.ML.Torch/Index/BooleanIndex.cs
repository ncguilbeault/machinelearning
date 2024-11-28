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
public class BooleanIndex
{
    public bool Value { get; set; } = false;

    public IObservable<TensorIndex> Process()
    {
        return Observable.Return(TensorIndex.Bool(Value));
    }
    
    public IObservable<TensorIndex> Process<T>(IObservable<T> source)
    {
        return source.Select((_) => TensorIndex.Bool(Value));
    }
}