using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IService<Product> _service;

        public ProductsController(IService<Product> service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string q = "")
        {
            var databaseContext = _service.GetAllAsync(p => p.IsActive && (p.Name.Contains(q) || p.Description.Contains(q) || p.ProductCode.Contains(q)));
            return View(await databaseContext);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _service.GetQueryable()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var model = new ProductDetailViewModel()
            {
                Product = product,
                RelatedProducts = _service.GetQueryable().Where(p => p.IsActive && p.CategoryId == product.CategoryId && p.Id!=product.Id)
            };
            return View(model);
        }
    }
}
