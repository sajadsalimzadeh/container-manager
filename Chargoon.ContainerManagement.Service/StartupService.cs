﻿using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Chargoon.ContainerManagement.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Service
{
    public class StartupService : IStartupService
    {
        private readonly ITemplateService templateService;

        public StartupService(ITemplateService templateService)
        {
            this.templateService = templateService;
        }
        public void Run()
        {

        }

        public void RunAsync()
        {
            var dc = new DockerCompose();
            dc.Version = "3.8";
            dc.Services = new Dictionary<string, DockerComposeService>()
            {
                {
                    "nodeApp1", new DockerComposeService()
                    {

                    }
                }
            };

        }
    }
}
