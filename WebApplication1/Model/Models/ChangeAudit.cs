using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class ChangeAudit
{
    public long Id { get; set; }

    public string? EntityName { get; set; }

    public string? Action { get; set; }

    public string? PropertyName { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? UserName { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? EntityIdentifier { get; set; }
}
