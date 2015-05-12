using System.Xml.Serialization;
using ForestSolverPackages;

namespace Server
{
    [XmlRoot(Namespace = "http://localhost", IsNullable = false)]
    public class Config
    {
        public string filename;
        [XmlArrayAttribute("players")]
        public ConfigPoints[] points;
    }

    public class ConfigPoints
    {
        public int StartPointX;
        public int StartPointY;
        public int TargetX;
        public int TargetY;
    }
}
