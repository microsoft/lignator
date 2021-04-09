// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Lignator.Models
{
    public class Options
    {
        public string Template { get; set;}
        public int Runs { get; set; } = 1;
        public string Output { get; set; } = "./logs";
        public int Logs { get; set; } = 1;
        public bool Infinite { get; set; } = false;
        public bool Clean { get; set;} = false;
        public string Extension { get; set; } = "log";
        public bool MultiLine { get; set; } = false;
        public string Head { get; set;}
        public string Tail { get; set; }
        public string[] Variable { get; set; }
        public bool NoBanner { get; set; }
        public string TokenOpening { get; set; } = "${";
        public string TokenClosing { get; set; } = "}";
    }
}