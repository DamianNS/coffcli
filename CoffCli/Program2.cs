using CoffCli.Tools;
using ConsoleInk;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.Text;


namespace CoffCli
{
    internal class Program2
    {
        static async Task Main2(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<Program2>()
                .Build();

            string? model = config["ModelName"] ?? "gemma-4-E2B";
            string? key = config["OpenAIKey"] ?? "nada";

            var clientOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri("http://127.0.0.1:9931/v1"),
                
            };

            var apiCredentials = new ApiKeyCredential(key);

            IChatClient client = new ChatClientBuilder(
                new OpenAIClient(apiCredentials, clientOptions)
                .GetChatClient(model)
                .AsIChatClient())
                .UseFunctionInvocation()
                .Build();

            var shellTool = new ShellTool();
            var options = new ChatOptions
            {
                //MaxOutputTokens = 400,
                //Reasoning = new Microsoft.Extensions.AI.ReasoningOptions() { Effort = Microsoft.Extensions.AI.ReasoningEffort.High, Output = Microsoft.Extensions.AI.ReasoningOutput.Full },
                Tools = [
                    AIFunctionFactory.Create(shellTool.ExecuteCommand),
                    AIFunctionFactory.Create((string location, string unit) =>
                    {
                        // Here you would call a weather API
                        // to get the weather for the location.
                        return "Periods of rain or drizzle, 15 C";
                    },
                    "get_current_weather",
                    "Gets the current weather in a given location")
                ],
                ToolMode = ChatToolMode.Auto,
                Reasoning = new Microsoft.Extensions.AI.ReasoningOptions() { Effort = Microsoft.Extensions.AI.ReasoningEffort.High, Output = Microsoft.Extensions.AI.ReasoningOutput.Full }
            };

            var systemIntruction = new ChatMessage( ChatRole.System, """
                Sos mi agente personal muntifuncion. 
                Usa simpre que sea posible usar las herramientas que tengas disponibles cuando se solicite o la herramienta sirva para esa accion.
            """);

            List<ChatMessage> chatHistory =
            [
                systemIntruction
            ];

            // ConsoleInk
            Console.OutputEncoding = Encoding.UTF8;
            var output = Console.OpenStandardOutput();
            var textOut = new System.IO.StreamWriter(output);
            var md = new MarkdownConsoleWriter(textOut);

            //var enviarRespuesta = false;
            while (true)
            {
                //if(!enviarRespuesta)
                //{
                   
                    Console.Write("Tu: ");
                    string? userPrompt = Console.ReadLine();

                    if (userPrompt == "bye")
                    {
                        break;
                    }

                    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));
                //}

                //enviarRespuesta = false;


                Console.WriteLine("AI pienza... ");
                string response = "";
                try
                {
                    
                    var chunk = client.GetStreamingResponseAsync(chatHistory, options);
                    await foreach (var c in chunk)
                    {
                        //Console.Write(c.Text);
                        //md.Write(c);
                        //Console.Write(c);
                        // c.Contents.ToList().ForEach(c => Console.Write(c));
                        c.Contents.ToList().ForEach(c => md.Write(c));
                        response += c;
                        //if (c.FinishReason == ChatFinishReason.ToolCalls)
                            //enviarRespuesta = true;
                        
                    }                    
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nError en el Stream: {ex.Message}");
                    Console.ResetColor();
                }

                chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));

                Console.WriteLine();
                Console.WriteLine(" ***************** AI Response: ***************");
                md.WriteLine(response);
                Console.WriteLine();
            }

            //ChatResponse response = await client.GetResponseAsync("Genera un Hola mundo en C#",options);
            //Console.WriteLine(response.Text);

            Console.WriteLine("Bye!");
        }
    }
}
