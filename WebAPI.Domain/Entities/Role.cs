using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Domain.Entities
{
    public class Role : BaseEntity 
    {
        public string RoleCode { get; set; }
        public string  RoleName { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
