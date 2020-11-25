using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Images;
using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Docker.DotNet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Mappings
{
    public static class ImageMapper
    {

        public static ImageGetDto ToDto(this Image model)
        {
            return new ImageGetDto()
            {
                Id = model.Id,
                Name = model.Name,
                BuildCron = model.BuildCron,
                BuildPath = model.BuildPath,
                LifeTime = model.LifeTime,
            };
        }

        public static IEnumerable<ImageGetDto> ToDto(this IEnumerable<Image> models)
        {
            return models.Select(x => x.ToDto());
        }

        public static Image ToDataModel(this ImageAddDto dto)
        {
            return new Image()
            {
                Name = dto.Name,
                BuildCron = dto.BuildCron,
                BuildPath = dto.BuildPath,
                LifeTime = dto.LifeTime,
            };
        }

        public static Image ToDataModel(this Image model, ImageChangeDto dto)
        {
            model.Name = dto.Name;
            model.BuildPath = dto.BuildPath;
            model.BuildCron = dto.BuildCron;
            model.LifeTime = dto.LifeTime;
            return model;
        }
    }
}
