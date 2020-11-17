using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
	public interface ICommandService
	{
		void Execute(string command, ProcessStartInfo psi = null, DataReceivedEventHandler handler = null);
	}
}
