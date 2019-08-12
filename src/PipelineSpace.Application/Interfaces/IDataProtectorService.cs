using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces
{
    public interface IDataProtectorService
    {
        string Protect(string plainText);
        string Unprotect(string protectedData);
    }
}
