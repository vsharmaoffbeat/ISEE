using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISEEDataModel.Repository.Services
{
    public class UserFactory
    {

        //public UserFactory()
        //    : base("name=ISEEEntities")
        //{
        //}
        //public UserFactory _context;
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    throw new UnintentionalCodeFirstException();
        //}

        //public virtual DbSet<Factory> User { get; set; }
        private ISEEEntities _context;
        public Factory GetUserById(string userName, string password)
        {
            _context = new ISEEEntities();
            return _context.Factories.Where(u => u.UserName == userName && u.Password == password).SingleOrDefault();
            //         return _context.Factory.Where(u => u.UserName == userName && u.Password == password).SingleOrDefault();
            //return _context.User.Where(u => u. == Id).SingleOrDefault();
        }
         
    }
}
