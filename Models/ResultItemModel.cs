using System;
using System.Collections.Generic;

namespace KeyValueTypesComprasion.Models
{
    public class ResultItemModel
    {
        public string TypeName { get; set; }
        public double AddTime { get; set; }
        public double TryAddTime { get; set; }
        public double ContainsTime { get; set; }
        public double FindIndexTime { get; set; }
        public double TryGetValueTime { get; set; }
        public double RemoveTime { get; set; }

    }
}