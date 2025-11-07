using POS.BLL;
using POS.MODEL;
using System.ComponentModel;

namespace POS.UI
{
    public partial class SalesForm : Form
    {
        private readonly User _currentUser;
        private readonly ProductService _productService;
        private readonly SalesService _salesService;
        private BindingList<CartItem> _cart; // Dùng BindingList để DGV tự cập nhật

        public SalesForm(User user)
        {
            InitializeComponent();

            _currentUser = user;
            _productService = new ProductService();
            _salesService = new SalesService();
            _cart = new BindingList<CartItem>();
        }

        private void SalesForm_Load(object sender, EventArgs e)
        {
            lblStaffName.Text = _currentUser.FullName;
            SetupCartGrid();
            SetupSearchGrid();
            ClearSale();
        }

        private void SetupCartGrid()
        {
            dgvCart.DataSource = _cart;
            dgvCart.AutoGenerateColumns = false;

            if (dgvCart.Columns.Count == 0)
            {
                dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "ID", Width = 60 });
                dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Tên Sản Phẩm", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SellingPrice", HeaderText = "Đơn Giá", Width = 100, DefaultCellStyle = { Format = "N0" } });
                dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "SL", Width = 60 });
                dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LineTotal", HeaderText = "Thành Tiền", Width = 120, DefaultCellStyle = { Format = "N0" }, ReadOnly = true });
            }
        }

        private void SetupSearchGrid()
        {
            dgvSearchResults.AutoGenerateColumns = false;
            if (dgvSearchResults.Columns.Count == 0)
            {
                dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "ID", Width = 60 });
                dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Tên Sản Phẩm", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SellingPrice", HeaderText = "Giá Bán", Width = 100, DefaultCellStyle = { Format = "N0" } });
                dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Tồn Kho", Width = 70 });
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                List<Product> results = _productService.GetProductsForSale(searchTerm);
                dgvSearchResults.DataSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(sender, e);
                e.SuppressKeyPress = true; // Ngăn tiếng "beep"
            }
        }

        private void dgvSearchResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var product = (Product)dgvSearchResults.Rows[e.RowIndex].DataBoundItem;
                if (product != null)
                {
                    AddProductToCart(product);
                }
            }
        }

        private void AddProductToCart(Product product)
        {
            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            CartItem existingItem = _cart.FirstOrDefault(item => item.ProductId == product.ProductId);

            if (existingItem != null)
            {
                // Nếu đã có, tăng số lượng
                existingItem.Quantity++;
            }
            else
            {
                // Nếu chưa có, thêm mới với số lượng 1
                _cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    SellingPrice = product.SellingPrice,
                    Quantity = 1
                });
            }

            // Reset BindingList để DGV cập nhật
            _cart.ResetBindings();
            UpdateTotal();
        }

        private void dgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Khi người dùng sửa số lượng trong DGV
            if (e.ColumnIndex == dgvCart.Columns["Quantity"]?.Index)
            {
                try
                {
                    CartItem item = _cart[e.RowIndex];
                    if (item.Quantity <= 0)
                    {
                        // Nếu sửa thành 0, xóa khỏi giỏ hàng
                        _cart.Remove(item);
                    }
                }
                catch (Exception)
                {
                    // (Xử lý nếu nhập chữ) - DGV thường tự xử lý
                }
            }
            _cart.ResetBindings();
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = _cart.Sum(item => item.LineTotal);
            lblTotal.Text = total.ToString("N0") + " VND";
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xóa toàn bộ giỏ hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ClearSale();
            }
        }

        private void ClearSale()
        {
            _cart.Clear();
            txtCustomerName.Clear();
            txtSearch.Clear();
            dgvSearchResults.DataSource = null;
            UpdateTotal();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang rỗng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Xác nhận thanh toán
            DialogResult confirm = MessageBox.Show($"Tổng cộng: {lblTotal.Text}\n\nXác nhận thanh toán hóa đơn này?", "Xác nhận Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.No)
            {
                return;
            }

            // Tạo đối tượng Invoice
            Invoice newInvoice = new Invoice
            {
                UserId = _currentUser.UserId,
                CustomerName = txtCustomerName.Text.Trim()
                // TotalAmount và FinalAmount sẽ được Service tính toán
            };

            try
            {
                int newInvoiceId = _salesService.CreateSale(newInvoice, _cart.ToList());

                MessageBox.Show($"Thanh toán thành công!\nSố hóa đơn: {newInvoiceId}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Xử lý in hóa đơn (Tương lai)
                // ... PrintInvoice(newInvoiceId) ...

                ClearSale();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thanh toán: \n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCart_KeyDown(object sender, KeyEventArgs e)
        {
            // Cho phép xóa bằng nút Delete
            if (e.KeyCode == Keys.Delete)
            {
                if (dgvCart.CurrentRow != null && dgvCart.CurrentRow.Index >= 0)
                {
                    CartItem item = _cart[dgvCart.CurrentRow.Index];
                    if (MessageBox.Show($"Bạn có muốn xóa '{item.ProductName}' khỏi giỏ hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _cart.Remove(item);
                        _cart.ResetBindings();
                        UpdateTotal();
                    }
                }
            }
        }
    }
}