using CabManagementSystem.Models;

namespace CabManagementSystem.Services
{
    public interface IInvoicePdfService
    {
        byte[] GenerateInvoice(Billing billing);
    }
}
