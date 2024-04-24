using System;
using System.Collections.Generic;
using System.Reflection;
using Bonsai.ML.LinearDynamicalSystems.Kinematics;

namespace Bonsai.ML.LinearDynamicalSystems
{
    public class LinearDynamicalSystemsHelper
    {
        /// <summary>
        /// Gets the names of the state components defined in the kinematic component class
        /// </summary>
        public static List<string> GetStateComponents()
        {
            List<string> stateComponents = new List<string>();

            foreach (PropertyInfo property in typeof(KinematicComponent).GetProperties())
            {
                if (property.PropertyType == typeof(StateComponent))
                {
                    stateComponents.Add(property.Name);
                }
            }
            return stateComponents;
        }

        /// <summary>
        /// Gets the names of the kinematic components defined in the kinematic state class
        /// </summary>
        public static List<string> GetKinematicComponents()
        {
            List<string> kinematicComponents = new List<string>();

            foreach (PropertyInfo property in typeof(KinematicState).GetProperties())
            {
                if (property.PropertyType == typeof(KinematicComponent))
                {
                    kinematicComponents.Add(property.Name);
                }
            }
            return kinematicComponents;
        }
    }
}

