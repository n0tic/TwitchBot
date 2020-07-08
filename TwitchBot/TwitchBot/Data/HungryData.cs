using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    [System.Serializable]
    public struct HungryData
    {
        public DateTime nextTime;
        public int timesHungry;
        public int timesHungryTotal;
    }
}
