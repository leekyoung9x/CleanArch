public class ReCaptchaResponse
{
    public bool Success { get; set; }
    public double Score { get; set; }
    public string Action { get; set; }
    public DateTime ChallengeTs { get; set; }
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
}