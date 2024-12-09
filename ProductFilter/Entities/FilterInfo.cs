namespace ProductFilter.Entities
{
	public class FilterInfo
	{
		public decimal MinPrice { get; set; }
		public decimal MaxPrice { get; set; }
		public IEnumerable<string> Sizes { get; set; }
		public IEnumerable<string> TopWords { get; set; }
	}
}
