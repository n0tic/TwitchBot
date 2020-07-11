using System;

namespace TwitchBot
{
    [System.Serializable]
    public struct HungryData {
        public DateTime nextTime;
        public int timesHungry;
        public int timesHungryTotal;

        public DateTime nextUpload;
    }
}
