using SatelittiBpms.VersionNormalization.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SatelittiBpms.VersionNormalization.Services
{
    public class ExecuteNormalizations : IExecuteNormalizations
    {
        private protected IServiceProvider _serviceProvider;
        IVersionNormalizationService _versionNormalizationService;
        string nameSpace = "SatelittiBpms.VersionNormalization.Normalizations";

        public ExecuteNormalizations(IVersionNormalizationService versionNormalizationService, IServiceProvider services)
        {
            _versionNormalizationService = versionNormalizationService;
            _serviceProvider = services;
        }

        public async Task Execute()
        {
            var normalizations = GetClassessInNamespace();

            var normalizationsExecuteds = this._versionNormalizationService.ListAll().Select(x => x.Normalization);

            normalizations.RemoveAll(x => normalizationsExecuteds.Contains(x.Name));

            foreach (var normalization in normalizations)
            {
                var newClass = VersionNormalizationInstance(normalization);

                await newClass.Execute();

                this._versionNormalizationService.AddNormalization(normalization.Name);
            }
        }

        protected virtual IVersionNormalization VersionNormalizationInstance(Type normalization)
        {
            var dependencies = new object[] { _serviceProvider };

            return (IVersionNormalization)Activator.CreateInstance(normalization, args: dependencies);
        }

        protected virtual List<Type> GetClassessInNamespace()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && t.IsPublic == true
                && t.MemberType == MemberTypes.TypeInfo
                ).OrderBy(x => x.Name).ToList();
        }
    }
}
