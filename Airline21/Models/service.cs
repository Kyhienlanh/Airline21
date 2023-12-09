using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airline21.Models
{
    public class service
    {
        public service() { }
        public service(int id, string name, int extraluggage,string securityService, int totalServicePeople)
        {
            Id = id;
            Name = name;
            this.extraluggage = extraluggage;
            this.securityService = securityService;
            TotalServicePeople = totalServicePeople;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int extraluggage { get; set; }
        public string securityService {  get; set; }
        public int TotalServicePeople {  get; set; }
    }
}