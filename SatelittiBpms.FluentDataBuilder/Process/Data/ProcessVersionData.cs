using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ProcessVersionData : BaseData, IActivityParentData
    {
        public ContextBuilder ContextBuilder { get; set; }

        public long TenantId { get; set; }
        public bool NeedPublish { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionFlow { get; set; }
        public string DiagramContent { get; set; }
        public string FormContent { get; set; }
        public IList<int> RolesIds { get; set; }
        public IList<ActivityBaseData> Activities { get; set; }
        public int Version { get; set; }
        public int TaskSequance { get; set; }
        public IList<ProcessFieldData> Fields { get; set; }


        public IEnumerable<FieldBaseData> AllFields
        {
            get
            {
                foreach (var field in Fields)
                {
                    yield return field;
                }
                foreach (var field in AllActivities.OfType<ActivityUserData>().SelectMany(a => a.Fields))
                {
                    yield return field;
                }
            }
        }

        public IEnumerable<ActivityBaseData> AllActivities
        {
            get
            {
                foreach (var activity in GetAllActivities(Activities))
                {
                    yield return activity;
                }

            }
        }


        private IEnumerable<ActivityBaseData> GetAllActivities(IEnumerable<ActivityBaseData> activities)
        {
            foreach (var activity in activities)
            {
                yield return activity;
            }
            var children = activities.OfType<ExclusiveGatewayData>().SelectMany(g => g.Branchs.SelectMany(b => b.Activities));
            if (children.Any())
            {
                foreach (var activity in GetAllActivities(children))
                {
                    yield return activity;
                }
            }
        }

        public FieldBaseData FindFieldById(DataId id)
        {
            return AllFields.FirstOrDefault(f => f.Id.InternalId == id.InternalId);
        }
    }
}
