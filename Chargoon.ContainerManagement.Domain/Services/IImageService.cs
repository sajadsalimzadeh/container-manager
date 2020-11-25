using Chargoon.ContainerManagement.Domain.Dtos.Images;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IImageService
    {
        ImageGetDto Add(ImageAddDto dto);
        void Build(int ImageId);
        ImageGetDto Change(int id, ImageChangeDto dto);
        ImageGetDto Get(int id);
        IEnumerable<ImageGetDto> GetAll();
        Stream GetBuildLog(int imageId, string buildname, string filename);
        IEnumerable<ImageBuildLogDto> GetAllBuildLog(int imageId);
        ImageGetDto Remove(int id);
		void ClearBuildLogBefore(DateTime datetime);
		void RemoveExpired();
	}
}
