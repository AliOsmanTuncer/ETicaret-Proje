using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IService<Category> _service;

        public CategoriesController(IService<Category> service)
        {
            _service = service;
        }

        public async Task<IActionResult> IndexAsync(int? id, string sort)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _service.GetQueryable().Include(p => p.Products).FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            //Ürün Filtre

            switch (sort)
            {
                case "price_asc":
                    category.Products = category.Products.OrderBy(p => p.Price).ToList();
                    break;
                case "price_desc":
                    category.Products = category.Products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "name_asc":
                    category.Products = category.Products.OrderBy(p => p.Name).ToList();
                    break;
                case "name_desc":
                    category.Products = category.Products.OrderByDescending(p => p.Name).ToList();
                    break;
            }

            return View(category);
        }
    }
}
