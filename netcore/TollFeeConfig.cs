using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace congestion.calculator
{
    public class TollFeeConfig
    {
        public List<TollFeeRule> TollFees { get; set; }

        public static TollFeeConfig LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<TollFeeConfig>(json);
        }
    }

    public class TollFeeRule
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Fee { get; set; }

        public TimeSpan StartTimeSpan => TimeSpan.Parse(StartTime);
        public TimeSpan EndTimeSpan => TimeSpan.Parse(EndTime);
    }
}
