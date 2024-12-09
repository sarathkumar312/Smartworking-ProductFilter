using Microsoft.AspNetCore.Mvc;
using ProductFilter.Interfaces;

namespace ProductFilter.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
	private readonly IProductService _productService;

	public ProductsController(IProductService productService)
	{
		_productService = productService;
	}

	[HttpGet("filter")]
	public async Task<IActionResult> FilterProducts([FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
		[FromQuery] string? size, [FromQuery] string? highlight)
	{
		var highlights = highlight?.Split(",") ?? Array.Empty<string>();
		var result = await _productService.GetFilteredProducts(minPrice, maxPrice, size, highlights);
		return Ok(result);
	}
}
