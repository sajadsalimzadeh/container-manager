using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Dtos.Templates;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository templateRepository;

        public TemplateService(ITemplateRepository templateRepository)
        {
            this.templateRepository = templateRepository;
        }

        public IEnumerable<TemplateGetDto> GetAll()
        {
            return templateRepository.GetAll().ToDto();
        }

        public TemplateGetDto Add(TemplateAddDto dto)
        {
            var template = templateRepository.Insert(dto.ToDataModel());
            return template.ToDto();
        }

        public TemplateGetDto Change(TemplateChangeDto dto)
        {
            var template = templateRepository.Update(dto.ToDataModel());
            return template.ToDto();
        }
    }
}
