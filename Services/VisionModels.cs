using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAAD.Services
{
    public class VisionRequest
    {
        public List<RequestItem> requests { get; set; }
    }

    public class RequestItem
    {
        public Image image { get; set; }
        public List<Feature> features { get; set; }
    }

    public class Image
    {
        public string content { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public int maxResults { get; set; }
    }

    public class VisionResponse
    {
        public List<ResponseItem> responses { get; set; }
    }

    public class ResponseItem
    {
        public List<EntityAnnotation> labelAnnotations { get; set; }
    }

    public class EntityAnnotation
    {
        public string description { get; set; }
        public float score { get; set; }
    }
}
