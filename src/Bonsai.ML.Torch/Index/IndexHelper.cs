using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using static TorchSharp.torch;

namespace Bonsai.ML.Torch
{
    /// <summary>
    /// Provides helper methods to parse tensor indexes.
    /// </summary>
    public static class IndexHelper
    {

        /// <summary>
        /// Parses the input string into an array of tensor indexes.
        /// </summary>
        /// <param name="input"></param>
        public static TensorIndex[] Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return [0];
            }

            var indexStrings = input.Split(',');
            var indices = new TensorIndex[indexStrings.Length];

            for (int i = 0; i < indexStrings.Length; i++)
            {
                var indexString = indexStrings[i].Trim();
                if (int.TryParse(indexString, out int intIndex))
                {
                    indices[i] = TensorIndex.Single(intIndex);
                }
                else if (indexString == ":")
                {
                    indices[i] = TensorIndex.Colon;
                }
                else if (indexString == "None")
                {
                    indices[i] = TensorIndex.None;
                }
                else if (indexString == "...")
                {
                    indices[i] = TensorIndex.Ellipsis;
                }
                else if (indexString.ToLower() == "false" || indexString.ToLower() == "true")
                {
                    indices[i] = TensorIndex.Bool(indexString.ToLower() == "true");
                }
                else if (indexString.Contains(":"))
                {
                    var rangeParts = indexString.Split(':');
                    rangeParts = rangeParts.ToArray();
                    var argsList = new List<long?>([null, null, null]);
                    try
                    {
                        for (int j = 0; j < rangeParts.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(rangeParts[j]))
                            {
                                argsList[j] = long.Parse(rangeParts[j]);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Invalid index format: {indexString}");
                    }
                    indices[i] = TensorIndex.Slice(argsList[0], argsList[1], argsList[2]);
                }
                else
                {
                    throw new Exception($"Invalid index format: {indexString}");
                }
            }
            return indices;
        }
    }
}