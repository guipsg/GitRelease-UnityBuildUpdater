using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static string token = "ghp_SEU_TOKEN_AQUI"; // coloque seu token aqui (de preferência usar arquivo ou variável de ambiente)
    static string owner = "MinhaOrg"; // nome da organização
    static string repo = "UnityApp";  // nome do repositório

    static string localVersionFile = "version.txt";
    static string appFolder = "App";
    static string appExecutable = "App/NomeDoSeuApp.exe";

    static async Task Main(string[] args)
    {
        Console.WriteLine("🔍 Verificando atualizações...");

        string localVersion = File.Exists(localVersionFile) ? File.ReadAllText(localVersionFile).Trim() : "0.0.0";
        string latestVersion = "";
        string zipUrl = "";
        string versionUrl = "";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("UnityUpdater", "1.0"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);

            // Get latest release
            var response = await client.GetAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            // Pega os arquivos da release
            var assets = doc.RootElement.GetProperty("assets");
            foreach (var asset in assets.EnumerateArray())
            {
                string name = asset.GetProperty("name").GetString();
                string url = asset.GetProperty("browser_download_url").GetString();

                if (name == "version.txt")
                    versionUrl = url;
                else if (name.EndsWith(".zip"))
                    zipUrl = url;
            }
        }

        if (string.IsNullOrEmpty(versionUrl))
        {
            Console.WriteLine("❌ version.txt não encontrado na última release.");
            return;
        }

        // Baixa a versão online
        string remoteVersion = "";
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("UnityUpdater", "1.0"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);

            remoteVersion = (await client.GetStringAsync(versionUrl)).Trim();
        }

        Console.WriteLine($"📦 Versão local: {localVersion}");
        Console.WriteLine($"☁️  Versão disponível: {remoteVersion}");

        if (remoteVersion != localVersion)
        {
            Console.WriteLine("⬇️ Atualizando...");

            string tempZip = "update.zip";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("UnityUpdater", "1.0"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);

                var zipData = await client.GetByteArrayAsync(zipUrl);
                File.WriteAllBytes(tempZip, zipData);
            }

            if (Directory.Exists(appFolder))
                Directory.Delete(appFolder, true);

            ZipFile.ExtractToDirectory(tempZip, appFolder);
            File.WriteAllText(localVersionFile, remoteVersion);
            File.Delete(tempZip);

            Console.WriteLine("✅ Atualização concluída!");
        }
        else
        {
            Console.WriteLine("✅ Aplicativo já está atualizado.");
        }

        // Rodar app Unity
        if (File.Exists(appExecutable))
        {
            Console.WriteLine("🚀 Iniciando aplicação...");
            Process.Start(appExecutable);
        }
        else
        {
            Console.WriteLine("❌ Executável não encontrado.");
        }
    }
}
