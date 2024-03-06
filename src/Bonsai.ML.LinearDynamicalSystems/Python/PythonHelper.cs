using Python.Runtime;
using System;
using System.Collections.Generic;

namespace Bonsai.ML.LinearDynamicalSystems.Python
{
    public class PythonHelper
    {
        public static object GetPythonAttribute(PyObject pyObject, string attributeName)
        {
            using (var attr = pyObject.GetAttr(attributeName))
            {
                return ConvertPythonObjectToCSharp(attr);
            }
        }

        public static T GetPythonAttribute<T>(PyObject pyObject, string attributeName)
        {
            using (var attr = pyObject.GetAttr(attributeName))
            {
                return (T)attr.AsManagedObject(typeof(T));
            }
        }

        public static object ConvertPythonObjectToCSharp(PyObject pyObject)
        {
            if (PyInt.IsIntType(pyObject))
            {
                return pyObject.As<int>();
            }
            else if (PyFloat.IsFloatType(pyObject))
            {
                return pyObject.As<double>();
            }
            else if (PyString.IsStringType(pyObject))
            {
                return pyObject.As<string>();
            }

            else if (PyList.IsListType(pyObject))
            {
                var pyList = new PyList(pyObject);
                var resultList = new List<object>();
                foreach (PyObject item in pyList)
                    resultList.Add(ConvertPythonObjectToCSharp(item));
                return resultList;
            }

            else if (PyDict.IsDictType(pyObject))
            {
                var pyDict = new PyDict(pyObject);
                var resultDict = new Dictionary<object, object>();
                foreach (PyObject key in pyDict.Keys())
                {
                    var value = pyDict[key];
                    resultDict.Add(ConvertPythonObjectToCSharp(key), ConvertPythonObjectToCSharp(value));
                }
                return resultDict;
            }

            else if (NumpyHelper.IsNumPyArray(pyObject))
            {
                NumpyHelper numpyHelper = NumpyHelper.Instance;
                return NumpyHelper.PyObjectToArray(pyObject);
            }

            throw new InvalidOperationException("Unable to convert python data type to C#. Allowed data types include: integer, float, string, list, dictionary, and numpy arrays");
        }
    }
}