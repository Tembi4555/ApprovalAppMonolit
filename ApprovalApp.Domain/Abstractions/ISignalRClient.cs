using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Domain.Abstractions
{
    public interface ISignalRClient
    {
        public Task ReceiveMessage(string[] personsIds, string message);
    }
}
