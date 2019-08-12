using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PipelineSpace.Infra.CrossCutting.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this System.Enum value)
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var descriptionAttribute =
                enumMember == null
                    ? default(DescriptionAttribute)
                    : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return
                descriptionAttribute == null
                    ? value.ToString()
                    : descriptionAttribute.Description;
        }
    }
}
