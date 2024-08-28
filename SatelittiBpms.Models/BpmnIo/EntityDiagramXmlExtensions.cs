using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.BpmnIo
{
    public static class EntityDiagramXmlExtensions
    {
        public static List<ActivityBase> GetPreviousTaskPossible(this ActivityBase activity)
        {
            var process = activity.Parent;

            var sequencesFlow = process.SequenceFlow.Where(s => activity.Incoming.Contains(s.Id));

            var activities = new List<ActivityBase>();

            activities.AddRange(process.ExclusiveGateway.Where(a => sequencesFlow.Any(s => s.SourceRef == a.Id)));
            activities.AddRange(process.EndEvent.Where(a => sequencesFlow.Any(s => s.SourceRef == a.Id)));
            activities.AddRange(process.StartEvent.Where(a => sequencesFlow.Any(s => s.SourceRef == a.Id)));
            activities.AddRange(process.UserTask.Where(a => sequencesFlow.Any(s => s.SourceRef == a.Id)));
            activities.AddRange(process.SendTask.Where(a => sequencesFlow.Any(s => s.SourceRef == a.Id)));
            activities.AddRange(process.SatelittiSigner.Where(a => sequencesFlow.Any(s => s.SourceRef == a.Id)));

            return activities;
        }

        public static List<ActivityBase> GetNextTaskPossible(this ActivityBase activity)
        {
            var process = activity.Parent;

            var sequencesFlow = process.SequenceFlow.Where(s => activity.Outgoing.Contains(s.Id));

            var activities = new List<ActivityBase>();

            activities.AddRange(process.ExclusiveGateway.Where(a => sequencesFlow.Any(s => s.TargetRef == a.Id)));
            activities.AddRange(process.EndEvent.Where(a => sequencesFlow.Any(s => s.TargetRef == a.Id)));
            activities.AddRange(process.StartEvent.Where(a => sequencesFlow.Any(s => s.TargetRef == a.Id)));
            activities.AddRange(process.UserTask.Where(a => sequencesFlow.Any(s => s.TargetRef == a.Id)));
            activities.AddRange(process.SendTask.Where(a => sequencesFlow.Any(s => s.TargetRef == a.Id)));
            activities.AddRange(process.SatelittiSigner.Where(a => sequencesFlow.Any(s => s.TargetRef == a.Id)));

            return activities;
        }
    }
}
