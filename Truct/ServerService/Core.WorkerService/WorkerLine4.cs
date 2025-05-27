using Worker.Application.Interface;

namespace Core.WorkerService
{
    /// <summary>
    /// Scan folder line 4
    /// </summary>
    public class WorkerLine4 : BackgroundService
    {
        private readonly IWorkerService _service;

        public WorkerLine4(IWorkerService service)
        {
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _service.ScanFolderLine4();
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}
