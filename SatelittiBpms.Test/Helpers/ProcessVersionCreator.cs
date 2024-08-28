using NUnit.Framework;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Helpers
{
    public static class ProcessVersionCreator
    {
        private static ProcessVersionDTO DefaultProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField
        {
            get
            {
                return new ProcessVersionDTO
                {
                    Description = null,
                    DescriptionFlow = null,
                    DiagramContent = "",
                    FormContent = "{\"components\":[{\"label\":\"Campo 1\",\"placeholder\":\"\",\"widget\":{\"type\":\"input\"},\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\"},\"type\":\"textfield\",\"input\":true,\"key\":\"textField\",\"prefix\":\"\",\"customClass\":\"\",\"suffix\":\"\",\"multiple\":false,\"defaultValue\":null,\"protected\":false,\"unique\":false,\"persistent\":true,\"hidden\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"redrawOn\":\"\",\"tableView\":true,\"modalEdit\":false,\"dataGridLabel\":false,\"labelPosition\":\"top\",\"description\":\"\",\"errorLabel\":\"\",\"tooltip\":\"\",\"hideLabel\":false,\"tabindex\":\"\",\"disabled\":false,\"autofocus\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"attributes\":{},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false,\"minLength\":\"\",\"maxLength\":\"\",\"pattern\":\"\"},\"overlay\":{\"style\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"properties\":{},\"allowMultipleMasks\":false,\"mask\":false,\"inputType\":\"text\",\"inputFormat\":\"plain\",\"inputMask\":\"\",\"spellcheck\":true,\"id\":\"e32uggy6\"}]}",
                    Name = "Papel Teste 1",
                    NeedPublish = true,
                    ProcessId = 0,
                    RolesIds = new int[] { 1 },
                    TaskSequance = 2,
                    TenantId = 0,
                    Version = 0,
                    Activities = new ActivityDTO[]
                    {
                        new ActivityDTO
                        {
                            ActivityId = "Activity_0h9sl4f",
                            ActivityName = "Tarefa 1",
                            ActivityType = 0,
                            Fields =  new ActivityFieldDTO[]
                            {
                                new ActivityFieldDTO
                                {
                                    FieldId = "textField",
                                    FieldLabel = "Campo 1",
                                    FieldType = Models.Enums.FieldTypeEnum.TEXTFIELD,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.EDITABLE,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                            },
                            ProcessVersionId = 0,
                        },
                    },
                };
            }
        }

        private static ProcessVersionDTO DefaultProcessVersionWithTwoUserActivityBothWithRequesterAndWithThreeFields
        {
            get
            {
                return new ProcessVersionDTO
                {
                    Description = null,
                    DescriptionFlow = "[[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] , NUMERO DO FLUXO: [[{\"text\":\"Número Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] , \n[[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] ,  Data da solicitação: [[{\"text\":\"Data da Solicitação\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]]\n",
                    DiagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:collaboration id=\"Collaboration_1\"><bpmn2:participant id=\"Participant_1\" processRef=\"Process_1\" /></bpmn2:collaboration><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:startEvent id=\"StartEvent_1\"><bpmn2:outgoing>Flow_0r77rvl</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_0h9sl4f\" name=\"Tarefa 1\" satelitti:executorType=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Avançar\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_0r77rvl</bpmn2:incoming><bpmn2:outgoing>Flow_1h6p1te</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_0r77rvl\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_0h9sl4f\" /><bpmn2:userTask id=\"Activity_1tpkr43\" name=\"Tarefa 3\" satelitti:executorType=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Avançar\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_1h6p1te</bpmn2:incoming><bpmn2:outgoing>Flow_0u0s5ox</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_1h6p1te\" sourceRef=\"Activity_0h9sl4f\" targetRef=\"Activity_1tpkr43\" /><bpmn2:endEvent id=\"Event_1o29gag\"><bpmn2:incoming>Flow_0u0s5ox</bpmn2:incoming></bpmn2:endEvent><bpmn2:sequenceFlow id=\"Flow_0u0s5ox\" sourceRef=\"Activity_1tpkr43\" targetRef=\"Event_1o29gag\" /></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Collaboration_1\"><bpmndi:BPMNShape id=\"Participant_2\" bpmnElement=\"Participant_1\" isHorizontal=\"true\"><dc:Bounds x=\"310\" y=\"133\" width=\"600\" height=\"250\" /></bpmndi:BPMNShape><bpmndi:BPMNEdge id=\"Flow_0r77rvl_di\" bpmnElement=\"Flow_0r77rvl\"><di:waypoint x=\"448\" y=\"258\" /><di:waypoint x=\"500\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_1h6p1te_di\" bpmnElement=\"Flow_1h6p1te\"><di:waypoint x=\"600\" y=\"258\" /><di:waypoint x=\"660\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_0u0s5ox_di\" bpmnElement=\"Flow_0u0s5ox\"><di:waypoint x=\"760\" y=\"258\" /><di:waypoint x=\"822\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"412\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0h9sl4f_di\" bpmnElement=\"Activity_0h9sl4f\"><dc:Bounds x=\"500\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_1tpkr43_di\" bpmnElement=\"Activity_1tpkr43\"><dc:Bounds x=\"660\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_1o29gag_di\" bpmnElement=\"Event_1o29gag\"><dc:Bounds x=\"822\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>",
                    FormContent = "{\"components\":[{\"label\":\"Nome\",\"placeholder\":\"\",\"widget\":{\"type\":\"input\"},\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\"},\"type\":\"textfield\",\"input\":true,\"key\":\"textField\",\"prefix\":\"\",\"customClass\":\"\",\"suffix\":\"\",\"multiple\":false,\"defaultValue\":null,\"protected\":false,\"unique\":false,\"persistent\":true,\"hidden\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"redrawOn\":\"\",\"tableView\":true,\"modalEdit\":false,\"dataGridLabel\":false,\"labelPosition\":\"top\",\"description\":\"\",\"errorLabel\":\"\",\"tooltip\":\"\",\"hideLabel\":false,\"tabindex\":\"\",\"disabled\":false,\"autofocus\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"attributes\":{},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false,\"minLength\":\"\",\"maxLength\":\"\",\"pattern\":\"\"},\"overlay\":{\"style\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"properties\":{},\"allowMultipleMasks\":false,\"mask\":false,\"inputType\":\"text\",\"inputFormat\":\"plain\",\"inputMask\":\"\",\"spellcheck\":true,\"id\":\"eyvuxy\"},{\"label\":\"Email\",\"placeholder\":\"\",\"widget\":{\"type\":\"input\"},\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\"},\"type\":\"email\",\"input\":true,\"key\":\"email\",\"prefix\":\"\",\"customClass\":\"\",\"suffix\":\"\",\"multiple\":false,\"defaultValue\":null,\"protected\":false,\"unique\":false,\"persistent\":true,\"hidden\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"redrawOn\":\"\",\"tableView\":true,\"modalEdit\":false,\"dataGridLabel\":false,\"labelPosition\":\"top\",\"description\":\"\",\"errorLabel\":\"\",\"tooltip\":\"\",\"hideLabel\":false,\"tabindex\":\"\",\"disabled\":false,\"autofocus\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"attributes\":{},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false,\"minLength\":\"\",\"maxLength\":\"\",\"pattern\":\"\"},\"overlay\":{\"style\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"properties\":{},\"allowMultipleMasks\":false,\"mask\":false,\"inputType\":\"email\",\"inputFormat\":\"plain\",\"inputMask\":\"\",\"spellcheck\":true,\"kickbox\":{\"enabled\":false},\"id\":\"e9ecmm\"},{\"label\":\"Observação\",\"placeholder\":\"\",\"widget\":{\"type\":\"input\"},\"customConditional\":\"\",\"conditional\":{\"json\":\"\",\"show\":null,\"when\":null,\"eq\":\"\"},\"type\":\"textarea\",\"rows\":3,\"wysiwyg\":false,\"input\":true,\"key\":\"textArea\",\"prefix\":\"\",\"customClass\":\"\",\"suffix\":\"\",\"multiple\":false,\"defaultValue\":null,\"protected\":false,\"unique\":false,\"persistent\":true,\"hidden\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"redrawOn\":\"\",\"tableView\":true,\"modalEdit\":false,\"dataGridLabel\":false,\"labelPosition\":\"top\",\"description\":\"\",\"errorLabel\":\"\",\"tooltip\":\"\",\"hideLabel\":false,\"tabindex\":\"\",\"disabled\":false,\"autofocus\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"attributes\":{},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false,\"minLength\":\"\",\"maxLength\":\"\",\"pattern\":\"\",\"minWords\":\"\",\"maxWords\":\"\"},\"overlay\":{\"style\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"properties\":{},\"allowMultipleMasks\":false,\"mask\":false,\"inputType\":\"text\",\"inputFormat\":\"html\",\"inputMask\":\"\",\"spellcheck\":true,\"editor\":\"\",\"fixedSize\":true,\"id\":\"e9odxoh\"}]}",
                    Name = "PROCESS 1",
                    NeedPublish = true,
                    ProcessId = 0,
                    RolesIds = new int[] { 1 },
                    TaskSequance = 3,
                    TenantId = 0,
                    Version = 0,
                    Activities = new ActivityDTO[]
                        {
                        new ActivityDTO
                        {
                            ActivityId = "Activity_0h9sl4f",
                            ActivityName = "Tarefa 1",
                            ActivityType = 0,
                            Fields =  new ActivityFieldDTO[]
                            {
                                new ActivityFieldDTO
                                {
                                    FieldId = "textField",
                                    FieldLabel = "Nome",
                                    FieldType = Models.Enums.FieldTypeEnum.TEXTFIELD,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                                 new ActivityFieldDTO
                                {
                                    FieldId = "email",
                                    FieldLabel = "Email",
                                    FieldType = Models.Enums.FieldTypeEnum.EMAIL,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                                  new ActivityFieldDTO
                                {
                                    FieldId = "textArea",
                                    FieldLabel = "Observação",
                                    FieldType = Models.Enums.FieldTypeEnum.TEXTAREA,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                            },
                            ProcessVersionId = 0,
                        },
                        new ActivityDTO
                        {
                            ActivityId = "Activity_1tpkr43",
                            ActivityName = "Tarefa 3",
                            ActivityType = 0,
                            Fields =  new ActivityFieldDTO[]
                            {
                                new ActivityFieldDTO
                                {
                                    FieldId = "textField",
                                    FieldLabel = "Nome",
                                    FieldType = Models.Enums.FieldTypeEnum.TEXTFIELD,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                                 new ActivityFieldDTO
                                {
                                    FieldId = "email",
                                    FieldLabel = "Email",
                                    FieldType = Models.Enums.FieldTypeEnum.EMAIL,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                                  new ActivityFieldDTO
                                {
                                    FieldId = "textArea",
                                    FieldLabel = "Observação",
                                    FieldType = Models.Enums.FieldTypeEnum.TEXTAREA,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                            },
                            ProcessVersionId = 0,
                        },
                    },
                };
            }
        }

        private static ProcessVersionDTO DefaultProcessVersionWithUserActivityBothWithPaperAndWithThreeFields
        {
            get
            {
                return new ProcessVersionDTO
                {
                    Description = "DESCRICAO DO PROCESSO",
                    DescriptionFlow = "NUMERO FLUXO: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] \nPROCESSO: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]]\n\nNome completo:[[{\"text\":\"Nome completo\",\"value\":\"textField\",\"prefix\":\"@\"}]]\nEmail: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]] \nCidade: [[{\"text\":\"Cidade\",\"value\":\"select\",\"prefix\":\"@\"}]]\n\n\nNumero fluxo? [[{\"text\":\"Número Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]]\n",
                    DiagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:collaboration id=\"Collaboration_1\"><bpmn2:participant id=\"Participant_1\" processRef=\"Process_1\" /></bpmn2:collaboration><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:startEvent id=\"StartEvent_1\"><bpmn2:outgoing>Flow_0mrrmoz</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_1kg89ke\" name=\"Tarefa 1\" satelitti:executorType=\"2\" satelitti:executorId=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Avançar\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_0mrrmoz</bpmn2:incoming><bpmn2:outgoing>Flow_00tzvrj</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_0mrrmoz\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_1kg89ke\" /><bpmn2:userTask id=\"Activity_1mj0fvx\" name=\"Tarefa 2\" satelitti:executorType=\"2\" satelitti:executorId=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Avançar\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_00tzvrj</bpmn2:incoming><bpmn2:outgoing>Flow_0j7qigc</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_00tzvrj\" sourceRef=\"Activity_1kg89ke\" targetRef=\"Activity_1mj0fvx\" /><bpmn2:endEvent id=\"Event_1lihxhv\"><bpmn2:incoming>Flow_0j7qigc</bpmn2:incoming></bpmn2:endEvent><bpmn2:sequenceFlow id=\"Flow_0j7qigc\" sourceRef=\"Activity_1mj0fvx\" targetRef=\"Event_1lihxhv\" /></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Collaboration_1\"><bpmndi:BPMNShape id=\"Participant_2\" bpmnElement=\"Participant_1\" isHorizontal=\"true\"><dc:Bounds x=\"310\" y=\"133\" width=\"600\" height=\"250\" /></bpmndi:BPMNShape><bpmndi:BPMNEdge id=\"Flow_0mrrmoz_di\" bpmnElement=\"Flow_0mrrmoz\"><di:waypoint x=\"448\" y=\"258\" /><di:waypoint x=\"500\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_00tzvrj_di\" bpmnElement=\"Flow_00tzvrj\"><di:waypoint x=\"600\" y=\"258\" /><di:waypoint x=\"660\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_0j7qigc_di\" bpmnElement=\"Flow_0j7qigc\"><di:waypoint x=\"760\" y=\"258\" /><di:waypoint x=\"822\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"412\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_1kg89ke_di\" bpmnElement=\"Activity_1kg89ke\"><dc:Bounds x=\"500\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_1mj0fvx_di\" bpmnElement=\"Activity_1mj0fvx\"><dc:Bounds x=\"660\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_1lihxhv_di\" bpmnElement=\"Event_1lihxhv\"><dc:Bounds x=\"822\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>",
                    FormContent = "{\"components\":[{\"label\":\"Nome completo\",\"placeholder\":\"\",\"widget\":{\"type\":\"input\"},\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\"},\"type\":\"textfield\",\"input\":true,\"key\":\"textField\",\"prefix\":\"\",\"customClass\":\"\",\"suffix\":\"\",\"multiple\":false,\"defaultValue\":null,\"protected\":false,\"unique\":false,\"persistent\":true,\"hidden\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"redrawOn\":\"\",\"tableView\":true,\"modalEdit\":false,\"dataGridLabel\":false,\"labelPosition\":\"top\",\"description\":\"\",\"errorLabel\":\"\",\"tooltip\":\"\",\"hideLabel\":false,\"tabindex\":\"\",\"disabled\":false,\"autofocus\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"attributes\":{},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false,\"minLength\":\"\",\"maxLength\":\"\",\"pattern\":\"\"},\"overlay\":{\"style\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"properties\":{},\"allowMultipleMasks\":false,\"mask\":false,\"inputType\":\"text\",\"inputFormat\":\"plain\",\"inputMask\":\"\",\"spellcheck\":true,\"id\":\"ej4giq4\"},{\"label\":\"Email\",\"placeholder\":\"\",\"widget\":{\"type\":\"input\"},\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\"},\"type\":\"email\",\"input\":true,\"key\":\"email\",\"prefix\":\"\",\"customClass\":\"\",\"suffix\":\"\",\"multiple\":false,\"defaultValue\":null,\"protected\":false,\"unique\":false,\"persistent\":true,\"hidden\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"redrawOn\":\"\",\"tableView\":true,\"modalEdit\":false,\"dataGridLabel\":false,\"labelPosition\":\"top\",\"description\":\"\",\"errorLabel\":\"\",\"tooltip\":\"\",\"hideLabel\":false,\"tabindex\":\"\",\"disabled\":false,\"autofocus\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"attributes\":{},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false,\"minLength\":\"\",\"maxLength\":\"\",\"pattern\":\"\"},\"overlay\":{\"style\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"properties\":{},\"allowMultipleMasks\":false,\"mask\":false,\"inputType\":\"email\",\"inputFormat\":\"plain\",\"inputMask\":\"\",\"spellcheck\":true,\"kickbox\":{\"enabled\":false},\"id\":\"ejblg4n\"},{\"label\":\"Cidade\",\"placeholder\":\"\",\"multiple\":true,\"dataSrc\":\"values\",\"data\":{\"values\":[{\"label\":\"Rio Preto\",\"value\":\"rioPreto\"},{\"label\":\"São Paulo\",\"value\":\"saoPaulo\"}],\"resource\":\"\",\"json\":\"\",\"url\":\"\",\"custom\":\"\"},\"valueProperty\":\"\",\"template\":\"<span>{{ item.label }}</span>\",\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\"},\"type\":\"select\",\"indexeddb\":{\"filter\":{}},\"selectFields\":\"\",\"minSearch\":0,\"redrawOn\":\"\",\"input\":true,\"key\":\"select\",\"prefix\":\"\",\"customClass\":\"\",\"suffix\":\"\",\"protected\":false,\"unique\":false,\"persistent\":true,\"hidden\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"tableView\":true,\"modalEdit\":false,\"dataGridLabel\":false,\"labelPosition\":\"top\",\"description\":\"\",\"errorLabel\":\"\",\"tooltip\":\"\",\"hideLabel\":false,\"tabindex\":\"\",\"disabled\":false,\"autofocus\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"widget\":null,\"attributes\":{},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false,\"onlyAvailableItems\":false},\"overlay\":{\"style\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"properties\":{},\"allowMultipleMasks\":false,\"idPath\":\"id\",\"clearOnRefresh\":false,\"limit\":100,\"lazyLoad\":true,\"filter\":\"\",\"searchEnabled\":true,\"searchField\":\"\",\"readOnlyValue\":false,\"authenticate\":false,\"ignoreCache\":false,\"searchThreshold\":0.3,\"uniqueOptions\":false,\"fuseOptions\":{\"include\":\"score\",\"threshold\":0.3},\"customOptions\":{},\"useExactSearch\":false,\"id\":\"eaijfob\",\"defaultValue\":\"\"}]}",
                    Name = "PROCESS 2",
                    NeedPublish = true,
                    ProcessId = 0,
                    RolesIds = new int[] { 1 },
                    TaskSequance = 3,
                    TenantId = 0,
                    Version = 0,
                    Activities = new ActivityDTO[]
                        {
                        new ActivityDTO
                        {
                            ActivityId = "Activity_1kg89ke",
                            ActivityName = "Tarefa 1",
                            //TODO Vem como 0 do frontend ao criar um evento, mas salva certo, teria de ver se realmente precisa desse campo vir do frontend
                            ActivityType = 0,
                            Fields =  new ActivityFieldDTO[]
                            {
                                new ActivityFieldDTO
                                {
                                    FieldId = "textField",
                                    FieldLabel = "Nome completo",
                                    FieldType = Models.Enums.FieldTypeEnum.TEXTFIELD,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                                 new ActivityFieldDTO
                                {
                                    FieldId = "email",
                                    FieldLabel = "Email",
                                    FieldType = Models.Enums.FieldTypeEnum.EMAIL,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                                  new ActivityFieldDTO
                                {
                                    FieldId = "select",
                                    FieldLabel = "Cidade",
                                    FieldType = Models.Enums.FieldTypeEnum.SELECT,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                            },
                            ProcessVersionId = 0,
                        },
                        new ActivityDTO
                        {
                            ActivityId = "Activity_1mj0fvx",
                            ActivityName = "Tarefa 2",
                            ActivityType = 0,
                            Fields =  new ActivityFieldDTO[]
                            {
                                new ActivityFieldDTO
                                {
                                    FieldId = "textField",
                                    FieldLabel = "Nome completo",
                                    FieldType = Models.Enums.FieldTypeEnum.TEXTFIELD,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                                 new ActivityFieldDTO
                                {
                                    FieldId = "email",
                                    FieldLabel = "Email",
                                    FieldType = Models.Enums.FieldTypeEnum.EMAIL,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                                  new ActivityFieldDTO
                                {
                                    FieldId = "select",
                                    FieldLabel = "Cidade",
                                    FieldType = Models.Enums.FieldTypeEnum.SELECT,
                                    ProcessVersionId = 0,
                                    State = Models.Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                    SystemFieldId = 0,
                                    TaskId = 0,
                                },
                            },
                            ProcessVersionId = 0,
                        },
                    },
                };
            }
        }

        public static ProcessVersionDTO ProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField
        {
            get
            {
                var defaultProcessVersion = DefaultProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField;
                defaultProcessVersion.DiagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:collaboration id=\"Collaboration_1\"><bpmn2:participant id=\"Participant_1\" processRef=\"Process_1\" /></bpmn2:collaboration><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:startEvent id=\"StartEvent_1\"><bpmn2:outgoing>Flow_18fyhd2</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_09i9hr8\" name=\"Tarefa 1\" satelitti:executorType=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Concluir\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_18fyhd2</bpmn2:incoming><bpmn2:outgoing>Flow_1t5s9i2</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_18fyhd2\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_09i9hr8\" /><bpmn2:endEvent id=\"Event_0qoynwl\"><bpmn2:incoming>Flow_1t5s9i2</bpmn2:incoming></bpmn2:endEvent><bpmn2:sequenceFlow id=\"Flow_1t5s9i2\" sourceRef=\"Activity_09i9hr8\" targetRef=\"Event_0qoynwl\" /></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Collaboration_1\"><bpmndi:BPMNShape id=\"Participant_2\" bpmnElement=\"Participant_1\" isHorizontal=\"true\"><dc:Bounds x=\"310\" y=\"133\" width=\"600\" height=\"250\" /></bpmndi:BPMNShape><bpmndi:BPMNEdge id=\"Flow_18fyhd2_di\" bpmnElement=\"Flow_18fyhd2\"><di:waypoint x=\"448\" y=\"258\" /><di:waypoint x=\"500\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_1t5s9i2_di\" bpmnElement=\"Flow_1t5s9i2\"><di:waypoint x=\"600\" y=\"258\" /><di:waypoint x=\"652\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"412\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_09i9hr8_di\" bpmnElement=\"Activity_09i9hr8\"><dc:Bounds x=\"500\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_0qoynwl_di\" bpmnElement=\"Event_0qoynwl\"><dc:Bounds x=\"652\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>";
                return defaultProcessVersion;
            }
        }

        public static ProcessVersionDTO ProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneFieldAndErrorAtDiagram
        {
            get
            {
                var defaultProcessVersion = DefaultProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField;
                defaultProcessVersion.DiagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:collaboration id=\"Collaboration_1\"><bpmn2:participant id=\"Participant_1\" processRef=\"Process_1\" /></bpmn2:collaboration><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:startEvent id=\"StartEvent_1\"><bpmn2:outgoing>Flow_18fyhd2</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_09i9hr8\" name=\"Tarefa 1\" satelitti:executorType=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Concluir\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_18fyhd2</bpmn2:incoming><bpmn2:outgoing>Flow_1t5s9i2</bpmn2:outgoing><bpmn2:outgoing>Flow_riquih3</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_18fyhd2\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_09i9hr8\" /><bpmn2:endEvent id=\"Event_0qoynwl\"><bpmn2:incoming>Flow_1t5s9i2</bpmn2:incoming></bpmn2:endEvent><bpmn2:sequenceFlow id=\"Flow_1t5s9i2\" sourceRef=\"Activity_09i9hr8\" targetRef=\"Event_0qoynwl\" /></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Collaboration_1\"><bpmndi:BPMNShape id=\"Participant_2\" bpmnElement=\"Participant_1\" isHorizontal=\"true\"><dc:Bounds x=\"310\" y=\"133\" width=\"600\" height=\"250\" /></bpmndi:BPMNShape><bpmndi:BPMNEdge id=\"Flow_18fyhd2_di\" bpmnElement=\"Flow_18fyhd2\"><di:waypoint x=\"448\" y=\"258\" /><di:waypoint x=\"500\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_1t5s9i2_di\" bpmnElement=\"Flow_1t5s9i2\"><di:waypoint x=\"600\" y=\"258\" /><di:waypoint x=\"652\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"412\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_09i9hr8_di\" bpmnElement=\"Activity_09i9hr8\"><dc:Bounds x=\"500\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_0qoynwl_di\" bpmnElement=\"Event_0qoynwl\"><dc:Bounds x=\"652\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>";
                return defaultProcessVersion;
            }
        }

        public static async Task<int> NewProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField(this MockServices mockServices)
        {
            var result = await mockServices.GetService<IProcessVersionService>().Save(ProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField);

            Assert.IsTrue(result.Success);

            return ResultContent<int>.GetValue(result);
        }

        public static async Task<int> NewProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneFieldAndErrorAtDiagram(this MockServices mockServices)
        {
            var result = await mockServices.GetService<IProcessVersionService>().Save(ProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneFieldAndErrorAtDiagram);

            Assert.IsFalse(result.Success);

            return ResultContent<int>.GetValue(result);
        }

        public static async Task<int> NewProcessVersionWithTwoUserActivityBothWithRequesterAndWithThreeFields(this MockServices mockServices)
        {
            var result = await mockServices.GetService<IProcessVersionService>().Save(ProcessVersionWithTwoUserActivityBothWithRequesterAndWithThreeFields);

            Assert.IsTrue(result.Success);

            return ResultContent<int>.GetValue(result);
        }

        public static async Task<int> NewProcessVersionWithUserActivityBothWithPaperAndWithThreeFields(this MockServices mockServices)
        {
            var result = await mockServices.GetService<IProcessVersionService>().Save(DefaultProcessVersionWithUserActivityBothWithPaperAndWithThreeFields);

            Assert.IsTrue(result.Success);

            return ResultContent<int>.GetValue(result);
        }

        public static ProcessVersionDTO ProcessVersionWithTwoUserActivityBothWithRequesterAndWithThreeFields
        {
            get
            {
                return DefaultProcessVersionWithTwoUserActivityBothWithRequesterAndWithThreeFields;
            }
        }
    }
}
