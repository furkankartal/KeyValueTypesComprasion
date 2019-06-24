using System;
using System.Collections.Generic;

namespace KeyValueTypesComprasion.Models
{
    public class ResultModel
    {
        public List<ResultItemModel> ResultItemList { get; set; }
        public int RequestCount { get; set; }
        public double ResponseTime { get; set; }

        public ResultModel()
        {
            ResultItemList = new List<ResultItemModel>();
        }
    }
}