using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebay_Price_updater
{
    class Data
    {
        public Dictionary<string, string> Tokens { get; set; }
        public string DevID { get; set; }
        public string CertID { get; set; }
        public string AppID { get; set; }

        public static Data Load()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Data>(System.IO.File.ReadAllText("Data.db"));
        }
        public void save()
        {
            System.IO.File.WriteAllText("Data.db", Newtonsoft.Json.JsonConvert.SerializeObject(this));
        }
    }

}
