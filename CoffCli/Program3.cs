using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace CoffCli
{
    internal class Program3
    {
        public static async Task Main3(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Registrar el cliente HTTP optimizado vinculándolo a nuestro servicio
            builder.Services.AddHttpClient<QwenTtsServiceClient>();

            var app = builder.Build();

            // Ejemplo de uso obteniendo el servicio desde el contenedor de dependencias
            var ttsService = app.Services.GetRequiredService<QwenTtsServiceClient>();

            try
            {
                Console.WriteLine("Enviando texto al servicio persistente de Qwen3...");

                // Al estar el modelo ya cargado en la memoria del servicio, la respuesta comenzará al instante
                await ttsService.SaveSpeechToFileAsync(
                    "Hola, esta petición es instantánea porque el modelo quedó residente en memoria como servicio.",
                    @"D:\aca.wav"
                );

                Console.WriteLine("¡Audio recibido y guardado con éxito!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de comunicación con el servicio: {ex.Message}");
            }
        }
    }
}
