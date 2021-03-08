// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Lignator.Helpers
{
    public static class StringHelpers
    {
        public static string ReplaceFirst(this string template, string toReplace, string replacement)
        {
            int index = template.IndexOf(toReplace);
            return index < 0
                ? template
                : $"{template.Substring(0, index)}{replacement}{template.Substring(index + toReplace.Length)}";
        }
    }
}