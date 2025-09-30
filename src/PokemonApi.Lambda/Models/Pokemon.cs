using System.Text.Json.Serialization;

namespace PokemonBuscador.Cliente.Models;

// Clase principal para el Pokémon
public class Pokemon
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sprites")]
    public PokemonSprites Sprites { get; set; }

    [JsonPropertyName("types")]
    public List<PokemonTypeInfo> Types { get; set; }
}

// Clases para las propiedades anidadas
public class PokemonSprites
{
    [JsonPropertyName("front_default")]
    public string FrontDefault { get; set; }
}

public class PokemonTypeInfo
{
    [JsonPropertyName("type")]
    public PokemonType Type { get; set; }
}

public class PokemonType
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}