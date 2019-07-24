using System;
using System.Collections.Generic;

namespace KeyValueTypesComprasion.Models
{
    public class DetailResultModel
    {
        public List<DetailResultItemModel> ResultItemList { get; set; }
        public int RequestCount { get; set; }
        public double ResponseTime { get; set; }
        public bool HasError { get; set; }

        public DetailResultModel()
        {
            ResultItemList = new List<DetailResultItemModel>();
        }
    }
}