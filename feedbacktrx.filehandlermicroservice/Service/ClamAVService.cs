using nClam;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public class ClamAVService: IClamAVService
    {
        private readonly ClamClient _clamAVClient;

        public ClamAVService(string clamAVHost, int clamAVPort)
        {
            _clamAVClient = new ClamClient(clamAVHost, clamAVPort)
            {
                MaxStreamSize = 209715200
            };
        }

        public async Task<ClamScanResult> ScanFileAsync(Stream fileStream)
        {
            var scanResult = await _clamAVClient.SendAndScanFileAsync(fileStream);
            return scanResult;
        }
    }
}
