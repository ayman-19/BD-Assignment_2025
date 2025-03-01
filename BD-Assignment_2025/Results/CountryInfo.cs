using System.Text.Json.Serialization;

namespace BD_Assignment_2025.Result
{
    public record CountryInfo(
        [property: JsonPropertyName("ip")] string IP,
        [property: JsonPropertyName("country_code2")] string Code,
        [property: JsonPropertyName("country_name")] string Name,
        [property: JsonPropertyName("isp")] string ISP
    );
}
