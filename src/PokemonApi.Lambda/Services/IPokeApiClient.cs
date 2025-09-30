using Refit;
using PokemonBuscador.Cliente.Models;

namespace PokemonBuscador.Cliente.Services;

public interface IPokeApiClient
{
    [Get("/pokemon/{name}")]
    Task<Pokemon> GetPokemonAsync(string name);
}