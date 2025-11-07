using Microsoft.Data.SqlClient;
using POS.MODEL;
using POS.UTIL;

namespace POS.DAL
{
    public class SalesRepo
    {
        /// <summary>
        /// Lưu hóa đơn và chi tiết hóa đơn vào CSDL bằng Transaction
        /// </summary>
        public int SaveInvoice(Invoice invoice, List<CartItem> cart)
        {
            // Sử dụng transaction để đảm bảo tất cả các lệnh được thực thi
            // hoặc không lệnh nào được thực thi
            using (SqlConnection connection = DBConnection.GetConnection())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Bước 1: Thêm hóa đơn (Invoices)
                    string sqlInvoice = @"
                        INSERT INTO Invoices (InvoiceDate, UserId, CustomerName, TotalAmount, FinalAmount)
                        VALUES (@InvoiceDate, @UserId, @CustomerName, @TotalAmount, @FinalAmount);
                        SELECT SCOPE_IDENTITY();"; // Lấy ID vừa chèn

                    int invoiceId;
                    using (SqlCommand cmdInvoice = new SqlCommand(sqlInvoice, connection, transaction))
                    {
                        cmdInvoice.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
                        cmdInvoice.Parameters.AddWithValue("@UserId", invoice.UserId);
                        cmdInvoice.Parameters.AddWithValue("@CustomerName", (object)invoice.CustomerName ?? DBNull.Value);
                        cmdInvoice.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                        cmdInvoice.Parameters.AddWithValue("@FinalAmount", invoice.FinalAmount);

                        // ExecuteScalar để lấy ID trả về
                        invoiceId = Convert.ToInt32(cmdInvoice.ExecuteScalar());
                    }

                    if (invoiceId <= 0)
                    {
                        throw new Exception("Không thể tạo hóa đơn.");
                    }

                    // Bước 2: Thêm chi tiết hóa đơn (InvoiceDetail)
                    string sqlDetail = @"
                        INSERT INTO InvoiceDetail (InvoiceId, ProductId, Quantity, UnitPrice, LineTotal)
                        VALUES (@InvoiceId, @ProductId, @Quantity, @UnitPrice, @LineTotal);";

                    using (SqlCommand cmdDetail = new SqlCommand(sqlDetail, connection, transaction))
                    {
                        foreach (var item in cart)
                        {
                            cmdDetail.Parameters.Clear();
                            cmdDetail.Parameters.AddWithValue("@InvoiceId", invoiceId);
                            cmdDetail.Parameters.AddWithValue("@ProductId", item.ProductId);
                            cmdDetail.Parameters.AddWithValue("@Quantity", item.Quantity);
                            cmdDetail.Parameters.AddWithValue("@UnitPrice", item.SellingPrice);
                            cmdDetail.Parameters.AddWithValue("@LineTotal", item.LineTotal);
                            cmdDetail.ExecuteNonQuery();
                        }
                    }

                    // Bước 3: Cập nhật tồn kho (Products)
                    string sqlUpdateStock = @"
                        UPDATE Products SET Quantity = Quantity - @SoldQuantity
                        WHERE ProductId = @ProductId";

                    using (SqlCommand cmdUpdateStock = new SqlCommand(sqlUpdateStock, connection, transaction))
                    {
                        foreach (var item in cart)
                        {
                            cmdUpdateStock.Parameters.Clear();
                            cmdUpdateStock.Parameters.AddWithValue("@SoldQuantity", item.Quantity);
                            cmdUpdateStock.Parameters.AddWithValue("@ProductId", item.ProductId);
                            cmdUpdateStock.ExecuteNonQuery();
                        }
                    }

                    // Nếu mọi thứ thành công, commit transaction
                    transaction.Commit();
                    return invoiceId;
                }
                catch (Exception)
                {
                    // Nếu có lỗi, rollback
                    transaction.Rollback();
                    throw; // Ném lỗi ra ngoài để BLL xử lý
                }
            }
        }
    }
}