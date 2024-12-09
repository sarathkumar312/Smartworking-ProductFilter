using ProductFilter.Entities;

namespace ProductFilter.Interfaces
{
	public interface IProductService
	{
		Task<FilterResponse> GetFilteredProducts(decimal? minPrice, decimal? maxPrice, string? size, string[]? highlight);
	}
}
