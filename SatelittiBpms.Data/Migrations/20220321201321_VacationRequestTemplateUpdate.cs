﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatelittiBpms.Data.Migrations
{
    public partial class VacationRequestTemplateUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                column: "DiagramContent",
                value: "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:collaboration id=\"Collaboration_16f8b6m\"><bpmn2:participant id=\"Participant_02opdn4\" processRef=\"Process_1\" /></bpmn2:collaboration><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:sequenceFlow id=\"Flow_1w9kq2g\" sourceRef=\"Activity_0wm1htw\" targetRef=\"Event_08gbpmn\" /><bpmn2:sequenceFlow id=\"Flow_07d23g9\" name=\"Aprovar\" sourceRef=\"Gateway_03p61of\" targetRef=\"Activity_0wm1htw\" satelitti:option=\"Aprovar\" /><bpmn2:sequenceFlow id=\"Flow_10py7h4\" name=\"Reprovar\" sourceRef=\"Gateway_03p61of\" targetRef=\"Activity_0brhvai\" satelitti:option=\"Reprovar\" /><bpmn2:sequenceFlow id=\"Flow_0xvnkdk\" sourceRef=\"Activity_1rtzi3p\" targetRef=\"Activity_0hnppbw\" /><bpmn2:sequenceFlow id=\"Flow_1p1d2xz\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_1rtzi3p\" /><bpmn2:sequenceFlow id=\"Flow_0s4j4tr\" sourceRef=\"Activity_0brhvai\" targetRef=\"Event_1gfzkh9\" /><bpmn2:sequenceFlow id=\"Flow_0rm3n25\" sourceRef=\"Activity_0hnppbw\" targetRef=\"Gateway_03p61of\" /><bpmn2:startEvent id=\"StartEvent_1\" name=\"Criar solicitação de férias\"><bpmn2:outgoing>Flow_1p1d2xz</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_0hnppbw\" name=\"RH Avalia Solicitação\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Aprovar\" hasEdgeAssociated=\"true\" /><satelitti:taskOption description=\"Reprovar\" hasEdgeAssociated=\"true\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_0xvnkdk</bpmn2:incoming><bpmn2:outgoing>Flow_0rm3n25</bpmn2:outgoing></bpmn2:userTask><bpmn2:exclusiveGateway id=\"Gateway_03p61of\" name=\"Férias aprovada?\"><bpmn2:incoming>Flow_0rm3n25</bpmn2:incoming><bpmn2:outgoing>Flow_10py7h4</bpmn2:outgoing><bpmn2:outgoing>Flow_07d23g9</bpmn2:outgoing></bpmn2:exclusiveGateway><bpmn2:sendTask id=\"Activity_0wm1htw\" name=\"Notificar aprovação\" satelitti:message=\"Ola!&#10;&#10;Suas férias foram aprovadas! Não responda aqui.&#10;Setor: [[{&#34;text&#34;:&#34;Setor&#34;,&#34;value&#34;:&#34;textField1&#34;,&#34;prefix&#34;:&#34;@&#34;}]] &#10;Solicitante: [[{&#34;text&#34;:&#34;Solicitante&#34;,&#34;value&#34;:&#34;wildcards.requester&#34;,&#34;prefix&#34;:&#34;#&#34;}]]&#10;\" satelitti:destinataryType=\"1\" satelitti:titleMessage=\"Férias Aprovada [[{&#34;text&#34;:&#34;Solicitante&#34;,&#34;value&#34;:&#34;wildcards.requester&#34;,&#34;prefix&#34;:&#34;#&#34;}]] . Não responder! Inicia em: [[{&#34;text&#34;:&#34;Data de início&#34;,&#34;value&#34;:&#34;dateTime&#34;,&#34;prefix&#34;:&#34;@&#34;}]]\"><bpmn2:incoming>Flow_07d23g9</bpmn2:incoming><bpmn2:outgoing>Flow_1w9kq2g</bpmn2:outgoing></bpmn2:sendTask><bpmn2:sendTask id=\"Activity_0brhvai\" name=\"Notificar reprovação\" satelitti:message=\"Ola!&#10;&#10;Sua solicitação de férias foi reprovada. Favor alterar a data ou entrar em contato com o RH.\" satelitti:destinataryType=\"1\" satelitti:titleMessage=\"Férias reprovada\"><bpmn2:incoming>Flow_10py7h4</bpmn2:incoming><bpmn2:outgoing>Flow_0s4j4tr</bpmn2:outgoing></bpmn2:sendTask><bpmn2:endEvent id=\"Event_08gbpmn\" name=\"Férias Aprovada\"><bpmn2:incoming>Flow_1w9kq2g</bpmn2:incoming></bpmn2:endEvent><bpmn2:endEvent id=\"Event_1gfzkh9\" name=\"Férias Reprovada\"><bpmn2:incoming>Flow_0s4j4tr</bpmn2:incoming></bpmn2:endEvent><bpmn2:userTask id=\"Activity_1rtzi3p\" name=\"Solicitante preenche informações\" satelitti:executorType=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Enviar para RH\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_1p1d2xz</bpmn2:incoming><bpmn2:outgoing>Flow_0xvnkdk</bpmn2:outgoing></bpmn2:userTask></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Collaboration_16f8b6m\"><bpmndi:BPMNShape id=\"Participant_02opdn4_di\" bpmnElement=\"Participant_02opdn4\" isHorizontal=\"true\"><dc:Bounds x=\"180\" y=\"120\" width=\"1160\" height=\"310\" /></bpmndi:BPMNShape><bpmndi:BPMNEdge id=\"Flow_1w9kq2g_di\" bpmnElement=\"Flow_1w9kq2g\"><di:waypoint x=\"1140\" y=\"180\" /><di:waypoint x=\"1232\" y=\"180\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_07d23g9_di\" bpmnElement=\"Flow_07d23g9\"><di:waypoint x=\"850\" y=\"233\" /><di:waypoint x=\"850\" y=\"180\" /><di:waypoint x=\"1040\" y=\"180\" /><bpmndi:BPMNLabel><dc:Bounds x=\"870\" y=\"163\" width=\"39\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_10py7h4_di\" bpmnElement=\"Flow_10py7h4\"><di:waypoint x=\"850\" y=\"283\" /><di:waypoint x=\"850\" y=\"370\" /><di:waypoint x=\"1040\" y=\"370\" /><bpmndi:BPMNLabel><dc:Bounds x=\"842\" y=\"324\" width=\"47\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_0xvnkdk_di\" bpmnElement=\"Flow_0xvnkdk\"><di:waypoint x=\"450\" y=\"258\" /><di:waypoint x=\"590\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_1p1d2xz_di\" bpmnElement=\"Flow_1p1d2xz\"><di:waypoint x=\"288\" y=\"258\" /><di:waypoint x=\"350\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_0s4j4tr_di\" bpmnElement=\"Flow_0s4j4tr\"><di:waypoint x=\"1140\" y=\"370\" /><di:waypoint x=\"1232\" y=\"370\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_0rm3n25_di\" bpmnElement=\"Flow_0rm3n25\"><di:waypoint x=\"690\" y=\"258\" /><di:waypoint x=\"825\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"252\" y=\"240\" width=\"36\" height=\"36\" /><bpmndi:BPMNLabel><dc:Bounds x=\"231\" y=\"283\" width=\"79\" height=\"27\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0hnppbw_di\" bpmnElement=\"Activity_0hnppbw\"><dc:Bounds x=\"590\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Gateway_03p61of_di\" bpmnElement=\"Gateway_03p61of\" isMarkerVisible=\"true\"><dc:Bounds x=\"825\" y=\"233\" width=\"50\" height=\"50\" /><bpmndi:BPMNLabel><dc:Bounds x=\"885\" y=\"251\" width=\"86\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0wm1htw_di\" bpmnElement=\"Activity_0wm1htw\"><dc:Bounds x=\"1040\" y=\"140\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0brhvai_di\" bpmnElement=\"Activity_0brhvai\"><dc:Bounds x=\"1040\" y=\"330\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_08gbpmn_di\" bpmnElement=\"Event_08gbpmn\"><dc:Bounds x=\"1232\" y=\"162\" width=\"36\" height=\"36\" /><bpmndi:BPMNLabel><dc:Bounds x=\"1210\" y=\"205\" width=\"81\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_1gfzkh9_di\" bpmnElement=\"Event_1gfzkh9\"><dc:Bounds x=\"1232\" y=\"352\" width=\"36\" height=\"36\" /><bpmndi:BPMNLabel><dc:Bounds x=\"1206\" y=\"395\" width=\"88\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_1rtzi3p_di\" bpmnElement=\"Activity_1rtzi3p\"><dc:Bounds x=\"350\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                column: "DiagramContent",
                value: "<?xml version=\"1.0\" encoding=\"UTF-8\"?><bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:startEvent id=\"StartEvent_1\" name=\"Criar solicitação de férias\"><bpmn2:outgoing>Flow_1p1d2xz</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_0hnppbw\" name=\"RH Avalia Solicitação\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Aprovar\" hasEdgeAssociated=\"true\" /><satelitti:taskOption description=\"Reprovar\" hasEdgeAssociated=\"true\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_0xvnkdk</bpmn2:incoming><bpmn2:outgoing>Flow_0rm3n25</bpmn2:outgoing></bpmn2:userTask><bpmn2:exclusiveGateway id=\"Gateway_03p61of\" name=\"Férias aprovada?\"><bpmn2:incoming>Flow_0rm3n25</bpmn2:incoming><bpmn2:outgoing>Flow_10py7h4</bpmn2:outgoing><bpmn2:outgoing>Flow_07d23g9</bpmn2:outgoing></bpmn2:exclusiveGateway><bpmn2:sequenceFlow id=\"Flow_0rm3n25\" sourceRef=\"Activity_0hnppbw\" targetRef=\"Gateway_03p61of\" /><bpmn2:sendTask id=\"Activity_0wm1htw\" name=\"Notificar aprovação\" satelitti:message=\"Ola!&#10;&#10;Suas férias foram aprovadas! Não responda aqui.&#10;Setor: [[{&#34;text&#34;:&#34;Setor&#34;,&#34;value&#34;:&#34;textField1&#34;,&#34;prefix&#34;:&#34;@&#34;}]] &#10;Solicitante: [[{&#34;text&#34;:&#34;Solicitante&#34;,&#34;value&#34;:&#34;wildcards.requester&#34;,&#34;prefix&#34;:&#34;#&#34;}]]&#10;\" satelitti:destinataryType=\"1\" satelitti:titleMessage=\"Férias Aprovada [[{&#34;text&#34;:&#34;Solicitante&#34;,&#34;value&#34;:&#34;wildcards.requester&#34;,&#34;prefix&#34;:&#34;#&#34;}]] . Não responder! Inicia em: [[{&#34;text&#34;:&#34;Data de início&#34;,&#34;value&#34;:&#34;dateTime&#34;,&#34;prefix&#34;:&#34;@&#34;}]]\"><bpmn2:incoming>Flow_07d23g9</bpmn2:incoming><bpmn2:outgoing>Flow_1w9kq2g</bpmn2:outgoing></bpmn2:sendTask><bpmn2:sendTask id=\"Activity_0brhvai\" name=\"Notificar reprovação\" satelitti:message=\"Ola!&#10;&#10;Sua solicitação de férias foi reprovada. Favor alterar a data ou entrar em contato com o RH.\" satelitti:destinataryType=\"1\" satelitti:titleMessage=\"Férias reprovada\"><bpmn2:incoming>Flow_10py7h4</bpmn2:incoming><bpmn2:outgoing>Flow_0s4j4tr</bpmn2:outgoing></bpmn2:sendTask><bpmn2:endEvent id=\"Event_08gbpmn\" name=\"Férias Aprovada\"><bpmn2:incoming>Flow_1w9kq2g</bpmn2:incoming></bpmn2:endEvent><bpmn2:endEvent id=\"Event_1gfzkh9\" name=\"Férias Reprovada\"><bpmn2:incoming>Flow_0s4j4tr</bpmn2:incoming></bpmn2:endEvent><bpmn2:sequenceFlow id=\"Flow_0s4j4tr\" sourceRef=\"Activity_0brhvai\" targetRef=\"Event_1gfzkh9\" /><bpmn2:userTask id=\"Activity_1rtzi3p\" name=\"Solicitante preenche informações\" satelitti:executorType=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Enviar para RH\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_1p1d2xz</bpmn2:incoming><bpmn2:outgoing>Flow_0xvnkdk</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_1p1d2xz\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_1rtzi3p\" /><bpmn2:sequenceFlow id=\"Flow_0xvnkdk\" sourceRef=\"Activity_1rtzi3p\" targetRef=\"Activity_0hnppbw\" /><bpmn2:sequenceFlow id=\"Flow_10py7h4\" name=\"Reprovar\" sourceRef=\"Gateway_03p61of\" targetRef=\"Activity_0brhvai\" satelitti:option=\"Reprovar\" /><bpmn2:sequenceFlow id=\"Flow_07d23g9\" name=\"Aprovar\" sourceRef=\"Gateway_03p61of\" targetRef=\"Activity_0wm1htw\" satelitti:option=\"Aprovar\" /><bpmn2:sequenceFlow id=\"Flow_1w9kq2g\" sourceRef=\"Activity_0wm1htw\" targetRef=\"Event_08gbpmn\" /></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Process_1\"><bpmndi:BPMNEdge id=\"Flow_1w9kq2g_di\" bpmnElement=\"Flow_1w9kq2g\"><di:waypoint x=\"1110\" y=\"180\" /><di:waypoint x=\"1202\" y=\"180\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_07d23g9_di\" bpmnElement=\"Flow_07d23g9\"><di:waypoint x=\"820\" y=\"233\" /><di:waypoint x=\"820\" y=\"180\" /><di:waypoint x=\"1010\" y=\"180\" /><bpmndi:BPMNLabel><dc:Bounds x=\"840\" y=\"163\" width=\"39\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_10py7h4_di\" bpmnElement=\"Flow_10py7h4\"><di:waypoint x=\"820\" y=\"283\" /><di:waypoint x=\"820\" y=\"370\" /><di:waypoint x=\"1010\" y=\"370\" /><bpmndi:BPMNLabel><dc:Bounds x=\"812\" y=\"324\" width=\"47\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_0xvnkdk_di\" bpmnElement=\"Flow_0xvnkdk\"><di:waypoint x=\"420\" y=\"258\" /><di:waypoint x=\"560\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_1p1d2xz_di\" bpmnElement=\"Flow_1p1d2xz\"><di:waypoint x=\"258\" y=\"258\" /><di:waypoint x=\"320\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_0s4j4tr_di\" bpmnElement=\"Flow_0s4j4tr\"><di:waypoint x=\"1110\" y=\"370\" /><di:waypoint x=\"1202\" y=\"370\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_0rm3n25_di\" bpmnElement=\"Flow_0rm3n25\"><di:waypoint x=\"660\" y=\"258\" /><di:waypoint x=\"795\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"222\" y=\"240\" width=\"36\" height=\"36\" /><bpmndi:BPMNLabel><dc:Bounds x=\"201\" y=\"283\" width=\"79\" height=\"27\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0hnppbw_di\" bpmnElement=\"Activity_0hnppbw\"><dc:Bounds x=\"560\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Gateway_03p61of_di\" bpmnElement=\"Gateway_03p61of\" isMarkerVisible=\"true\"><dc:Bounds x=\"795\" y=\"233\" width=\"50\" height=\"50\" /><bpmndi:BPMNLabel><dc:Bounds x=\"855\" y=\"251\" width=\"86\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0wm1htw_di\" bpmnElement=\"Activity_0wm1htw\"><dc:Bounds x=\"1010\" y=\"140\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0brhvai_di\" bpmnElement=\"Activity_0brhvai\"><dc:Bounds x=\"1010\" y=\"330\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_08gbpmn_di\" bpmnElement=\"Event_08gbpmn\"><dc:Bounds x=\"1202\" y=\"162\" width=\"36\" height=\"36\" /><bpmndi:BPMNLabel><dc:Bounds x=\"1180\" y=\"205\" width=\"81\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_1gfzkh9_di\" bpmnElement=\"Event_1gfzkh9\"><dc:Bounds x=\"1202\" y=\"352\" width=\"36\" height=\"36\" /><bpmndi:BPMNLabel><dc:Bounds x=\"1176\" y=\"395\" width=\"88\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_1rtzi3p_di\" bpmnElement=\"Activity_1rtzi3p\"><dc:Bounds x=\"320\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>");

        }
    }
}
