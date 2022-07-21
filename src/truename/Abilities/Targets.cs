namespace truename.Abilities;

public class Targets
{
    public int Minimum { get; set; } = 1;
    public int Maximum { get; set; } = 1;

    // Empty = "Any" for now
    public string[] Types { get; set; } = Array.Empty<string>();

    public string[] Zones { get; set; } = new string[1] { "Battlefield" };
}