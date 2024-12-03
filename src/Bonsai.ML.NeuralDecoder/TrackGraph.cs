using System.ComponentModel;
using System;
using System.Reactive.Linq;
using System.Xml.Serialization;
using System.Linq;
using Python.Runtime;
using Bonsai.ML.Python;

namespace Bonsai.ML.NeuralDecoder;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class TrackGraph
{
    private double[,] nodes;
    public double[,] Nodes => nodes;

    public TrackGraph()
    {
    }

    public TrackGraph(double[,] nodes)
    {
        this.nodes = nodes;
    }

    public IObservable<TrackGraph> Process(IObservable<PyObject> source)
    {
        return source.Select(value => {
            var nodes = (double[,])value.GetArrayAttr("track_graph");
            return new TrackGraph(
                nodes
            );
        });
    }
}