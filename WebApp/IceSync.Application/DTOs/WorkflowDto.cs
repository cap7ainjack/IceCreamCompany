﻿namespace IceSync.Application.DTOs
{
    public class WorkflowDto
    {
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public bool IsActive { get; set; }
        public string MultiExecBehavior { get; set; }
    }
}
