using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        public readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update(OrderHeader obj)
        {
            _db.orderHeader.Update(obj);
        }

		public void UpdateStatus(int id, string? orderStatus, string? orderPaymentStatus = null)
		{
			var orderHeader = _db.orderHeader.FirstOrDefault(u => u.Id == id);
			if (orderHeader != null)
			{
				orderHeader.OrderStatus = orderStatus;
				if (!string.IsNullOrEmpty(orderPaymentStatus))
				{
					orderHeader.PaymentStatus= orderPaymentStatus;
				}
			}
            

		}

		public void UpdateStripePaymentID(int id, string sessionId, string stripePaymentIntentID)
		{
			var cartfromdb = _db.orderHeader.FirstOrDefault(u=>u.Id == id);
			if (!string.IsNullOrEmpty(sessionId))
			{
				cartfromdb.SessionId = sessionId;
			}

			if (!string.IsNullOrEmpty(stripePaymentIntentID))
			{
				cartfromdb.PaymentIntentId = stripePaymentIntentID;
				cartfromdb.PaymentDate = DateTime.Now;
			}
		}
	}
}
