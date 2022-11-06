using BookStore.DAL.Data;
using BookStore.DAL.Repository.IRepository;
using BookStore.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repository
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private readonly ApplicationDbContext _db;
		public ShoppingCartRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		private void Save()
		{
			_db.SaveChanges();
		}

		public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
			Save();
			return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
			shoppingCart.Count += count;
			Save();
			return shoppingCart.Count;
		}

        
	}
}
