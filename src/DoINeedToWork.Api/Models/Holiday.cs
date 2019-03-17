using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoINeedToWork.Api.Models
{
    public class Holiday
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }  //Province.ToString
        public string Type { get; set; }   //HolidayType.ToString

    }

    public enum HolidayType
    {
        Statutory,
        Optional
    }

    public enum Province
    {
        Alberta
    }
}
