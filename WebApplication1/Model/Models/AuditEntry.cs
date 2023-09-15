using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class AuditEntry
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string UserName { get; set; }
        public string? EntityIdentifier { get; set; }
    }

}
