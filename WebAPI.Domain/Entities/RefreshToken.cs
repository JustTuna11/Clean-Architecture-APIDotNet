using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Domain.Entities
{
    public class RefreshToken:BaseEntity
    {
        public string Token { get; set; }
        public long UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime Expirytime { get; set; }

    }
}
