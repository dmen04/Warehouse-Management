using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace warehouse.Models
{
    class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal Volume { get; set; }
        public string Unit { get; set; }

        [JsonIgnore]
        public string CategoryName { get; set; }
    }
}