using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Interfaces
{
    public interface ISlugService
    {
        string GetSlug(string input);
    }

    public class DefaultSlugService : ISlugService
    {
        public string GetSlug(string input)
        {
            /*removing accent*/
            input = input.Normalize(NormalizationForm.FormD);
            input = input.ToLower();
            var stringBuilder = new StringBuilder();

            foreach (var c in input)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            input = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            input = Regex.Replace(input, @"-", " "); // hyphens 
            input = Regex.Replace(input, @"[^a-z0-9\s-]", ""); // invalid chars           
            input = Regex.Replace(input, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            input = Regex.Replace(input, @"\s", "-"); // hyphens   

            return input;
        }
    }
}
