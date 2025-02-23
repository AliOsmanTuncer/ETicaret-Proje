using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class AppUser : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Adı")]
        public string Name { get; set; }
        [Display(Name = "Soyadı")]
        public string Surname { get; set; }
        public string Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
        [Display(Name = "Şifre")]
        public string Password { get; set; }
        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; }
        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; }
        [Display(Name = "Admin Mi?")]
        public bool IsAdmin { get; set; }
        //public DateTime CreateDate { get; set; } = DateTime.Now;
        //public Guid? UserGuid { get; set; } = Guid.NewGuid();
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }
        [ScaffoldColumn(false)]
        public Guid? UserGuid { get; set; }
        public List<Address>? Addresses { get; set; }
    }
}
