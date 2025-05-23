namespace JOB_FINDER_API.Models.Services
{
    public interface ICvSnapshotService
    {
        Task<string> CaptureCvAsImageAsync(CV cv);
    }
}
