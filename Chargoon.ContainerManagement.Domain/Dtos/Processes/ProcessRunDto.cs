using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Processes
{
    public class ProcessRunDto
    {
        public DataReceivedEventHandler ErrorDataReceived { get; set; }
        public DataReceivedEventHandler OutputDataReceived { get; set; }
        
        public EventHandler Exited { get; set; }
    }
}
