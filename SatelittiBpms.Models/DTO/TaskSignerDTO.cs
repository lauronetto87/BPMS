using System;

namespace SatelittiBpms.Models.DTO
{
    public class TaskSignerDTO
    {
        public int Id { get; set; }
        public DateTime DateSendEvelope { get; set; }
        public int EnvelopeId { get; set; }
    }
}
