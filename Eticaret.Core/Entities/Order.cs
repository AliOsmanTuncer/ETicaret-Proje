using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Order : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Sipariş No"), StringLength(50)]
        public string OrderNumber { get; set; }
        [Display(Name = "Sipariş Toplamı")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Müşteri No")]
        public int AppUserId { get; set; }
        [Display(Name = "Müşteri Id"), StringLength(50)]
        public string CustomerId { get; set; }
        [Display(Name = "Fatura Adresi"), StringLength(200)]
        public string BillingAddress { get; set; }
        [Display(Name = "Teslimat Adresi"), StringLength(200)]
        public string DeliveryAddress { get; set; }
        [Display(Name = "Sipariş Tarihi")]
        public DateTime OrderDate { get; set; } //= DateTime.Now;
        public List<OrderLine>? OrderLines { get; set; }
        [Display(Name = "Müşteri Adı")]
        public AppUser? AppUser { get; set; }
        [Display(Name = "Sipariş Durumu")]
        public EnumOrderState OrderState { get; set; }

    }
    public enum EnumOrderState
    {
        [Display(Name = "Onay Bekliyor")]
        Waiting,

        [Display(Name = "Ödeme Bekleniyor")]
        PaymentPending,

        [Display(Name = "Ödeme Alındı")]
        PaymentReceived,

        [Display(Name = "Onaylandı")]
        Approved,

        [Display(Name = "Hazırlanıyor")]
        Processing,

        [Display(Name = "Kargoya Hazırlanıyor")]
        Packing,

        [Display(Name = "Kargolandı")]
        Shipped,

        [Display(Name = "Dağıtımda")]
        OutForDelivery,

        [Display(Name = "Teslim Edildi")]
        Delivered,

        [Display(Name = "Tamamlandı")]
        Completed,

        [Display(Name = "İptal Edildi")]
        Cancelled,

        [Display(Name = "İptal Edildi - Ödeme İadesi Bekleniyor")]
        CancelledRefundPending,

        [Display(Name = "İade Edildi")]
        Returned,

        [Display(Name = "İade Sürecinde")]
        ReturnProcessing,

        [Display(Name = "İade Reddedildi")]
        ReturnRejected
    }

}
