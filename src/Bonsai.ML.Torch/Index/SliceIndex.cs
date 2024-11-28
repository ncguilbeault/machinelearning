using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using static TorchSharp.torch;
using System.Xml.Serialization;
using Bonsai.Expressions;
using System.Linq.Expressions;

namespace Bonsai.ML.Torch;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Source)]
public class SliceIndex
{    
    public long? Start { get; set; } = null;
    public long? End { get; set; } = null;
    public long? Step { get; set; } = null;

    public IObservable<TensorIndex> Process()
    {
        return Observable.Return(TensorIndex.Slice(Start, End, Step));
    }

    public IObservable<TensorIndex> Process<T>(IObservable<T> source)
    {
        return source.Select((_) => TensorIndex.Slice(Start, End, Step));
    }
}