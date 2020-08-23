using NPOI.OpenXmlFormats.Vml.Office;
using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class ClockifyResponsePageInfo
    {
        public string _id { get; set; }

        public int totalTime { get; set; }

        public int entriesCount { get; set; }
    }
}
