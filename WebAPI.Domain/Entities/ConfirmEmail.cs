using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Domain.Entities
{
    public class ConfirmEmail : BaseEntity
    {
        public string  ComfirmCode { get; set; }
        public long UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime ExpiryTime { get; set; } // thời gian hết hạn
        public bool IsComfirmed { get; set; } = false;

    }
}
