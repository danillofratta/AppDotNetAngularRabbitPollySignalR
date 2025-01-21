using SharedDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDatabase.Dto
{
    public class OrderDto : SharedDatabase.Models.Order
    {
        public string namestatus { get; set; }
        public string nameproduct { get; set; }
    }
}
