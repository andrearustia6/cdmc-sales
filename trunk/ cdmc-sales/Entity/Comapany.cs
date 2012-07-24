using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class Comapany:EntityBase
    {
        public string ComapanyTitle { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public List<Client> Clients;
    }
}
