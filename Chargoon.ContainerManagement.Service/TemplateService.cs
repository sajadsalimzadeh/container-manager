using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Dtos.Templates;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Chargoon.ContainerManagement.Service
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository templateRepository;
		private readonly IInstanceRepository instanceRepository;
		private readonly ITemplateCommandRepository templateCommandRepository;

        public TemplateService(
            ITemplateRepository templateRepository,
            IInstanceRepository instanceRepository,
            ITemplateCommandRepository templateCommandRepository)
        {
            this.templateRepository = templateRepository;
			this.instanceRepository = instanceRepository;
			this.templateCommandRepository = templateCommandRepository;
        }

        private void ValidateDockerCompose(string dockerCompose)
        {
            if (string.IsNullOrEmpty(dockerCompose)) throw new Exception("docker compose can not be null");
            var deserializer = new DeserializerBuilder().Build();
            deserializer.Deserialize<DockerCompose>(dockerCompose);
        }

        private string ReplaceTime(string input)
        {
            input = input.Replace("{yyyy}", DateTime.Now.ToString("yyyy"));
            input = input.Replace("{MM}", DateTime.Now.ToString("MM"));
            input = input.Replace("{dd}", DateTime.Now.ToString("dd"));
            input = input.Replace("{HH}", DateTime.Now.ToString("HH"));
            input = input.Replace("{mm}", DateTime.Now.ToString("mm"));
            input = input.Replace("{ss}", DateTime.Now.ToString("ss"));
            return input;
        }

        public IEnumerable<TemplateGetDto> GetAll()
        {
            return templateRepository.GetAll().ToDto();
        }

        public TemplateGetDto Get(int id)
        {
            return templateRepository.Get(id).ToDto();
        }

        public TemplateGetDto Add(TemplateAddDto dto)
        {
            var model = dto.ToDataModel();
            ValidateDockerCompose(model.DockerCompose);
            return templateRepository.Insert(model).ToDto();
        }

        public TemplateGetDto DupplicateFrom(int templateId)
        {
            var template = templateRepository.Get(templateId);
            if (template == null) throw new Exception("Template not found");
            template.IsActive = true;
            template.InsertCron = null;
            template.Name = ReplaceTime(template.Name);
            template.Environments = ReplaceTime(template.Environments);
            template.DockerCompose = ReplaceTime(template.DockerCompose);
            template = templateRepository.Insert(template);
            if(template.InsertLifeTime.HasValue && template.InsertLifeTime.Value > 0)
			{
                template.ExpireTime = DateTime.Now.AddDays(template.InsertLifeTime.Value);
			}
            var commands = new List<TemplateCommand>();
            foreach (var templateCommand in templateCommandRepository.GetAllByTemplateId(templateId))
            {
                templateCommand.TemplateId = template.Id;
                templateCommandRepository.Insert(templateCommand);
                commands.Add(templateCommand);
            }
            template.Commands = commands;
            return template.ToDto();
        }

        public TemplateGetDto Change(int id, TemplateChangeDto dto)
        {
            var model = templateRepository.Get(id);
            if (model == null) throw new NullReferenceException("Template not found");
            model.ToDataModel(dto);
            model.Id = id;
            ValidateDockerCompose(model.DockerCompose);
            return templateRepository.Update(model).ToDto();
        }

        public TemplateGetDto Remove(int id)
        {
            foreach (var command in templateCommandRepository.GetAllByTemplateId(id))
            {
                templateCommandRepository.Delete(command.Id);
            }
			foreach (var instance in instanceRepository.GetAllByTemplateId(id))
			{
                instance.TemplateId = null;
                instanceRepository.Update(instance);
			}
            return templateRepository.Delete(id).ToDto();
        }

        public void RemveExpired()
		{
            var templates = templateRepository.GetAllExpired();
			foreach (var template in templates)
			{
                Remove(template.Id);
			}
		}
    }
}
