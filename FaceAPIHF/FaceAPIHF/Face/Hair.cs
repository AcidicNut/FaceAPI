using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FaceAPIHF.Face
{
    public class Hair
    {
        public double Bald { get; set; }

        private List<HairColor> _hairColor;
        [JsonProperty("hairColor", NullValueHandling = NullValueHandling.Ignore)]
        public List<HairColor> HairColor
        {
            get { return _hairColor; }
            set
            {
                if (value != null && value.Count > 0)
                {
                    // filter hair with highest score
                    var top = value.OrderByDescending(entry => entry.Confidence).Take(1).First();
                    _hairColor = new List<HairColor>();
                    _hairColor.Add(top);
                }
            }
        }
    }
}
