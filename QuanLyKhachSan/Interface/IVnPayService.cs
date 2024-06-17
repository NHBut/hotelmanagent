using QuanLyKhachSan.ViewModel;
using Microsoft.AspNetCore.Http;


namespace QuanLyKhachSan.Interface
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(System.Web.HttpContextBase context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}