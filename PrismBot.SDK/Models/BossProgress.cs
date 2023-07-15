namespace PrismBot.SDK.Models;
using System.Text.Json.Serialization;

public class BossProgress
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("response")]
    public BossStatus Response { get; set; }
}

public class BossStatus
{
    [JsonPropertyName("King Slime")]
    public bool KingSlime { get; set; }

    [JsonPropertyName("Eye of Cthulhu")]
    public bool EyeOfCthulhu { get; set; }

    [JsonPropertyName("Eater of Worlds / Brain of Cthulhu")]
    public bool EaterOfWorlds { get; set; }
    
    [JsonPropertyName("Eater of Worlds / Brain of Cthulhu 2")]
    public bool BrainOfCthulhu { get; set; }

    [JsonPropertyName("Queen Bee")]
    public bool QueenBee { get; set; }

    [JsonPropertyName("Skeletron")]
    public bool Skeletron { get; set; }

    [JsonPropertyName("Deerclops")]
    public bool Deerclops { get; set; }

    [JsonPropertyName("Wall of Flesh")]
    public bool WallOfFlesh { get; set; }

    [JsonPropertyName("Queen Slime")]
    public bool QueenSlime { get; set; }

    [JsonPropertyName("The Twins")]
    public bool TheTwins { get; set; }

    [JsonPropertyName("The Destroyer")]
    public bool TheDestroyer { get; set; }

    [JsonPropertyName("Skeletron Prime")]
    public bool SkeletronPrime { get; set; }

    [JsonPropertyName("Plantera")]
    public bool Plantera { get; set; }

    [JsonPropertyName("Golem")]
    public bool Golem { get; set; }

    [JsonPropertyName("Duke Fishron")]
    public bool DukeFishron { get; set; }

    [JsonPropertyName("Empress of Light")]
    public bool EmpressOfLight { get; set; }

    [JsonPropertyName("Lunatic Cultist")]
    public bool LunaticCultist { get; set; }

    [JsonPropertyName("Moon Lord")]
    public bool MoonLord { get; set; }

    [JsonPropertyName("Solar Pillar")]
    public bool SolarPillar { get; set; }

    [JsonPropertyName("Nebula Pillar")]
    public bool NebulaPillar { get; set; }

    [JsonPropertyName("Vortex Pillar")]
    public bool VortexPillar { get; set; }

    [JsonPropertyName("Stardust Pillar")]
    public bool StardustPillar { get; set; }
}
