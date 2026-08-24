using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class AuditLogger
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public Event Event { get; set; }

        public DateTime TimeOfEvent { get; set; }

        public string Details { get; set; }

        public string RecordId { get; set; }
    }
}