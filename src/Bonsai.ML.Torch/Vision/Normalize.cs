using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using static TorchSharp.torch;
using static TorchSharp.torchvision;
using System.Xml.Serialization;

namespace Bonsai.ML.Torch.Vision
{
    [Combinator]
    [Description("")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class Normalize
    {
        public double[] Means { get; set; } = [ 0.1307 ];
        public double[] StdDevs { get; set; } = [ 0.3081 ];

        public IObservable<Tensor> Process(IObservable<Tensor> source)
        {
            return source.Select(tensor => {
                var inputTransform = transforms.Normalize(Means, StdDevs, tensor.dtype, tensor.device);
                return inputTransform.call(tensor);
            });
        }
    }
}