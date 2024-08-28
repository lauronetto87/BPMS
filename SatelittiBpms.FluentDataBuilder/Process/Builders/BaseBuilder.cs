using Bogus;
using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity;
using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ActivitySigner;
using SatelittiBpms.FluentDataBuilder.Process.Builders.Process;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders
{
    public abstract class BaseBuilder : IBaseBuilder
    {
        public BaseBuilder(ContextBuilder context, IBaseBuilder parent)
        {
            Context = context;
            _parent = parent;
        }

        private readonly IBaseBuilder _parent;

        internal ContextBuilder Context { get; set; }

        protected readonly Faker faker = new("pt_BR");

        internal abstract IData Build();

        private IData BuildProtected()
        {
            foreach (var item in GetChildren())
            {
                ((BaseBuilder)item).BuildProtected();
            }
            LastBuild = Build();
            foreach (var item in GetChildren())
            {
                ((BaseBuilder)item).LastBuild.Parent = LastBuild;
            }
            return LastBuild;
        }

        internal IData LastBuild;

        IData IBaseBuilder.LastBuild { get => LastBuild; set { LastBuild = value; } }
        public ProcessVersionData MakeProcess()
        {
            var processBuilder = FindFirstParentOrThis<ProcessBuilder>();
            var processVersionData = processBuilder.BuildProtected();
            processBuilder.AfterBuildProtected(processVersionData);
            return (ProcessVersionData)processVersionData;
        }

        private void AfterBuildProtected(IData buildResult)
        {
            foreach (var item in GetChildren())
            {
                ((BaseBuilder)item).AfterBuildProtected(item.LastBuild);
            }
            AfterBuild(buildResult);
        }

        protected virtual void AfterBuild(IData buildResult)
        {

        }

        protected abstract IEnumerable<IBaseBuilder> GetChildren();


        internal T FindFirstParentOrThis<T>() where T : IBaseBuilder
        {
            return this is T t ? t : ((BaseBuilder)_parent).FindFirstParentOrThis<T>();            
        }

    }
}
