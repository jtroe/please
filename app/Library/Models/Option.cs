using System;
using System.Text.RegularExpressions;

namespace Library.Models
{
    public class Option<TTask>
    {
        public string Pattern { get; set; }
        public Action<TTask, Match> Action { get; set; }
    }
}