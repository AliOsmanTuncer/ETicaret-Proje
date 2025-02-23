using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    [Authorize]
    public class MyAddressesController : Controller
    {
        private readonly IService<AppUser> _serviceAppUser;
        private readonly IService<Address> _serviceAddress;

        public MyAddressesController(IService<AppUser> service, IService<Address> serviceAddress)
        {
            _serviceAppUser = service;
            _serviceAddress = serviceAddress;
        }
        public async Task<IActionResult> Index()
        {
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return NotFound("Kullanıcı Datası Bulunamadı! Lütfen Yeniden Giriş Yapınız!");
            }
            var model = await _serviceAddress.GetAllAsync(u => u.AppUserId == appUser.Id);
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        // POST: User/Addresses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Address address)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
                    if (appUser != null)
                    {
                        address.AddressGuid= Guid.NewGuid();
                        address.CreateDate = DateTime.Now;
                        address.AppUserId = appUser.Id;
                        _serviceAddress.Add(address);
                        await _serviceAddress.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Beklenmeyen Bir Hata Oluştu!");
                }
                
            }
            ModelState.AddModelError("", "Adres Kaydı Başarısız Oldu!");
            return View(address);
        }

        //Edit Get
        public async Task<IActionResult> Edit(string id)
        {
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return NotFound("Kullanıcı Datası Bulunamadı! Lütfen Yeniden Giriş Yapınız!");
            }
            var model = await _serviceAddress.GetAsync(u => u.AddressGuid.ToString() == id && u.AppUserId == appUser.Id);
            if (model == null)
            {
                return NotFound("Adres Bilgisi Bulunamadı!");
            }
            return View(model);
        }

        //Edit Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Address address)
        {
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return NotFound("Kullanıcı Datası Bulunamadı! Lütfen Yeniden Giriş Yapınız!");
            }
            var model = await _serviceAddress.GetAsync(u => u.AddressGuid.ToString() == id && u.AppUserId == appUser.Id);
            if (model == null)
            {
                return NotFound("Adres Bilgisi Bulunamadı!");
            }
            model.Title = address.Title;
            model.District = address.District;
            model.City = address.City;
            model.OpenAddress = address.OpenAddress;
            model.IsDeliveryAddress = address.IsDeliveryAddress;
            model.IsBillingAddress = address.IsBillingAddress;
            model.IsActive = address.IsActive;
            var otherAddresses = await _serviceAddress.GetAllAsync(x => x.AppUserId == appUser.Id && x.Id != model.Id);

            foreach (var otherAddress in otherAddresses)
            {
                otherAddress.IsBillingAddress = false;
                otherAddress.IsDeliveryAddress = false;
                _serviceAddress.Update(otherAddress);
            }

            try
            {
                _serviceAddress.Update(model);
                await _serviceAddress.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Beklenmeyen Bir Hata Oluştu!");
            }
            return View(model);
        }

        //Delete Get
        public async Task<IActionResult> Delete(string id)
        {
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return NotFound("Kullanıcı Datası Bulunamadı! Lütfen Yeniden Giriş Yapınız!");
            }
            var model = await _serviceAddress.GetAsync(u => u.AddressGuid.ToString() == id && u.AppUserId == appUser.Id);
            if (model == null)
            {
                return NotFound("Adres Bilgisi Bulunamadı!");
            }
            return View(model);
        }

        //Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, Address address)
        {
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return NotFound("Kullanıcı Datası Bulunamadı! Lütfen Yeniden Giriş Yapınız!");
            }
            var model = await _serviceAddress.GetAsync(u => u.AddressGuid.ToString() == id && u.AppUserId == appUser.Id);
            if (model == null)
            {
                return NotFound("Adres Bilgisi Bulunamadı!");
            }
            try
            {
                _serviceAddress.Delete(model);
                await _serviceAddress.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Beklenmeyen Bir Hata Oluştu!");
            }
            return View(model);
        }
    }
}
