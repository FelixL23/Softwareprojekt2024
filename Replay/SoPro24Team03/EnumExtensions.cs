using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SoPro24Team03
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumType)
        {
            string name = enumType.ToString();

            DisplayAttribute? displAttr =
                enumType.GetType()
                .GetMember(name)
                .First()
                .GetCustomAttribute<DisplayAttribute>();
            if (displAttr == null)
                return name;
            
            string? displayName = displAttr.GetName();
            if (String.IsNullOrEmpty(displayName))
                return name;

            return displayName;
        }
    }
}