using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity
{
    public class ActivitySendBuilder : ActivityBaseBuilder
    {
        public ActivitySendBuilder(ContextBuilder context, IBaseBuilder parent, DataId id) : base(context, parent, id)
        {
        }


        private SendTaskDestinataryTypeEnum? _destinataryType;
        public ActivitySendBuilder DestinataryRole()
        {
            _destinataryType = SendTaskDestinataryTypeEnum.ROLE;
            return this;
        }
        public ActivitySendBuilder DestinataryCustom()
        {
            _destinataryType = SendTaskDestinataryTypeEnum.CUSTOM;
            return this;
        }
        public ActivitySendBuilder DestinataryPerson()
        {
            _destinataryType = SendTaskDestinataryTypeEnum.PERSON;
            return this;
        }
        public ActivitySendBuilder DestinataryRequester()
        {
            _destinataryType = SendTaskDestinataryTypeEnum.REQUESTER;
            return this;
        }

        internal new ActivityBaseData LastBuild => base.LastBuild as ActivityBaseData;
        internal override IData Build()
        {
            int? destinataryId;
            var destinataryType = _destinataryType ?? faker.Random.Enum<SendTaskDestinataryTypeEnum>();

            if (destinataryType == SendTaskDestinataryTypeEnum.PERSON)
            {
                destinataryId = faker.Random.ArrayElement(Context.Users.ToArray()).Id;
            }
            else if (destinataryType == SendTaskDestinataryTypeEnum.ROLE)
            {
                destinataryId = faker.Random.ArrayElement(Context.Roles.ToArray()).Id;
            }
            else
            {
                destinataryId = null;
            }

            return new ActivitySendData
            {
                ActivityId = Id?.InternalId ?? ("Activity_" + faker.Random.AlphaNumeric(7)),
                ActivityName = faker.Name.JobArea(),
                ActivityType = WorkflowActivityTypeEnum.SEND_TASK_ACTIVITY,
                DestinataryType = destinataryType,
                CustomEmail = destinataryType != SendTaskDestinataryTypeEnum.CUSTOM ? null : faker.Internet.Email(),
                DestinataryId = destinataryId,
                Message = faker.Lorem.Paragraphs(),
                Name = faker.Random.Words(),
                TitleMessage = faker.Random.Words(),
            };
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            return new List<IBaseBuilder>();
        }
    }
}