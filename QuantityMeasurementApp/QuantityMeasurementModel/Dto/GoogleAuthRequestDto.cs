namespace QuantityMeasurementModel.Dto
{
    /// <summary>
    /// UC19: The frontend sends Google's ID Token here after Google Sign-In succeeds.
    /// Backend verifies it with Google and issues our own JWT.
    /// </summary>
    public class GoogleAuthRequestDto
    {
        public string IdToken { get; set; } = string.Empty;
    }
}
