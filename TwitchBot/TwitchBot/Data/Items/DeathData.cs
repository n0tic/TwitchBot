using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    [System.Serializable]
    public struct DeathData
    {
        public DateTime addNextTime;
        public DateTime removeNextTime;
        public int deaths;
        public int deathsTotal;
        [NonSerialized] public int session_deaths;
        [NonSerialized] public int session_deathsTotal;

        public DateTime nextUpload;
    }
}
