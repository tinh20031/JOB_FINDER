
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Models.Services
{
    public interface ICvSnapshotService
    {
        Task<List<string>> CaptureCvAsImagesAsync(CV cv);
}}