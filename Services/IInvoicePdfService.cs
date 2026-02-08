using Cab_Management_System.Models;

namespace Cab_Management_System.Services
{
    public interface IInvoicePdfService
    {
        byte[] GenerateInvoice(Billing billing);
    }
}
