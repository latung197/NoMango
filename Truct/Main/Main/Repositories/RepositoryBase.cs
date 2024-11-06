using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace Main.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _ConnectionString;
        public RepositoryBase() 
        {
            _ConnectionString = "Server=DESKTOP-6S7C1KO; Database=MVVMLoginDb; Integrated Security=true";
        }
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_ConnectionString);
        }
    }
}
