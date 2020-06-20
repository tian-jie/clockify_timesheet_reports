using System.Collections.Generic;

namespace Innocellence.Activity.Model
{
    public class PollingSortingEntity
    {
        public int PollingId { get; set; }

        public string OptionName { get; set; }

        public int Sort { get; set; }
    }

    public class SortViewModel
    {
        public string Name { get; set; }

        public IList<int?> Sorts { get; set; }

        public int? SortSum { get; set; }
    }
}
