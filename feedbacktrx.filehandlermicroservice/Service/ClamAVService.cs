using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public enum ClamScanResult
    {
        Clean,
        Infected,
        Error
    }

    public class ClamAVService : IClamAVService
    {
        private readonly string _clamAVHost;
        private readonly int _clamAVPort;

        public ClamAVService(string clamAVHost, int clamAVPort)
        {
            _clamAVHost = clamAVHost;
            _clamAVPort = clamAVPort;
        }

        public async Task<ClamScanResult> ScanFileAsync(Stream fileStream)
        {
            using (var tcpClient = new TcpClient(AddressFamily.InterNetwork))
            {
                await tcpClient.ConnectAsync(_clamAVHost, _clamAVPort);

                using (var networkStream = tcpClient.GetStream())
                {
                    await SendCommandAsync(networkStream, "zINSTREAM");

                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await networkStream.WriteAsync(buffer, 0, bytesRead);
                    }

                    await networkStream.WriteAsync(Encoding.ASCII.GetBytes("\0"), 0, 1);
                    await networkStream.FlushAsync();

                    var response = await ReadResponseAsync(networkStream);
                    var scanResult = ParseScanResponse(response);

                    return scanResult;
                }
            }
        }

        private async Task SendCommandAsync(NetworkStream networkStream, string command)
        {
            byte[] commandBytes = Encoding.ASCII.GetBytes($"{command}\0");
            await networkStream.WriteAsync(commandBytes, 0, commandBytes.Length);
            await networkStream.FlushAsync();
        }

        private async Task<string> ReadResponseAsync(NetworkStream networkStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                do
                {
                    bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    await memoryStream.WriteAsync(buffer, 0, bytesRead);
                }
                while (networkStream.DataAvailable);

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(memoryStream))
                {
                    string response = await streamReader.ReadToEndAsync();
                    return response;
                }
            }
        }

        private ClamScanResult ParseScanResponse(string response)
        {
            if (response.StartsWith("OK"))
            {
                return ClamScanResult.Clean;
            }
            else if (response.StartsWith("FOUND"))
            {
                return ClamScanResult.Infected;
            }
            else
            {
                return ClamScanResult.Error;
            }
        }
    }
}
