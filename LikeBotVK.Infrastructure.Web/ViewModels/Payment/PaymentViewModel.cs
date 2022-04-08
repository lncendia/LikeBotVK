using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Infrastructure.Web.ViewModels.Payment;

public class PaymentViewModel
{
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Идентификатор пользователя")]
    public long UserId { get; set; }

    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Дата платежа")]
    [DataType(DataType.DateTime)]
    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Стоимость")]
    [DataType(DataType.Currency)]
    public decimal Cost { get; set; }
}