using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Dtos.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface ITemplateService
    {
        TemplateGetDto Add(TemplateAddDto dto);
        TemplateGetDto DupplicateFrom(int templateId);
        TemplateGetDto Change(int id, TemplateChangeDto dto);
        TemplateGetDto Get(int id);
        IEnumerable<TemplateGetDto> GetAll();
        TemplateGetDto Remove(int id);
		void RemveExpired();
	}
}
