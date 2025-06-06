﻿namespace TaskFlowAPI.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime CreatedAtUtc { get; set; }

        DateTime? UpdatedAtUtc { get; set; }

        Guid? CreatedById { get; set; }
        Guid? UpdatedById { get; set; }
    }

}
