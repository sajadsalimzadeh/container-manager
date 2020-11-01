using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Chargoon.ContainerManagement.Service.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Test
{
    public class YamlTest
    {
        [Test]
        public void Yaml()
        {
            var dc = new DockerCompose();
            var web = new DockerComposeService();
            web.Image = "nodeapp";
            web.Ports = new List<string>()
            {
                "8090:8080"
            };
            web.Environment = new List<string>()
            {
                "DEBUG=1",
                "DATABASE_USERNAME=digah",
                "DATABASE_PASSWORD=lfdc82zo",
                "Image=Release",
                "COMPOSE_PROJECT_NAME=sajadsalimzadeh_test",
                "BUILDVERSION=2020.10.25",
            };
            dc.Services = new Dictionary<string, DockerComposeService>()
            {
                { "web", web }
            };
            var yamlHelper = new YamlHelper(dc);
            var str = yamlHelper.ToYaml();
        }
    }
}
