using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, object?> ToDictionary(this object? values)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        if (values != null)
        {
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(values))
            {
                if (propertyDescriptor is null) continue;

                object? obj = propertyDescriptor.GetValue(values);
                dict.Add(propertyDescriptor.Name, obj);
            }
        }

        return dict;
    }
}
