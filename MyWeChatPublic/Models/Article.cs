using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeChatPublic.Models
{
    public class Article
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 发布日期，格式为：20150722
        /// </summary>
        [Required]
        [StringLength(8)]
        [Index]
        public string Published { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string PicUrl { get; set; }

        [Required]
        public string Url { get; set; }

        /// <summary>
        /// 标签，格式为：aspnet visualstudio
        /// </summary>
        [Required]
        [StringLength(256)]
        [Index]
        public string Tags { get; set; }
    }
}
