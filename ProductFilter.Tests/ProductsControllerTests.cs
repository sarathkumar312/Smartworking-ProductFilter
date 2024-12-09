using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using ProductFilter.Interfaces;
using ProductFilter.Entities;
using ProductFilter.Controllers;

namespace ProductFilter.Tests;

[TestFixture]
public class ProductsControllerTests
{
	private Mock<IProductService> _mockService;
	private ProductsController _controller;

	[SetUp]
	public void Setup()
	{
		_mockService = new Mock<IProductService>();
		_controller = new ProductsController(_mockService.Object);
	}

	[Test]
	public async Task FilterProducts_ReturnsExpectedResult()
	{
		// Arrange
		var expectedResponse = new FilterResponse
		{
			Products = new List<Product>
			{
				new Product { Name = "Test Product", Description = "Test Description", Price = 10, Size = "M" }
			},
			Filter = new FilterInfo
			{
				MinPrice = 10,
				MaxPrice = 10,
				Sizes = new[] { "M" },
				TopWords = new[] { "Test" }
			}
		};

		_mockService
			.Setup(s => s.GetFilteredProducts(null, null, null, new string[0]))
			.ReturnsAsync(expectedResponse);

		// Act
		var result = await _controller.FilterProducts(null, null, null, null) as OkObjectResult;

		// Assert
		Assert.That(result, Is.Not.Null, "Result should not be null.");
		Assert.That(result.Value, Is.InstanceOf<FilterResponse>(), "Result should be of type FilterResponse.");

		var actualResponse = result.Value as FilterResponse;
		Assert.That(actualResponse, Is.Not.Null, "Actual response should not be null.");

		var productsList = actualResponse.Products.ToList();
		Assert.That(productsList.Count, Is.EqualTo(1), "Expected one product in the response.");
		Assert.That(productsList[0].Name, Is.EqualTo("Test Product"), "Product name mismatch.");
		Assert.That(actualResponse.Filter.MinPrice, Is.EqualTo(10), "MinPrice mismatch.");
		Assert.That(actualResponse.Filter.MaxPrice, Is.EqualTo(10), "MaxPrice mismatch.");
	}
}
