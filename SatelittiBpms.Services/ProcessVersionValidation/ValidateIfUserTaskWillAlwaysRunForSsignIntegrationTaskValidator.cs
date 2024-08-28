using FluentValidation.Results;
using SatelittiBpms.Models.BpmnIo;
using SatelittiBpms.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Services.ProcessVersionValidation
{
    internal class ValidateIfUserTaskWillAlwaysRunForSsignIntegrationTaskValidator : ProcessValidationBase
    {
        private ProcessVersionDTO _processVersionDTO;
        private Models.BpmnIo.Definitions _bpmnDefinitions;

        public ValidateIfUserTaskWillAlwaysRunForSsignIntegrationTaskValidator(Models.BpmnIo.Definitions bpmnDefinitions, ProcessVersionDTO processVersionDTO)
        {
            _processVersionDTO = processVersionDTO;
            _bpmnDefinitions = bpmnDefinitions;
        }

        public override List<ValidationFailure> Validate()
        {
            var errors = new List<ValidationFailure>();
            if (_processVersionDTO.SignerTasks == null)
            {
                return errors;
            }
            foreach (var signerTask in _processVersionDTO.SignerTasks)
            {
                if (signerTask.Signatories != null)
                {
                    foreach (var signatory in signerTask.Signatories)
                    {
                        if (signatory.RegistrationLocation != Models.Enums.SignerRegistrationLocationEnum.UserTask)
                        {
                            continue;
                        }
                        errors.AddRange(ValidateOriginActivityId(signatory.OriginActivityId, signerTask.ActivityKey));
                    }
                }
                if (signerTask.Authorizers != null)
                {
                    foreach (var authorizer in signerTask.Authorizers)
                    {
                        if (authorizer.RegistrationLocation != Models.Enums.SignerRegistrationLocationEnum.UserTask)
                        {
                            continue;
                        }
                        errors.AddRange(ValidateOriginActivityId(authorizer.OriginActivityId, signerTask.ActivityKey));
                    }
                }
            }

            return errors;
        }

        private IEnumerable<ValidationFailure> ValidateOriginActivityId(string originActivityUserId, string activitySignerKey)
        {
            var errors = new List<ValidationFailure>();

            var userTask = _bpmnDefinitions.Process.UserTask.FirstOrDefault(u => u.Id == originActivityUserId);
            if (userTask == null)
            {
                throw new System.Exception($"Não foi encontrado a atividade de usuário de id: {originActivityUserId}");
            }
            var signerTask = _bpmnDefinitions.Process.SatelittiSigner.FirstOrDefault(u => u.Id == activitySignerKey);
            if (userTask == null)
            {
                throw new System.Exception($"Não foi encontrado a atividade de intgração com o signer de id: {activitySignerKey}");
            }
            var result = GetAllElementActivitiesUpToTheStartPoint(signerTask, new List<ActivityBase>(), new List<ActivityBase>());
            if (!result.FoundStartPoint)
            {
                throw new System.Exception($"Não foi encontrado a atividade de inicio a partir da atividade de id: {signerTask.Id}");
            }
            if (!result.PathElements.Contains(userTask))
            {
                errors.Add(CreateValidationFailure($"A atividade de usuário de id `{userTask.Id}` configurada como origem dos dados para atividade de integração não é valida, pois tem a possibilidade de não ser executada"));
            }
            return errors;
        }

        private GetAllElementActivitiesUpToTheStartPointResult GetAllElementActivitiesUpToTheStartPoint(ActivityBase activity, List<ActivityBase> pathElements, List<ActivityBase> elementsVerified)
        {
            if (elementsVerified.Contains(activity))
            {
                return new GetAllElementActivitiesUpToTheStartPointResult(false);
            }
            elementsVerified.Add(activity);
            pathElements.Add(activity);

            if (activity is StartEvent)
            {
                return new GetAllElementActivitiesUpToTheStartPointResult(pathElements);
            }

            if ((activity.Incoming?.Count ?? 0) == 0)
            {
                return new GetAllElementActivitiesUpToTheStartPointResult(false);
            }

            foreach (var incoming in activity.GetPreviousTaskPossible())
            {
                var pathElementsCloned = new List<ActivityBase>(pathElements);
                var result = GetAllElementActivitiesUpToTheStartPoint(incoming, pathElementsCloned, elementsVerified);
                if (result.FoundStartPoint)
                {
                    return result;
                }
            }
            return new GetAllElementActivitiesUpToTheStartPointResult(false);
        }


        private class GetAllElementActivitiesUpToTheStartPointResult
        {
            public GetAllElementActivitiesUpToTheStartPointResult(bool foundStartPoint)
            {
                FoundStartPoint = foundStartPoint;
            }

            public GetAllElementActivitiesUpToTheStartPointResult(List<ActivityBase> pathElements)
            {
                FoundStartPoint = true;
                PathElements = pathElements;
            }

            public bool FoundStartPoint { get; set; }
            public List<ActivityBase> PathElements { get; set; }
        }
    }
}
