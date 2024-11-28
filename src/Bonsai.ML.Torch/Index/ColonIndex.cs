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
public class ColonIndex
{
    public IObservable<TensorIndex> Process()
    {
        return Observable.Return(TensorIndex.Colon);
    }

    public IObservable<TensorIndex> Process<T>(IObservable<T> source)
    {
        return source.Select((_) => TensorIndex.Colon);
    }
}