using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyConnectApp.Models
{
    public class VisitorRequest
    {
        [JsonProperty("visitor_name")]
        public string visitor_name { get; set; }

        [JsonProperty("student_number")]
        public string student_number { get; set; }

        [JsonProperty("purpose")]
        public string purpose { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        public VisitorRequest()
        {
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override string ToString()
        {
            return $"Visitor: {visitor_name}, Student#: {student_number}, Purpose: {purpose}, Time: {timestamp}";
        }
    }
}

