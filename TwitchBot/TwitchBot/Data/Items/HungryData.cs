using System;

namespace TwitchBot
{
    [System.Serializable]
    public struct HungryData {
        public DateTime addNextTime;
        public DateTime removeNextTime;
        public int timesHungry;
        public int timesHungryTotal;
        [NonSerialized] public int session_timesHungry;
        [NonSerialized] public int session_timesHungryTotal;

        public DateTime nextUpload;
    }
}
