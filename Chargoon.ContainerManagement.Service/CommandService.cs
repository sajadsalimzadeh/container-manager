using Chargoon.ContainerManagement.Domain.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chargoon.ContainerManagement.Service
{
	public class CommandService : ICommandService
	{
		private readonly ILoggerService logger;

		public CommandService(ILoggerService logger)
		{
			this.logger = logger;
		}
		public void Execute(string command, ProcessStartInfo psi = null, DataReceivedEventHandler handler = null)
		{
			try
			{
				if (psi == null) psi = new ProcessStartInfo();
				logger.LogInformation("CommandService > Execute :", new { command });
				psi.FileName = "powershell.exe";
				psi.Arguments = $"-Command {command}";
				psi.UseShellExecute = false;
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardError = true;
				psi.RedirectStandardInput = true;
				psi.CreateNoWindow = true;
				var process = new Process();
				process.StartInfo = psi;
				var sb = new StringBuilder();
				if (handler == null)
				{
					process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
					{
						if (!string.IsNullOrEmpty(e.Data))
						{
							sb.AppendLine(e.Data);
						}
					};
					process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
					{
						if (!string.IsNullOrEmpty(e.Data))
						{
							sb.AppendLine(e.Data);
						}
					};
				} else
				{
					process.OutputDataReceived += handler;
					process.ErrorDataReceived += handler;
				}
				process.Start();
				process.BeginOutputReadLine();
				process.WaitForExit();
				logger.LogInformation($"CommandService > Execute : {nameof(Execute)}", new { Command = command, Output = sb.ToString() });
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
			}
		}
	}
}
