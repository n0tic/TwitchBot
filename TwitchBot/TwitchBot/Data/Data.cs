using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchBot
{
    [System.Serializable]
    public class Data
    {
        public HungryData hungry = new HungryData();
        public DeathData deaths = new DeathData();

        public Data() => Start();

        void Start() {
            hungry = new HungryData();
            deaths = new DeathData();
        }
    }
}
