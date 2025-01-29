using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDatabase.Dto
{
    public class SaleDto : SharedDatabase.Models.Sale
    {
        public string namestatus { get; set; }
        public string nameproduct { get; set; }
    }
}
