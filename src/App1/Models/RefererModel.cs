﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    [Table("referers")]
    public class RefererModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("url_id")]
        public int UrlId { get; set; }

        [Column("referer")]
        public string Referer { get; set; }

        public virtual UrlModel Url { get; set; }
    }
}
