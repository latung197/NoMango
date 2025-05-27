using Worker.Application.Interface;

namespace Core.WorkerService
{
    /// <summary>
    /// Scan folder line 3
    /// </summary>
    public class WorkerLine3 : BackgroundService
    {
        private readonly IWorkerService _service;

        public WorkerLine3(IWorkerService service)
        {
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _service.ScanFolderLine3();
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}
