using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zone7systems.com.Models
{
    public class Career
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public byte[] ResumeCV { get; set; }
    }
}