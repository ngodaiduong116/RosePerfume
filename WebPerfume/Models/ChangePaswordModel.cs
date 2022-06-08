using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPerfume.Models
{
    public class ChangePaswordModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CurrentPaswordId { get; set; }
        public string NewPaswordId { get; set; }
        public string CfPaswordId { get; set; }
    }
}