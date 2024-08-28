using SatelittiBpms.FluentDataBuilder.Process.Data;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Process
{
    public class ProcessFieldBuilder : FieldBaseBuilder
    {
        public ProcessFieldBuilder(ContextBuilder context, ProcessBuilder parent, DataId fieldId) : base(context, parent, fieldId)
        {
            Parent = parent;
        }

        internal ProcessBuilder Parent { get; private set; }

        public ProcessFieldBuilder Field(DataId fieldId = null)
        {
            return FindFirstParentOrThis<ProcessBuilder>().Field(fieldId);
        }
        public ProcessFieldBuilder Id(DataId id)
        {
            FieldId = id;
            return this;
        }

        public ProcessFieldBuilder Type(Models.Enums.FieldTypeEnum type)
        {
            _type = type;
            return this;
        }


        internal new ProcessFieldData LastBuild => base.LastBuild as ProcessFieldData;
        internal override IData Build()
        {
            return new ProcessFieldData()
            {
                Id = FieldId ?? new DataId(),
                Label = faker.Name.FullName(),
                Type = _type ?? faker.Random.Enum<Models.Enums.FieldTypeEnum>(),
            };
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            return new List<IBaseBuilder>();
        }
    }
}
