using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SatelittiBpms.Models.BpmnIo
{

    // Gerado pelo site https://json2csharp.com/xml-to-csharp
    // Foi feito um XML com todas as possibilidades e colocado no site para gerar essas classes

    public static class NameSpace
    {
        public const string xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string xsd = "http://www.w3.org/2001/XMLSchema";
        public const string bpmn2 = "http://www.omg.org/spec/BPMN/20100524/MODEL";
        public const string bpmndi = "http://www.omg.org/spec/BPMN/20100524/DI";
        public const string dc = "http://www.omg.org/spec/DD/20100524/DC";
        public const string di = "http://www.omg.org/spec/DD/20100524/DI";
        public const string satelitti = "http://selbetti/schema/bpmn/satelitti";
    }


    [XmlRoot(ElementName = "participant", Namespace = NameSpace.bpmn2)]
    public class Participant
    {

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "processRef")]
        public string ProcessRef { get; set; }
    }

    [XmlRoot(ElementName = "collaboration", Namespace = NameSpace.bpmn2)]
    public class Collaboration
    {

        [XmlElement(ElementName = "participant", Namespace = NameSpace.bpmn2)]
        public Participant Participant { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "lane", Namespace = NameSpace.bpmn2)]
    public class Lane
    {

        [XmlElement(ElementName = "flowNodeRef", Namespace = NameSpace.bpmn2)]
        public List<string> FlowNodeRef { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "executorType", Namespace = NameSpace.satelitti)]
        public int ExecutorType { get; set; }

        [XmlAttribute(AttributeName = "executorId", Namespace = NameSpace.satelitti)]
        public int ExecutorId { get; set; }
    }

    [XmlRoot(ElementName = "laneSet", Namespace = NameSpace.bpmn2)]
    public class LaneSet
    {

        [XmlElement(ElementName = "lane", Namespace = NameSpace.bpmn2)]
        public List<Lane> Lane { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    public abstract class ActivityBase
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "incoming", Namespace = NameSpace.bpmn2)]
        public List<string> Incoming { get; set; }

        [XmlElement(ElementName = "outgoing", Namespace = NameSpace.bpmn2)]
        public List<string> Outgoing { get; set; }

        [XmlIgnore]
        public Process Parent { get; set; }

        [XmlElement(ElementName = "signerIntegrationChangesNeeded", Namespace = NameSpace.satelitti)]
        public bool SignerIntegrationChangesNeeded { get; set; }
        
    }

    [XmlRoot(ElementName = "startEvent", Namespace = NameSpace.bpmn2)]
    public class StartEvent : ActivityBase
    {
    }

    [XmlRoot(ElementName = "taskOption", Namespace = NameSpace.satelitti)]
    public class TaskOption
    {

        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "hasEdgeAssociated")]
        public bool HasEdgeAssociated { get; set; }
        public bool ShouldSerializeHasEdgeAssociated()
        {
            return HasEdgeAssociated;
        }
    }

    [XmlRoot(ElementName = "taskOptions", Namespace = NameSpace.satelitti)]
    public class TaskOptions
    {

        [XmlElement(ElementName = "taskOption", Namespace = NameSpace.satelitti)]
        public List<TaskOption> TaskOption { get; set; }
    }

    [XmlRoot(ElementName = "extensionElements", Namespace = NameSpace.bpmn2)]
    public class ExtensionElements
    {

        [XmlElement(ElementName = "taskOptions", Namespace = NameSpace.satelitti)]
        public TaskOptions TaskOptions { get; set; }
    }


    [XmlRoot(ElementName = "userTask", Namespace = NameSpace.bpmn2)]
    public class UserTask : ActivityBase
    {
        [XmlElement(ElementName = "extensionElements")]
        public ExtensionElements ExtensionElements { get; set; }

        [XmlAttribute(AttributeName = "executorType", Namespace = NameSpace.satelitti)]
        public int ExecutorType { get; set; }

        [XmlAttribute(AttributeName = "executorId", Namespace = NameSpace.satelitti)]
        public int ExecutorId { get; set; }
        public bool ShouldSerializeExecutorId()
        {
            return ExecutorId != 0;
        }

        [XmlAttribute(AttributeName = "personId", Namespace = NameSpace.satelitti)]
        public int PersonId { get; set; }
        public bool ShouldSerializePersonId()
        {
            return PersonId != 0;
        }
    }


    [XmlRoot(ElementName = "satelittiSigner", Namespace = NameSpace.satelitti)]
    public class SatelittiSigner : ActivityBase
    {
        
    }

    [XmlRoot(ElementName = "sequenceFlow", Namespace = NameSpace.bpmn2)]
    public class SequenceFlow
    {

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "sourceRef")]
        public string SourceRef { get; set; }

        [XmlAttribute(AttributeName = "targetRef")]
        public string TargetRef { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "option", Namespace = NameSpace.satelitti)]
        public string Option { get; set; }
    }

    [XmlRoot(ElementName = "exclusiveGateway", Namespace = NameSpace.bpmn2)]
    public class ExclusiveGateway : ActivityBase
    {

    }

    [XmlRoot(ElementName = "sendTask", Namespace = NameSpace.bpmn2)]
    public class SendTask : ActivityBase
    {
        [XmlAttribute(AttributeName = "message", Namespace = NameSpace.satelitti)]
        public string Message { get; set; }

        [XmlAttribute(AttributeName = "destinataryType", Namespace = NameSpace.satelitti)]
        public int DestinataryType { get; set; }

        [XmlAttribute(AttributeName = "titleMessage", Namespace = NameSpace.satelitti)]
        public string TitleMessage { get; set; }

        [XmlAttribute(AttributeName = "customEmail", Namespace = NameSpace.satelitti)]
        public string CustomEmail { get; set; }

        [XmlAttribute(AttributeName = "destinataryId", Namespace = NameSpace.satelitti)]
        public int DestinataryId { get; set; }
        public bool ShouldSerializeDestinataryId()
        {
            return DestinataryId != 0;
        }
    }

    [XmlRoot(ElementName = "endEvent", Namespace = NameSpace.bpmn2)]
    public class EndEvent : ActivityBase
    {
    }

    [XmlRoot(ElementName = "process", Namespace = NameSpace.bpmn2)]
    public class Process
    {
        [XmlElement(ElementName = "laneSet", Namespace = NameSpace.bpmn2)]
        public LaneSet LaneSet { get; set; }

        [XmlElement(ElementName = "startEvent", Namespace = NameSpace.bpmn2)]
        public List<StartEvent> StartEvent { get; set; }

        [XmlElement(ElementName = "userTask", Namespace = NameSpace.bpmn2)]
        public List<UserTask> UserTask { get; set; }

        [XmlElement(ElementName = "satelittiSigner", Namespace = NameSpace.satelitti)]
        public List<SatelittiSigner> SatelittiSigner { get; set; }
        
        [XmlElement(ElementName = "sequenceFlow", Namespace = NameSpace.bpmn2)]
        public List<SequenceFlow> SequenceFlow { get; set; }

        [XmlElement(ElementName = "exclusiveGateway", Namespace = NameSpace.bpmn2)]
        public List<ExclusiveGateway> ExclusiveGateway { get; set; }

        [XmlElement(ElementName = "sendTask", Namespace = NameSpace.bpmn2)]
        public List<SendTask> SendTask { get; set; }

        [XmlElement(ElementName = "endEvent", Namespace = NameSpace.bpmn2)]
        public List<EndEvent> EndEvent { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "isExecutable")]
        public bool IsExecutable { get; set; }

    }

    [XmlRoot(ElementName = "Bounds", Namespace = NameSpace.dc)]
    public class Bounds
    {

        [XmlAttribute(AttributeName = "x")]
        public int X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public int Y { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }
    }

    [XmlRoot(ElementName = "BPMNShape", Namespace = NameSpace.bpmndi)]
    public class BPMNShape
    {

        [XmlElement(ElementName = "Bounds", Namespace = NameSpace.dc)]
        public Bounds Bounds { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "bpmnElement")]
        public string BpmnElement { get; set; }

        [XmlAttribute(AttributeName = "isHorizontal")]
        public bool IsHorizontal { get; set; }
        public bool ShouldSerializeIsHorizontal()
        {
            return IsHorizontal;
        }

        [XmlElement(ElementName = "BPMNLabel", Namespace = NameSpace.bpmndi)]
        public BPMNLabel BPMNLabel { get; set; }

        [XmlAttribute(AttributeName = "isMarkerVisible")]
        public bool IsMarkerVisible { get; set; }
        public bool ShouldSerializeIsMarkerVisible()
        {
            return IsMarkerVisible;
        }
    }

    [XmlRoot(ElementName = "waypoint", Namespace = NameSpace.di)]
    public class Waypoint
    {

        [XmlAttribute(AttributeName = "x")]
        public int X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public int Y { get; set; }
    }

    [XmlRoot(ElementName = "BPMNEdge", Namespace = NameSpace.bpmndi)]
    public class BPMNEdge
    {

        [XmlElement(ElementName = "waypoint", Namespace = NameSpace.di)]
        public List<Waypoint> Waypoint { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "bpmnElement")]
        public string BpmnElement { get; set; }

        [XmlElement(ElementName = "BPMNLabel", Namespace = NameSpace.bpmndi)]
        public BPMNLabel BPMNLabel { get; set; }
    }

    [XmlRoot(ElementName = "BPMNLabel", Namespace = NameSpace.bpmndi)]
    public class BPMNLabel
    {

        [XmlElement(ElementName = "Bounds", Namespace = NameSpace.dc)]
        public Bounds Bounds { get; set; }
    }

    [XmlRoot(ElementName = "BPMNPlane", Namespace = NameSpace.bpmndi)]
    public class BPMNPlane
    {

        [XmlElement(ElementName = "BPMNShape", Namespace = NameSpace.bpmndi)]
        public List<BPMNShape> BPMNShape { get; set; }

        [XmlElement(ElementName = "BPMNEdge", Namespace = NameSpace.bpmndi)]
        public List<BPMNEdge> BPMNEdge { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "bpmnElement")]
        public string BpmnElement { get; set; }
    }

    [XmlRoot(ElementName = "BPMNDiagram", Namespace = NameSpace.bpmndi)]
    public class BPMNDiagram
    {

        [XmlElement(ElementName = "BPMNPlane", Namespace = NameSpace.bpmndi)]
        public BPMNPlane BPMNPlane { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "definitions", Namespace = NameSpace.bpmn2)]
    public class Definitions
    {

        [XmlElement(ElementName = "collaboration", Namespace = NameSpace.bpmn2)]
        public Collaboration Collaboration { get; set; }

        [XmlElement(ElementName = "process", Namespace = NameSpace.bpmn2)]
        public Process Process { get; set; }

        [XmlElement(ElementName = "BPMNDiagram", Namespace = NameSpace.bpmndi)]
        public BPMNDiagram BPMNDiagram { get; set; }


        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "targetNamespace")]
        public string TargetNamespace { get; set; }
    }
}
