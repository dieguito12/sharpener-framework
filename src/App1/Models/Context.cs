using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Context : DbContext
    {
        public Context() : base(nameOrConnectionString: "UrlShortener") { }

        public Context(DbConnection existingConnection, bool contextOwnsConnection)
          : base(existingConnection, contextOwnsConnection)
        {

        }
        public DbSet<UserModel> Users { get; set; }

        public DbSet<UrlModel> Urls { get; set; }

        public DbSet<RefererModel> Referers { get; set; }

        public DbSet<AgentModel> Agents { get; set; }

        public DbSet<PlatformModel> Platforms { get; set; }

        public DbSet<LocationModel> Locations { get; set; }

    }
}
