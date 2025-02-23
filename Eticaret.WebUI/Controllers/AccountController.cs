using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Eticaret.WebUI.ExtensionMethods;
using Eticaret.WebUI.Models;
using Eticaret.WebUI.Utils;
using Microsoft.AspNetCore.Authentication; //login için gerekli kütüphane
using Microsoft.AspNetCore.Authorization; //login için gerekli kütüphane
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims; //login için gerekli kütüphane

namespace Eticaret.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IService<AppUser> _service;
        private readonly IService<Order> _serviceOrder;

        public AccountController(IService<AppUser> service, IService<Order> serviceOrder)
        {
            _service = service;
            _serviceOrder = serviceOrder;

        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (user is null)
            {
                return NotFound();
            }
            var model = new UserEditViewModel()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Password = user.Password,
                Phone = user.Phone,
                Surname = user.Surname
            };
            return View(model);
        }
        //Index Post
        [HttpPost, Authorize]
        public async Task<IActionResult> IndexAsync(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
                    if (user is not null)
                    {
                        user.Surname = model.Surname;
                        user.Password = model.Password;
                        user.Phone = model.Phone;
                        user.Email = model.Email;
                        user.Name = model.Name;
                        _service.Update(user);
                        var sonuc = _service.SaveChanges();
                        if (sonuc > 0)
                        {
                            TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Hesap Bilgileriniz Başarıyla Güncellenmiştir</strong> 
                        <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
                        </div>";
                            // await MailHelper.SendMailAsync(contact); //Mail gönderme MailHelper.cs
                            return RedirectToAction("Index");
                        }
                    }

                }
                catch (Exception)
                {

                    ModelState.AddModelError("", "Beklenmeyen Bir Hata Oluştu!");
                }
            }
            return View(model);
        }

        //Siparişlerim
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (user is null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn");
            }
            var model = _serviceOrder.GetQueryable().Where(s => s.AppUserId == user.Id).Include(o => o.OrderLines).ThenInclude(p => p.Product);
            return View(model);
        }

        //Login Get
        public IActionResult SignIn()
        {
            return View();
        }
        //Login Post
        [HttpPost]
        public async Task<IActionResult> SignInAsync(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _service.GetAsync(s => s.Email == loginViewModel.Email && s.Password == loginViewModel.Password && s.IsActive);
                    if (account == null)
                    {
                        ModelState.AddModelError("", "Giriş Başarısız!");
                    }
                    else
                    {
                        HttpContext.Session.SetJson("User", account.Name);
                        var claims = new List<Claim>()
                        {
                            new(ClaimTypes.Name, account.Name),
                            new(ClaimTypes.Role, account.IsAdmin ? "Admin" : "Customer"),
                            new(ClaimTypes.Email, account.Email),
                            new("UserId", account.Id.ToString()),
                            new("UserGuid", account.UserGuid.ToString())
                        };
                        var userIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(userPrincipal);
                        return Redirect(string.IsNullOrEmpty(loginViewModel.ReturnUrl) ? "/" : loginViewModel.ReturnUrl);
                    }
                }
                catch (Exception hata)
                {

                    ModelState.AddModelError("", "Beklenmeyen Bir Hata Oluştu!");
                }
            }
            return View(loginViewModel);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUpAsync(AppUser appUser)
        {
            appUser.IsAdmin = false;
            appUser.IsActive = true;
            appUser.UserGuid = Guid.NewGuid();
            appUser.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                await _service.AddAsync(appUser);
                await _service.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }
        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetJson("User", "");

            CartService cart = new CartService();
            HttpContext.Session.SetJson("Cart", cart);

            List<Product> product = new List<Product>();
            HttpContext.Session.SetJson("GetFavorites", product);

            return RedirectToAction("SignIn");
        }
        public IActionResult PasswordRenew()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordRenew(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("", "Lütfen geçerli Email adresi giriniz!");
                return View();
            }
            AppUser user = await _service.GetAsync(x => x.Email == Email);
            if (user is null)
            {
                ModelState.AddModelError("", "Lütfen geçerli Email adresi giriniz!");
                return View();
            }
            string mesaj = $"Sayın {user.Name} {user.Surname} <hr /> Şifrenizi Yenilemek İçin Lütfen <a href='https://localhost:7054/Account/PasswordReset?user={user.UserGuid.ToString()}'>Buraya Tıklayanız</a>";
            var sonuc = await MailHelper.SendMailAsync(Email, "Şifre Yenileme", mesaj);
            if (sonuc)
            {
                TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Şifre Resetleme Linki Mail Adresinize Gönderilmiştir.</strong> 
                        <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
                        </div>";
            }
            else
            {
                TempData["Message"] = @"<div class=""alert alert-danger alert-dismissible fade show"" role=""alert"">
                        <strong>Şifre Resetleme Linki Mail Adresinize Gönderilirken Bir Hata Oluştu!</strong> 
                        <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
                        </div>";
            }
            return View();
        }
        //Şifre Yenileme Sayfasında User Kontrolü
        public async Task<IActionResult> PasswordResetAsync(string user)
        {
            if (user is null)
            {
                return BadRequest("Geçersiz Talep!");
            }
            AppUser appUser = await _service.GetAsync(x => x.UserGuid.ToString() == user);
            if (appUser is null)
            {
                return NotFound("Geçersiz Değer!");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordReset(string user, string Password)
        {
            if (user is null)
            {
                return BadRequest("Geçersiz Talep!");
            }
            AppUser appUser = await _service.GetAsync(x => x.UserGuid.ToString() == user);
            if (appUser is null)
            {
                ModelState.AddModelError("", "Geçersiz Değer!");
                return View();
            }
            appUser.Password = Password;
            var sonuc = await _service.SaveChangesAsync();
            if (sonuc > 0)
            {
                TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Şifreniz Güncellenmiştir.</strong> 
                        <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
                        </div>";
            }
            else 
            {
                ModelState.AddModelError("", "Şifre Güncelleme Başarısız!");
            }
            return View();
        }
    }
}
