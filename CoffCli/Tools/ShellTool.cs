using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CoffCli.Tools
{
    internal class ShellTool : AITool
    {

        override public string Description => "Ejecuta comandos";

        private static readonly IReadOnlyDictionary<string, object?> _additionalProperties = new Dictionary<string, object?>
        {
            { "comando", "linea de comando completo a ejecutar" },
        };

        /// <summary>Gets any additional properties associated with the tool.</summary>
        override public IReadOnlyDictionary<string, object?> AdditionalProperties => _additionalProperties;

        /// <inheritdoc/>
        public override string ToString() => Name;

        public string ExecuteCommand(string command)
        {
            try
            {
                var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var process = new Process { StartInfo = processInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    if (!string.IsNullOrEmpty(error))
                    {
                        return $"Error: {error}";
                    }
                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
