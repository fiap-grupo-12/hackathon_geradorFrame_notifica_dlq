using System.Text.Json.Serialization;

namespace hackaton_geradorFrame_notifica_dlq.Domain.Model
{
    public class S3MessageEvent
    {
        public Record[] Records { get; set; }
    }

    public class Record
    {
        public S3 s3 { get; set; }
    }

    public class S3
    {
        [JsonPropertyName("object")]
        public Object _object { get; set; }
    }

    public class Object
    {
        public string key { get; set; }
    }

}
