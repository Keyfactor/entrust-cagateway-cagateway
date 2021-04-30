namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Profile
    {
        public string name { get; set; }
        public string id { get; set; }
        public string[] protocols { get; set; }
        public Subjectvariablerequirement[] subjectVariableRequirements { get; set; }
        public Subjectaltnamerequirement[] subjectAltNameRequirements { get; set; }
        public Requestedproperty[] requestedProperties { get; set; }
        public Properties properties { get; set; }
    }
}

    
