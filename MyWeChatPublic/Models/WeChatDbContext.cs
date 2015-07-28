using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeChatPublic.Models
{
    public class WeChatDbContext:DbContext
    {
        public WeChatDbContext()
#if !Production
            :base("wechat_sqlcompactdb")
#else
            : base("wechat_sqldb")
#endif
        {
            
        }

        public DbSet<Article> Articles { get; set; }
    }
}
