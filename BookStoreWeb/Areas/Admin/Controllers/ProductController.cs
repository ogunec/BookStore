using BookStore.DAL.Repository.IRepository;
using BookStore.MODEL;
using BookStore.MODEL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {

            return View();
        }
        //GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            #region using SelectList
            /*IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            IEnumerable<SelectListItem> CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            */
            #endregion
            if (id == null || id == 0)
            {
                //create product
                //ViewBag.CategoryList = CategoryList;
                //ViewBag.CoverTypeList = CoverTypeList;
                return View(productVM);
            }
            else
            {
                //update product
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                return View(productVM);

            }


        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath; //statik dosyalarımızın yer aldığı wwwroot konumunu elde etmek için.
                if (file != null) //Create ve Update yaparken resim upload edilirse buraya girecek.
                {
                    string fileName = Guid.NewGuid().ToString(); //Yüklenen resimlerde isim çakışmasının önüne geçmek için benzersiz isim oluşturma
                    var uploads = Path.Combine(wwwRootPath, @"images\products"); //dosyanın upload edilecek klasör konumu
                    var extension = Path.GetExtension(file.FileName); //Yüklenen dosya uzantısını çekmek için



                    if (obj.Product.ImageUrl != null) //gönderilen objenin içerisinde halihazırda bir imageurl varsa bu bir upload işlemi olacaktır. Eski resmi silmek için buraya girilir.
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\')); //silinecek resmin dosya yolunu çekeriz.
                        if (System.IO.File.Exists(oldImagePath)) //bu dosya wwwroot içerisinde varsa bu bloğa girecektir.
                        {
                            System.IO.File.Delete(oldImagePath); //silme işlemini gerçekleştiren komut.
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) //yeni upload edilen dosyanın oluşturulması
                    {
                        file.CopyTo(fileStreams); //bu kod ile wwwroot/images/products içerisine gönderilir.
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension; //en son gönderilen bu resim objemizin imageurl propertysi içerisine atılır.

                }

                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product created succesfully";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated succesfully";
                }
                return RedirectToAction("Index", "Product");
            }
            return View(obj);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }

        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\')); //silinecek resmin dosya yolunu çekeriz.
            if (System.IO.File.Exists(oldImagePath)) //bu dosya wwwroot içerisinde varsa bu bloğa girecektir.
            {
                System.IO.File.Delete(oldImagePath); //silme işlemini gerçekleştiren komut.
            }

            _unitOfWork.Product.Remove(obj);
            return Json(new { success = true, message = "Deleted succesfully" });
        }
        #endregion


    }
}
