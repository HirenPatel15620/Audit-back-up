using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class ChangeAuditAddress 
{
    public long Id { get; set; }
    public string? EntityName { get; set; }
    public string? Action { get; set; }
    public string? PropertyName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? UserName { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? EntityIdentifier { get; set; }
}
