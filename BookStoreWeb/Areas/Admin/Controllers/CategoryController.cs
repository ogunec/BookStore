using BookStore.DAL.Data;
using BookStore.DAL.Repository.IRepository;
using BookStore.MODEL;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public CategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			IEnumerable<Category> categoryList = _unitOfWork.Category.GetAll();
			return View(categoryList);
		}
		//GET
		public IActionResult Create()
		{

			return View();
		}
		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Category obj)
		{
			if (obj.Name == obj.DisplayOrder.ToString())
			{
				ModelState.AddModelError("CustomError", "The Name cannot be same with Display Order");
			}
			if (ModelState.IsValid)
			{
				_unitOfWork.Category.Add(obj);
				TempData["success"] = "Category created succesfully";
				return RedirectToAction("Index", "Category");
			}
			return View(obj);
		}
		//GET
		public IActionResult Edit(int? id)
		{
			var CategoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
			if (CategoryFromDb == null)
			{
				return NotFound();
			}
			return View(CategoryFromDb);
		}
		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Category obj)
		{
			if (obj.Name == obj.DisplayOrder.ToString())
			{
				ModelState.AddModelError("CustomError", "The Name cannot be same with Display Order");
			}
			if (ModelState.IsValid)
			{
				_unitOfWork.Category.Update(obj);

				TempData["success"] = "Category updated succesfully";
				return RedirectToAction("Index", "Category");
			}
			return View(obj);
		}
		//GET
		public IActionResult Delete(int? id)
		{
			var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
			if (obj == null)
			{
				return NotFound();
			}
			return View(obj);
		}
		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(Category obj)
		{
			_unitOfWork.Category.Remove(obj);
			TempData["success"] = "Category removed succesfully";
			return RedirectToAction("Index", "Category");
		}
	}
}
