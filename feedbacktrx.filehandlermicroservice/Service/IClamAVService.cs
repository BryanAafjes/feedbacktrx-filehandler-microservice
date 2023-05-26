using nClam;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public interface IClamAVService
    {
        public Task<ClamScanResult> ScanFileAsync(Stream fileStream);
    }
}
