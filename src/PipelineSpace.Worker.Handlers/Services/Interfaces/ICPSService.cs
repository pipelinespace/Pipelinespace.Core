using PipelineSpace.Worker.Handlers.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services.Interfaces
{
    public interface ICPSService
    {
        Task DeleteService(string name, CPSAuthModel auth);
    }
}
