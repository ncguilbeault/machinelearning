﻿using System;
using System.ComponentModel;
using System.Reactive.Linq;
using static TorchSharp.torch;
using TorchSharp;

namespace Bonsai.ML.Torch
{
    /// <summary>
    /// Creates a 1-D tensor of values within a given range given the start, end, and step.
    /// </summary>
    [Combinator]
    [Description("Creates a 1-D tensor of values within a given range given the start, end, and step.")]
    [WorkflowElementCategory(ElementCategory.Source)]
    public class Arange
    {
        /// <summary>
        /// The start of the range.
        /// </summary>
        [Description("The start of the range.")]
        public int Start { get; set; } = 0;

        /// <summary>
        /// The end of the range.
        /// </summary>
        [Description("The end of the range.")]
        public int End { get; set; } = 10;

        /// <summary>
        /// The step size between values.
        /// </summary>
        [Description("The step size between values.")]
        public int Step { get; set; } = 1;

        /// <summary>
        /// Generates an observable sequence of 1-D tensors created with the <see cref="arange(Scalar, Scalar, Scalar, ScalarType?, Device?, bool)"/> function.
        /// </summary>
        public IObservable<Tensor> Process()
        {
            return Observable.Defer(() => Observable.Return(arange(Start, End, Step)));
        }
    }
}
