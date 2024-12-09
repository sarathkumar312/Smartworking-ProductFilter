using System.IO;

namespace ProductFilter.Entities
{
	public class FilterResponse
	{
		public required IEnumerable<Product> Products { get; set; }
		public FilterInfo Filter { get; set; }
	}
}
