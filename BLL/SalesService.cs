using POS.DAL;
using POS.MODEL;

namespace POS.BLL
{
    public class SalesService
    {
        private readonly SalesRepo _salesRepo;
        private readonly ProductRepo _productRepo; // Dùng để kiểm tra tồn kho

        public SalesService()
        {
            _salesRepo = new SalesRepo();
            _productRepo = new ProductRepo();
        }

        public int CreateSale(Invoice invoice, List<CartItem> cart)
        {
            // Validation logic (Rất quan trọng)
            if (cart == null || cart.Count == 0)
            {
                throw new Exception("Giỏ hàng rỗng. Không thể tạo hóa đơn.");
            }

            // 1. Tính toán lại tổng tiền
            decimal totalAmount = cart.Sum(item => item.LineTotal);
            invoice.TotalAmount = totalAmount;
            // (Chưa có giảm giá, nên FinalAmount = TotalAmount)
            invoice.FinalAmount = totalAmount;
            invoice.InvoiceDate = DateTime.Now;

            // 2. Kiểm tra tồn kho trước khi lưu
            foreach (var item in cart)
            {
                Product productInDb = _productRepo.GetProductById(item.ProductId);
                if (productInDb == null)
                {
                    throw new Exception($"Sản phẩm '{item.ProductName}' không còn tồn tại.");
                }
                if (productInDb.Quantity < item.Quantity)
                {
                    throw new Exception($"Không đủ hàng tồn kho cho '{item.ProductName}'. " +
                                      $"Trong kho chỉ còn: {productInDb.Quantity}");
                }
            }

            // 3. Nếu mọi thứ OK, gọi Repo để lưu (Repo sẽ dùng Transaction)
            try
            {
                return _salesRepo.SaveInvoice(invoice, cart);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi nghiêm trọng khi lưu hóa đơn: \n" + ex.Message);
            }
        }
    }
}