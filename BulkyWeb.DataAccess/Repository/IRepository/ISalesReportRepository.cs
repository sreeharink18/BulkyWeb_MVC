using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository.IRepository
{
    public interface ISalesReportRepository : IRepository<SalesReport>
    {
        void Update(SalesReport obj); 
    }
}
