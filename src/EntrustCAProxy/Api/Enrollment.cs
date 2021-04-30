namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Enrollment
    {
        public string id { get; set; }
        public string status { get; set; }
        public string body { get; set; }
        public Api.Properties properties { get; set; }
    }
}
