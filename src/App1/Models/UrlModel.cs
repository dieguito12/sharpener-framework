using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    [Table("urls")]
    public class UrlModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("short")]
        public string Short{ get; set;}

        [Column("clicks")]
        public int Clicks { get; set; }

        [Column("last_click")]
        public DateTime LastClick { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        public virtual List<RefererModel> Referers { get; set; }

        public virtual List<AgentModel> Agents { get; set; }

        public virtual List<PlatformModel> Platforms { get; set; }

        public virtual List<LocationModel> Locations { get; set; }

        public virtual UserModel User { get; set; }
    }
}
