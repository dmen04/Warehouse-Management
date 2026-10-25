using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace warehouse.Models
{
    class ShelfStock
    {
        public int ShelfId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public string ItemName { get; set; }
    }
}
