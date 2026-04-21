using System.Text.Json.Serialization;

namespace Domain.Models.Administrator.DTO
{
    public class RecaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public string ChallengeTs { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("score")]
        public float Score { get; set; }  // For reCAPTCHA v3

        [JsonPropertyName("action")]
        public string Action { get; set; } // For reCAPTCHA v3

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }  // Capture errors
    }

}
