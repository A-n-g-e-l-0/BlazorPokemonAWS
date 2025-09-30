using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using PokemonApi.Lambda.Models; // Asegúrate que el namespace sea correcto
using PokemonApi.Lambda.Services; // Asegúrate que el namespace sea correcto
using Refit;
using System.Net;
using System.Text.Json;

// Ensamblador de Lambda que apunta al manejador de la función.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PokemonApi.Lambda;

public class Function
{
    private static readonly ServiceProvider _serviceProvider;

    // Usamos un constructor estático para configurar la inyección de dependencias una sola vez.
    static Function()
    {
        var services = new ServiceCollection();

        // Configuramos Refit para que apunte a la PokéAPI
        services.AddRefitClient<IPokeApiClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://pokeapi.co/api/v2"));

        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// El manejador de nuestra Lambda. Recibe una petición de API Gateway.
    /// </summary>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        // Extraemos el nombre del pokémon de la URL (ej: /pokemon/pikachu)
        if (!request.PathParameters.TryGetValue("name", out var pokemonName))
        {
            return CreateResponse(HttpStatusCode.BadRequest, "El nombre del Pokémon es requerido en la URL.");
        }

        context.Logger.LogInformation($"Buscando al Pokémon: {pokemonName}");

        try
        {
            // Obtenemos el cliente API del contenedor de servicios
            var pokeApiClient = _serviceProvider.GetRequiredService<IPokeApiClient>();

            // Llamamos a la PokéAPI
            var pokemon = await pokeApiClient.GetPokemonAsync(pokemonName.ToLower());

            return CreateResponse(HttpStatusCode.OK, JsonSerializer.Serialize(pokemon));
        }
        catch (ApiException ex)
        {
            context.Logger.LogError($"Error de la API de Pokémon: {ex.Message}");
            return CreateResponse(ex.StatusCode, $"No se encontró el Pokémon '{pokemonName}'.");
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error inesperado: {ex.Message}");
            return CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.");
        }
    }

    private APIGatewayProxyResponse CreateResponse(HttpStatusCode statusCode, string body)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = (int)statusCode,
            Body = body,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" } // CORS para permitir llamadas desde nuestra app
            }
        };
    }
}