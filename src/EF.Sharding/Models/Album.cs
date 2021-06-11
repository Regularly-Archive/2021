using EF.Sharding.MulitiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Sharding.Models
{
    public class Album : IMulitiTenancy
    {
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public string TenantId { get; set; }
    }
}
