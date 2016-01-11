using ClassBooking.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ClassBooking.Database {
    public class GymContext : DbContext {
        //private static string cstr = "provider = System.Data.SQLite.EF6; provider connection string=\"data source =test.db";
        private static string cstr = "GymContext";
        //provider=System.Data.SQLite;provider connection string=""data source = " + System.Environment.CurrentDirectory + @"\LocalData\FD1CBA65-1B68-449F-8E6D-A652137466D4.db"
        public GymContext() : base(cstr) {
            // metadata=res://*/LocalData.MuthootClientContext.csdl|res://*/LocalData.MuthootClientContext.ssdl|res://*/LocalData.MuthootClientContext.msl;provider=System.Data.SQLite;provider connection string=""data source=" + System.Environment.CurrentDirectory + @"\LocalData\FD1CBA65-1B68-449F-8E6D-A652137466D4.db""
            //SQLiteConnectionStringBuilder
        }
        public DbSet<GymClassType> GymClassTypes { get; set; }
        public DbSet<GymClass> GymClass { get; set; }
        public DbSet<GymMember> GymMembers { get; set; }
        public DbSet<GymClassBooking> MemberClassBookings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            // Gym Booking Database does not pluralize table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}