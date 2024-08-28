using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.Models.Mapper;

namespace SatelittiBpms.Models.Extensions
{
    public static class ModelsDependencyInjectionExtension
    {
        public static void AddModelsDependencyInjection(
          this IServiceCollection services)
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new DefaultProfile()));
            IMapper mapper = new AutoMapper.Mapper(configuration);
            services.AddSingleton(mapper);
        }
    }
}
