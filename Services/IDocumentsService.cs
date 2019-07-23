using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public interface IDocumentsService
    {
        Task<(string, string)> SaveArticleImage(IFormFile model);

        Task<List<Tuple<string, string>>> SaveArticleImages(List<IFormFile> models);

    }

    public class DocumentsService : IDocumentsService
    {
        private readonly IHostingEnvironment _env;
        public DocumentsService(IHostingEnvironment env)
        {
            _env = env;
        }

        public async Task<(string, string)> SaveArticleImage(IFormFile model)
        {
            // marrim ex.
            var extention = GetExtention(model.ContentType);
            // nese nuk pranohet -> return
            if (string.IsNullOrEmpty(extention))
                return ("", "");
            // perndryshe: generate name,
            var imageName = $"{Guid.NewGuid().ToString()}{extention}";
            // save file
            var path = Path.Combine(_env.WebRootPath, "documents", imageName);
            using (var fs = new FileStream(path, FileMode.Create))
            {
                await model.CopyToAsync(fs);
            }

            // return name
            return (extention, imageName);
        }

        public async Task<List<Tuple<string, string>>> SaveArticleImages(List<IFormFile> models)
        {
            var extention = "";
            var imageName = "";

            var ls = new List<Tuple<string, string>>();
            foreach (var item in models)
            {
                extention = GetExtention(item.ContentType);
                if (string.IsNullOrEmpty(extention))
                    return ls;
                imageName = $"{Guid.NewGuid().ToString()}{extention}";
                var path = Path.Combine(_env.WebRootPath, "documents", imageName);
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await item.CopyToAsync(fs);
                }
                var mb = new Tuple<string, string>(extention, imageName);
                ls.Add(mb);
            }


            return ls;
        }

        private string GetExtention(string mimeType)
        {
            var ex = "";
            switch (mimeType)
            {
                case "image/png":
                    ex = ".png";
                    break;
                case "image/jpeg":
                    ex = ".jpeg";
                    break;
                case "image/jpg":
                    ex = ".jpg";
                    break;
                default:
                    break;
            }

            return ex;
        }
    }
}

