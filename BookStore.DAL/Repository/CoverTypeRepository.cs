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
	public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
	{
		private readonly ApplicationDbContext _db;
		public CoverTypeRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
		private void Save()
		{
			_db.SaveChanges();
		}
		public void Update(CoverType obj)
		{
			table.Update(obj);
			Save();
		}
	}
}
