using Bogus;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.BpmnIo;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SatelittiBpms.FluentDataBuilder.Process.DiagramXml
{
    public class DiagramXmlHelper
    {
        private static readonly Faker faker = new("pt_BR");


        public static string Generate(ProcessVersionData processVersion)
        {
            var definitions = GenerateDefinitions(processVersion);
            return Serializer(definitions);
        }

        private static Definitions GenerateDefinitions(ProcessVersionData processVersion)
        {
            var process = GenerateProcess(processVersion);

            return new Definitions
            {
                Id = "sample-diagram",
                TargetNamespace = "http://bpmn.io/schema/bpmn",
                Process = process,
                Collaboration = new Collaboration
                {
                    Id = "Collaboration_1",
                    Participant = new Participant
                    {
                        Id = "Participant_1",
                        ProcessRef = "Process_1",
                    },
                },
                BPMNDiagram = new BPMNDiagram
                {
                    Id = "BPMNDiagram_1",
                    BPMNPlane = GenerateLayoutDiagram(processVersion, process),
                },
            };
        }

        private static Models.BpmnIo.Process GenerateProcess(ProcessVersionData processVersion)
        {
            var activities = processVersion.Activities;
            if (activities.Count == 0)
            {
                throw new System.ArgumentException($"Não é possível gerar processo sem nenhuma atividade.");
            }

            var startEventId = "StartEvent_" + faker.Random.Number(9);
            var endEventId = "Event_" + faker.Random.AlphaNumeric(7);

            var sequenceFlows = GenerateSequenceFlow(processVersion.Activities, startEventId, endEventId);

            var firstActivityId = activities.First().ActivityId;
            var lastActivityId = activities.Last().ActivityId;

            var allActivities = processVersion.AllActivities;
            return new Models.BpmnIo.Process
            {
                Id = "Process_1",
                IsExecutable = false,
                StartEvent = new List<StartEvent>
                {
                    new StartEvent
                    {
                        Id = startEventId,
                        Outgoing = sequenceFlows.Where(f => f.SourceRef == startEventId).Select(s => s.Id).ToList(),
                    },
                },
                SendTask = GenerateSendTasks(allActivities.OfType<ActivitySendData>(), sequenceFlows),
                UserTask = GenerateUserTasks(allActivities.OfType<ActivityUserData>(), sequenceFlows),
                SatelittiSigner = GenerateSatelittiSignerTasks(allActivities.OfType<ActivitySignerData>(), sequenceFlows),
                ExclusiveGateway = GenerateExclusiveGateways(allActivities.OfType<ExclusiveGatewayData>(), sequenceFlows),
                SequenceFlow = sequenceFlows,
                // LaneSet Não implementado
                EndEvent = new List<EndEvent>
                {
                    new EndEvent
                    {
                        Id = endEventId,
                        Incoming = sequenceFlows.Where(s => s.TargetRef == endEventId).Select(s => s.Id).ToList(),
                    },
                },
            };
        }

        private static List<SendTask> GenerateSendTasks(IEnumerable<ActivitySendData> activitiesSend, List<SequenceFlow> sequenceFlows)
        {
            return activitiesSend
                .Select(a =>
                    new SendTask
                    {
                        Id = a.ActivityId,
                        Message = a.Message,
                        DestinataryType = (int)a.DestinataryType,
                        DestinataryId = a.DestinataryId ?? 0,
                        Name = a.Name,
                        TitleMessage = a.TitleMessage,
                        CustomEmail = a.CustomEmail,
                        Incoming = sequenceFlows.Where(s => s.TargetRef == a.ActivityId).Select(s => s.Id).ToList(),
                        Outgoing = sequenceFlows.Where(s => s.SourceRef == a.ActivityId).Select(s => s.Id).ToList(),
                    }
                ).ToList();
        }

        private static List<UserTask> GenerateUserTasks(IEnumerable<ActivityUserData> activitiesUser, List<SequenceFlow> sequenceFlows)
        {
            return activitiesUser
                .Select(a =>
                    new UserTask
                    {
                        Id = a.ActivityId,
                        Name = a.ActivityName,
                        ExecutorId = a.ExecutorId ?? 0,
                        ExecutorType = (int)a.ExecutorType,
                        PersonId = a.PersonId ?? 0,
                        Incoming = sequenceFlows.Where(s => s.TargetRef == a.ActivityId).Select(s => s.Id).ToList(),
                        Outgoing = sequenceFlows.Where(s => s.SourceRef == a.ActivityId).Select(s => s.Id).ToList(),
                        ExtensionElements = new ExtensionElements
                        {
                            TaskOptions = new TaskOptions
                            {
                                TaskOption = a.Buttons.Select(
                                    b => new TaskOption
                                    {
                                        Description = b.Description,
                                        HasEdgeAssociated = b.Branch != null,
                                    }
                                ).ToList(),
                            },
                        },
                    }
                ).ToList();
        }

        private static List<SatelittiSigner> GenerateSatelittiSignerTasks(IEnumerable<ActivitySignerData> activitiesSigner, List<SequenceFlow> sequenceFlows)
        {
            return activitiesSigner
                .Select(a =>
                    new SatelittiSigner
                    {
                        Id = a.ActivityId,
                        Name = a.ActivityName,
                        Incoming = sequenceFlows.Where(s => s.TargetRef == a.ActivityId).Select(s => s.Id).ToList(),
                        Outgoing = sequenceFlows.Where(s => s.SourceRef == a.ActivityId).Select(s => s.Id).ToList(),
                    }
                ).ToList();
        }

        private static List<ExclusiveGateway> GenerateExclusiveGateways(IEnumerable<ExclusiveGatewayData> exclusivesGateway, List<SequenceFlow> sequenceFlows)
        {
            return exclusivesGateway
                .Select(a =>
                    new ExclusiveGateway
                    {
                        Id = a.ActivityId,
                        Name = a.ActivityName,
                        Incoming = sequenceFlows.Where(s => s.TargetRef == a.ActivityId).Select(a => a.Id).ToList(),
                        Outgoing = sequenceFlows.Where(s => s.SourceRef == a.ActivityId).Select(a => a.Id).ToList(),
                    }
                ).ToList();
        }

        private static List<SequenceFlow> GenerateSequenceFlow(IList<ActivityBaseData> activities, string startEventId, string endEventId, string optionFirstSequenceFlow = null)
        {
            var sequenceFlows = new List<SequenceFlow>();

            var activityIdLast = startEventId;
            for (int i = 0; i < activities.Count; i++)
            {
                var activity = activities[i];

                var sequenceFlow = new SequenceFlow
                {
                    Id = "Flow_" + faker.Random.AlphaNumeric(7),
                    SourceRef = activityIdLast,
                    TargetRef = activityIdLast = activity.ActivityId,
                    Option = i == 0 ? optionFirstSequenceFlow : null,
                    Name = i == 0 ? optionFirstSequenceFlow : null,
                };
                sequenceFlows.Add(sequenceFlow);


                if (activity is ExclusiveGatewayData exclusiveGatewayData)
                {
                    foreach (var branch in exclusiveGatewayData.Branchs)
                    {
                        var sequenceFlowsBranch = GenerateSequenceFlow(branch.Activities, activityIdLast, endEventId, branch.Button?.Description);
                        sequenceFlows.AddRange(sequenceFlowsBranch);
                    }

                    var isLastActivity = (i + 1) == activities.Count;
                    if (isLastActivity)
                    {
                        return sequenceFlows;
                    }
                    throw new System.Exception("Depois de um ExclusiveGateway não pode ter mais atividades, o fluxo continua com as branchs.");
                }

            }

            var sequenceFlowEnd = new SequenceFlow
            {
                Id = "Flow_" + faker.Random.AlphaNumeric(7),
                SourceRef = activityIdLast,
                TargetRef = endEventId,
            };
            sequenceFlows.Add(sequenceFlowEnd);

            return sequenceFlows;
        }

        private static BPMNPlane GenerateLayoutDiagram(ProcessVersionData processVersion, Models.BpmnIo.Process process)
        {
            const int positionYActivity = 218;
            const int positionYWaipoint = 258;
            const int positionYStartEndEvent = 240;
            const int sequenceFlowWidth = 52;
            const int activityHeight = 80;
            const int activityWidth = 100;
            const int sizeStartEndEvent = 36;
            const int positionXStart = 212;
            const int gatewayHeight = 50;
            const int gatewayWidth = 50;
            const int marginToPool = 60;

            var allActivities = processVersion.AllActivities;

            var behindShapeBounds = new Bounds
            {
                Height = 250,
                Width = 600,
                X = positionXStart,
                Y = 133,
            };

            var bPMNShapes = new List<BPMNShape>
            {
                new BPMNShape
                {
                    Id = "Participant_2",
                    BpmnElement = "Participant_1",
                    IsHorizontal = true,
                    Bounds = behindShapeBounds,
                }
            };
            var bPMNEdges = new List<BPMNEdge>();

            var lastActivityId = allActivities.Last().ActivityId;
            var lastActivitySequenceFlowTarget = process.SequenceFlow.First(s => s.SourceRef == lastActivityId);
            var endEnvent = process.EndEvent.First(s => s.Incoming.Contains(lastActivitySequenceFlowTarget.Id));

            var firstActivityId = allActivities.First().ActivityId;
            var firstActivitySequenceFlowSource = process.SequenceFlow.First(s => s.TargetRef == firstActivityId);
            var startEnvent = process.StartEvent.First(s => s.Outgoing.Contains(firstActivitySequenceFlowSource.Id));

            var positionX = positionXStart + marginToPool;
            bPMNShapes.Add(new BPMNShape
            {
                BpmnElement = startEnvent.Id,
                Id = startEnvent.Id + "_di",
                Bounds = new Bounds
                {
                    Height = sizeStartEndEvent,
                    Width = sizeStartEndEvent,
                    X = positionX,
                    Y = positionYStartEndEvent,
                },
            });

            positionX += sizeStartEndEvent;
            foreach (var activity in allActivities)
            {
                var sequenceFlows = process.SequenceFlow.Where(s => s.TargetRef == activity.ActivityId);
                foreach (var sequenceFlow in sequenceFlows)
                {
                    var bPMNEdge = new BPMNEdge
                    {
                        BpmnElement = sequenceFlow.Id,
                        Id = sequenceFlow.Id + "_di",
                        Waypoint = new List<Waypoint>
                        {
                            new Waypoint(){ X = positionX, Y =  positionYWaipoint},
                            new Waypoint(){ X = (positionX += sequenceFlowWidth), Y =  positionYWaipoint},
                        },
                    };
                    bPMNEdges.Add(bPMNEdge);
                }


                var bPMNShape = new BPMNShape
                {
                    BpmnElement = activity.ActivityId,
                    Id = activity.ActivityId + "_di",
                    Bounds = new Bounds
                    {
                        Height = activity.ActivityType == WorkflowActivityTypeEnum.EXCLUSIVE_GATEWAY_ACTIVITY ? gatewayHeight : activityHeight,
                        Width = activity.ActivityType == WorkflowActivityTypeEnum.EXCLUSIVE_GATEWAY_ACTIVITY ? gatewayWidth : activityWidth,
                        X = positionX,
                        Y = positionYActivity,
                    },
                };
                bPMNShapes.Add(bPMNShape);

                positionX += activityWidth;
            }

            var sequenceFlowsEnd = process.SequenceFlow.Where(s => s.TargetRef == endEnvent.Id);
            foreach (var sequenceFlowEnd in sequenceFlowsEnd)
            {
                var bPMNEdgeEnd = new BPMNEdge
                {
                    BpmnElement = sequenceFlowEnd.Id,
                    Id = sequenceFlowEnd.Id + "_di",
                    Waypoint = new List<Waypoint>
                    {
                        new Waypoint(){ X = positionX, Y =  positionYWaipoint},
                        new Waypoint(){ X = (positionX += sequenceFlowWidth), Y =  positionYWaipoint},
                    },
                };
                bPMNEdges.Add(bPMNEdgeEnd);
            }

            bPMNShapes.Add(new BPMNShape
            {
                BpmnElement = endEnvent.Id,
                Id = endEnvent.Id + "_di",
                Bounds = new Bounds
                {
                    Height = sizeStartEndEvent,
                    Width = sizeStartEndEvent,
                    X = positionX,
                    Y = positionYStartEndEvent,
                },
            });

            behindShapeBounds.Width = positionX - positionXStart + marginToPool;

            return new BPMNPlane
            {
                Id = "BPMNPlane_1",
                BpmnElement = "Collaboration_1",
                BPMNShape = bPMNShapes,
                BPMNEdge = bPMNEdges,
            };

        }

        private static string Serializer(Definitions definitions)
        {
            var serializer = new XmlSerializer(typeof(Definitions));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xsd", NameSpace.xsd);
            namespaces.Add("xsi", NameSpace.xsi);
            namespaces.Add("bpmn2", NameSpace.bpmn2);
            namespaces.Add("bpmndi", NameSpace.bpmndi);
            namespaces.Add("dc", NameSpace.dc);
            namespaces.Add("satelitti", NameSpace.satelitti);
            namespaces.Add("di", NameSpace.di);
            namespaces.Add("targetNamespace", "http://bpmn.io/schema/bpmn");
            namespaces.Add("schemaLocation", "http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd");

            using var writer = new Utf8StringWriter();
            serializer.Serialize(writer, definitions, namespaces);
            return writer.ToString();
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
