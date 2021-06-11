using EF.Sharding.MulitiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Sharding.Models
{
    public class Artist : IMulitiTenancy
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string TenantId { get; set; }
    }
}
