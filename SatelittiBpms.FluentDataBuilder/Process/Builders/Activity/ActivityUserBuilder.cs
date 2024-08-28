using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity
{
    public class ActivityUserBuilder : ActivityBaseBuilder
    {
        public ActivityUserBuilder(ContextBuilder context, IBaseBuilder parent, DataId id) : base(context, parent, id)
        {
        }


        private UserTaskExecutorTypeEnum? _executorType;
        public ActivityUserBuilder ExecutorRequester()
        {
            _executorType = UserTaskExecutorTypeEnum.REQUESTER;
            return this;
        }
        public ActivityUserBuilder ExecutorPerson()
        {
            _executorType = UserTaskExecutorTypeEnum.PERSON;
            return this;
        }

        public ExclusiveGatewayBuilder ExclusiveGateway(DataId id = null)
        {
            return FindFirstParentOrThis<IActivityParent>().ExclusiveGateway(id);
        }

        public ActivityUserBuilder ExecutorRole()
        {
            _executorType = UserTaskExecutorTypeEnum.ROLE;
            return this;
        }

        readonly List<ButtonData> _buttons = new();
        public ActivityUserBuilder Button()
        {
            _buttons.Add(new ButtonData());
            return this;
        }

        internal readonly List<ActivityFieldBuilder> _activityFieldBuilders = new();
        
        public ActivityFieldBuilder Field(DataId fieldId = null)
        {
            var builder = new ActivityFieldBuilder(Context, this, fieldId);            
            _activityFieldBuilders.Add(builder);
            return builder;
        }
        
        public ActivityFieldBuilder Field(DataId fieldId, FieldTypeEnum? type = null)
        {
            var builder = new ActivityFieldBuilder(Context, this, fieldId);
            if (type != null) builder.Type(type.Value);
            _activityFieldBuilders.Add(builder);
            return builder;
        }

        internal new ActivityBaseData LastBuild => base.LastBuild as ActivityBaseData;
        internal override IData Build()
        {
            int? personId = null;
            int? executorId = null;
            var executarType = _executorType ?? faker.Random.Enum<UserTaskExecutorTypeEnum>();

            if (executarType == UserTaskExecutorTypeEnum.PERSON)
            {
                personId = faker.Random.ArrayElement(Context.Users.ToArray()).Id;
            }
            else if (executarType == UserTaskExecutorTypeEnum.ROLE)
            {
                executorId = faker.Random.ArrayElement(Context.Roles.ToArray()).Id;
            }

            if (_buttons.Count == 0)
            {
                var numberOfButtons = faker.Random.Number(1, 6);
                for (int i = 0; i < numberOfButtons; i++)
                {
                    Button();
                }
            }

            return new ActivityUserData
            {
                ActivityId = Id?.InternalId ?? ("Activity_" + faker.Random.AlphaNumeric(7)),
                ActivityName = faker.Name.JobArea(),
                ActivityType = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Fields = _activityFieldBuilders.Select(a => a.LastBuild).ToList(),
                ExecutorType = executarType,
                PersonId = personId,
                ExecutorId = executorId,
                Buttons = _buttons.Select(b => new ButtonData
                {
                    Description = faker.Random.Words(),
                }).ToList(),
            };
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            foreach (var item in _activityFieldBuilders)
            {
                yield return item;
            }
        }
    }
}