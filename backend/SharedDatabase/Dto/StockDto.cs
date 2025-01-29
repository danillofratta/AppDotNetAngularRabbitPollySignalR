using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDatabase.Dto
{
    public class StockDto 
    {
        public int id { get; set; }
        public int idproduct { get; set; }
        public string nameproduct { get; set; }

        public int amount { get; set; }
        public decimal price { get; set; }
    }
}
