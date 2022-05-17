namespace NetExam.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class Suggestion
    {
        public int CapacityNeeded { get; }
        public string PreferedNeigborHood { get; }
        public IEnumerable<string> ResourcesNeeded { get; }
    }
}
