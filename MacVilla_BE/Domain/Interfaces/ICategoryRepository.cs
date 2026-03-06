using Domain.Entities;

namespace Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(long id);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(long id);
    Task<bool> SetIsActiveAsync(long id, bool isActive);
    Task<(IEnumerable<Category> Categories, int TotalCount)> SearchAsync(string? name, bool? isActive, int pageNumber, int pageSize);

    /// <summary>
    /// Kiểm tra tên danh mục đã tồn tại hay chưa (không phân biệt hoa thường).
    /// </summary>
    /// <param name="categoryName">Tên danh mục cần kiểm tra.</param>
    /// <param name="excludeId">Bỏ qua một ID (dùng cho trường hợp cập nhật).</param>
    Task<bool> ExistsByNameAsync(string categoryName, long? excludeId = null);

    /// <summary>
    /// Kiểm tra danh mục hiện có sản phẩm nào đang gắn với nó hay không.
    /// </summary>
    Task<bool> HasProductsAsync(long categoryId);
}
