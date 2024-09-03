using System;
using System.Collections.Generic;

namespace HappyCoaching.Helpers
{
    public class SetValuesOfObject
    {
        public static void SetValues(List<string> fieldNames, Type dynamicType, object dynamicObject, Dictionary<string, string> initialValues)
        {
            foreach (var field in fieldNames)
            {
                var property = dynamicType.GetProperty(field);
                if (property != null && initialValues.Count>0)
                {
                    property.SetValue(dynamicObject, initialValues[field]);
                }
            }
        }

    }
}