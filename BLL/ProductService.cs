using POS.DAL;
using POS.MODEL;

namespace POS.BLL
{
    public class ProductService
    {
        private readonly ProductRepo _productRepo;

        public ProductService()
        {
            _productRepo = new ProductRepo();
        }

        public List<Category> GetCategories()
        {
            return _productRepo.GetCategories();
        }

        public List<Product> GetProducts()
        {
            return _productRepo.GetProducts();
        }

        // THÊM PHƯƠNG THỨC MỚI
        public Product GetProductById(int productId)
        {
            return _productRepo.GetProductById(productId);
        }

        // THÊM PHƯƠNG THỨC MỚI
        public List<Product> GetProductsForSale(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Product>(); // Trả về danh sách rỗng nếu không có gì để tìm
            }
            return _productRepo.GetProductsForSale(searchTerm);
        }

        public void AddProduct(Product product)
        {
            // Thêm các bước kiểm tra logic (validation) ở đây
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new Exception("Tên sản phẩm không được để trống.");
            }
            if (product.SellingPrice < product.CostPrice)
            {
                throw new Exception("Giá bán không được nhỏ hơn giá nhập.");
            }
            _productRepo.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new Exception("Tên sản phẩm không được để trống.");
            }
            if (product.SellingPrice < product.CostPrice)
            {
                throw new Exception("Giá bán không được nhỏ hơn giá nhập.");
            }
            if (product.ProductId <= 0)
            {
                throw new Exception("ID sản phẩm không hợp lệ.");
            }
            _productRepo.UpdateProduct(product);
        }

        public void DeleteProduct(int productId)
        {
            if (productId <= 0)
            {
                throw new Exception("ID sản phẩm không hợp lệ.");
            }
            _productRepo.DeleteProduct(productId);
        }
    }
}