using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Framework.Common.Core
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieves the <see cref="DisplayAttribute.Name" /> property on the <see cref="DisplayAttribute" />
        /// of the current enum value, or the enum's member name if the <see cref="DisplayAttribute" /> is not present.
        /// </summary>
        /// <param name="val">This enum member to get the name for.</param>
        /// <returns>The <see cref="DisplayAttribute.Name" /> property on the <see cref="DisplayAttribute" /> attribute, if present.</returns>
        public static string GetDisplayName(this Enum val)
        {
            return val.GetType()
                       .GetMember(val.ToString())
                       .FirstOrDefault()
                       ?.GetCustomAttribute<DisplayAttribute>(false)
                       ?.Description
                   ?? val.ToString();
        }

        public static string GetFullName(this Enum val)
        {
            return val.ToString().ToUnderscoreCase() + ":" + val.GetType()
                       .GetMember(val.ToString())
                       .FirstOrDefault()
                       ?.GetCustomAttribute<DisplayAttribute>(false)
                       ?.Description
                   ?? val.ToString();
        }

        public static object GetKeyValue(this Enum val)
        {
            var key = val.ToString().ToUnderscoreCase();
            var value = val.GetType()
                            .GetMember(val.ToString())
                            .FirstOrDefault()
                            ?.GetCustomAttribute<DisplayAttribute>(false)
                            ?.Description
                        ?? val.ToString();
            return new { key = key, value = value };
        }

        public static object GetNameKeyValue(this Enum val)
        {
            var key = val.ToString().ToUnderscoreCase();
            var value = val.GetType()
                            .GetMember(val.ToString())
                            .FirstOrDefault()
                            ?.GetCustomAttribute<DisplayForAttribute>(false)
                            ?.Name
                        ?? val.ToString();
            return new { key = key, value = value };
        }
    }
}
