using System.Text.Json;
using CoffCli.Models;

namespace CoffCli
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {            
            try
            {
                var http = new HttpClient();
                var response = await http.GetAsync("https://wttr.in/San+Miguel:Buenos+Aires:Argentina?format=j1&lang=es");
                if(response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Error al obtener los datos del clima: {response.StatusCode}");
                    return 3;
                }
                Console.Error.WriteLine($"Status Code: {response.StatusCode}");
                Console.Error.WriteLine($"Content Type: {response.Content.Headers.ContentType}");
                Console.Error.WriteLine($"Content Length: {response.Content.Headers.ContentLength}");
                var jsonString = await response.Content.ReadAsStringAsync();                
                File.WriteAllText("/home/coff/temp_san_miguel.json", jsonString);

                WeatherData? data = JsonSerializer.Deserialize<WeatherData>(jsonString);

                var condition = data?.CurrentCondition?.FirstOrDefault();

                if (condition != null)
                {
                    // Extraer los valores requeridos
                    string t = condition.FeelsLikeC ?? 0.ToString();
                    string c = condition.LangEs?.FirstOrDefault()?.Value ?? "Desconocido";
                    string k = condition.WindspeedKmph ?? "0";
                    string v = condition.Winddir16Point ?? "Desconocido";
                    string u = condition.Humidity ?? "0";

                    if(!int.TryParse(t, out int temp))
                    {
                        Console.WriteLine("No se pudo obtener la temperatura actual: '{0}'.", t);
                        return 5;
                    }

                    var guardada = getActual();

                    if(guardada != -99 && guardada == temp)
                    {
                        Console.WriteLine("La temperatura actual es la misma que la anterior {0} {1}.", guardada, temp);
                        return 4;
                    }
                    

                    if(!setActual(temp))
                    {
                        Console.WriteLine("Error al guardar la temperatura actual.");
                        return 6;
                    }

                    var windDirection = v switch
                    {
                        "N" => "norte",
                        "NE" => "noreste",
                        "E" => "este",
                        "SE" => "sureste",
                        "S" => "sur",
                        "SW" => "suroeste",
                        "W" => "oeste",
                        "NW" => "noroeste",
                        _ => v
                    };

                    // Formatear e imprimir la frase solicitada
                    string frase = $"En San Miguel la temperatura es de {temp} grados centigrados, está {c}, el viento corre desde el {windDirection} a {k} kilometros por hora, la humedad es del {u} porciento.";
                    Console.WriteLine(frase);
                    return 0;
                }
                else
                {                    
                    Console.WriteLine("No se encontraron condiciones actuales del clima.");
                    return 1;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de comunicación con el servicio: ");
                Console.WriteLine(ex.ToString());
                return 2;
            }
        }

        private static int getActual()
        {
            try
            {                
                if(File.Exists("/home/coff/temp_san_miguel.txt"))
                {
                    var contenido = File.ReadAllText("/home/coff/temp_san_miguel.txt");
                    if(int.TryParse(contenido, out int temp))
                    {
                        return temp;
                    }
                }                                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer la temperatura actual: {ex.Message}");
            }            
            return -99;    
        }

        private static bool setActual(int temp)
        {
            try
            {
                File.WriteAllText("/home/coff/temp_san_miguel.txt", temp.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la temperatura actual: {ex.Message}");
            }
            return false;
        }
    }
}
