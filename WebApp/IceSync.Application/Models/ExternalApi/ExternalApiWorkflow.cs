using System.Text.Json.Serialization;

namespace IceSync.Application.Models.ExternalApi
{
    public class ExternalApiWorkflow
    {
        [JsonPropertyName("id")]
        public int WorkflowId { get; set; }
        [JsonPropertyName("name")]
        public string WorkflowName { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("multiExecBehavior")]
        public string MultiExecBehavior { get; set; }
    }
}
