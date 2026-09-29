using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace CoffCli
{
    internal class QwenTtsServiceClient
    {
        private readonly HttpClient _httpClient;

        // Inyectamos HttpClient siguiendo las buenas prácticas de .NET Core
        public QwenTtsServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8080");
        }

        /// <summary>
        /// Envía el texto al servicio persistente y guarda el archivo WAV de inmediato.
        /// </summary>
        public async Task SaveSpeechToFileAsync(string text, string targetWavPath, CancellationToken cancellationToken = default)
        {
            // Estructura de payload estándar para los endpoints de audio de llama-server
            var payload = new
            {
                model = "Qwen3-TTS-12Hz-1.7B-Base",
                prompt = text,
                voice = "default",
                language = "es" // Forzamos fonemas en español
            };

            // Realizamos la petición POST al endpoint nativo del servidor
            using var response = await _httpClient.PostAsJsonAsync("/v1/audio/generations", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"El servicio TTS devolvió un error ({response.StatusCode}): {errorContent}");
            }

            // Asegurar que el directorio destino exista
            string? directory = Path.GetDirectoryName(targetWavPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Guardamos el flujo binario (Stream) directo al disco de manera asíncrona
            using var audioStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var fileStream = new FileStream(targetWavPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);

            await audioStream.CopyToAsync(fileStream, cancellationToken);
        }
    }
}
