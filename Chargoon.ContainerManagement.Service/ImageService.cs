using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.Dtos.Images;
using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.Service
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository imageRepository;
        private readonly ILoggerService logger;

        public ImageService(
            IImageRepository imageRepository,
            ILoggerService logger)
        {
            this.imageRepository = imageRepository;
            this.logger = logger;
        }

        public IEnumerable<ImageGetDto> GetAll()
        {
            return imageRepository.GetAll().ToDto();
        }

        public ImageGetDto Get(int id)
        {
            var image = imageRepository.Get(id);
            if (image == null) throw new Exception("Image not found");
            return image.ToDto();
        }

        private DirectoryInfo GetDirInfo(string basePath, string name)
        {
            var dirPath = Path.Combine(basePath, name);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return new DirectoryInfo(dirPath);
        }

        private DirectoryInfo GetLogDirInfo(string basePath)
        {
            return GetDirInfo(basePath, "logs");
        }

        private DirectoryInfo GetScriptDirInfo(string basePath)
        {
            return GetDirInfo(basePath, "scripts");
        }

        public IEnumerable<ImageBuildLogDto> GetAllBuildLog(int imageId)
        {
            var image = Get(imageId);
            var logDirInfo = GetLogDirInfo(image.BuildPath);
            return logDirInfo.GetDirectories().Select(x => new ImageBuildLogDto()
            {
                BuildName = x.Name,
                Scripts = x.GetFiles().Select(x => x.Name)
            });
        }

        public Stream GetBuildLog(int imageId, string buildname, string filename)
        {
            var image = Get(imageId);
            var logDirInfo = GetLogDirInfo(image.BuildPath);
            return new FileStream(Path.Combine(logDirInfo.FullName, buildname, filename), FileMode.Open, FileAccess.Read);
        }

        public ImageGetDto Add(ImageAddDto dto)
        {
            return imageRepository.Insert(dto.ToDataModel()).ToDto();
        }

        public ImageGetDto Change(int id, ImageChangeDto dto)
        {
            var model = imageRepository.Get(id);
            if (model == null) throw new NullReferenceException("Image not found");
            model.ToDataModel(dto);
            model.Id = id;
            return imageRepository.Update(model).ToDto();
        }

        public ImageGetDto Remove(int id)
        {
            return imageRepository.Delete(id).ToDto();
        }

        public void Build(int imageId)
        {
            try
            {
                var image = Get(imageId);

                var logDirInfo = GetLogDirInfo(image.BuildPath);
                var logDirPath = Path.Combine(logDirInfo.FullName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

                if (!Directory.Exists(logDirPath)) Directory.CreateDirectory(logDirPath);

                var scriptDirInfo = GetScriptDirInfo(image.BuildPath);

                foreach (var scriptFile in scriptDirInfo.GetFiles())
                {
                    if (!scriptFile.Name.EndsWith(".bat")) continue;
                    var logFilePath = Path.Combine(logDirPath, scriptFile.Name.Substring(0, scriptFile.Name.Length - 4) + ".log");
                    if (File.Exists(scriptFile.FullName))
                    {
                        var psi = new ProcessStartInfo(scriptFile.FullName)
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            RedirectStandardInput = true,
                            CreateNoWindow = true,
                            WorkingDirectory = image.BuildPath,
                        };
                        var process = new Process();
                        process.StartInfo = psi;
                        process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                File.AppendAllLines(logFilePath, new List<string> { e.Data });
                            }
                        };
                        process.Start();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
        }

        public Task BuidAsync(int ImageId)
        {
            return Task.Run(() =>
            {
                Build(ImageId);
            });
        }
    }
}
