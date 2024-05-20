using NSubstitute;
using NUnit.Framework;
using static ShoppingCart.Tests.ProductBuilder;

namespace ShoppingCart.Tests;

public class ShoppingCartCheckoutTest : ShoppingCartTest
{
    private const string Iceberg = "Iceberg";

    [Test]
    public void add_available_product()
    {
        _productsRepository.Get(Iceberg).Returns(
            TaxFreeWithNoRevenueProduct().Named(Iceberg).Costing(1.55m).Build());

        _shoppingCart.AddItem(Iceberg);
        _shoppingCart.Checkout();

        _checkoutService.Received(1).Checkout(CreateShoppingCartDto(1.55m));
    }


    [Test]
    public void add_available_two_products()
    {
        const string tomato = "Tomato";
        _productsRepository.Get(Iceberg).Returns(
            TaxFreeWithNoRevenueProduct().Named(Iceberg).Costing(1.55m).Build());
        _productsRepository.Get(tomato).Returns(
            TaxFreeWithNoRevenueProduct().Named(tomato).Costing(1m).Build());

        _shoppingCart.AddItem(Iceberg);
        _shoppingCart.AddItem(tomato);
        _shoppingCart.Checkout();

        _checkoutService.Received(1).Checkout(CreateShoppingCartDto(2.55m));
    }

    [Test]
    public void add_available_one_product_with_tax()
    {
        _productsRepository.Get(Iceberg).Returns(
            NoRevenueProduct()
                .Named(Iceberg)
                .WithTax(0.10m)
                .Costing(1m)
                .Build());

        _shoppingCart.AddItem(Iceberg);
        _shoppingCart.Checkout();

        _checkoutService.Received(1).Checkout(CreateShoppingCartDto(1.10m));
    }

    [Test]
    public void add_available_one_product_with_revenue()
    {
        _productsRepository.Get(Iceberg).Returns(
            NoTaxProduct()
                .Named(Iceberg)
                .WithRevenue(0.05m)
                .Costing(2m)
                .Build());

        _shoppingCart.AddItem(Iceberg);
        _shoppingCart.Checkout();

        _checkoutService.Received(1).Checkout(CreateShoppingCartDto(2.10m));
    }

    [Test]
    public void add_available_one_product_with_revenue_and_tax()
    {
        _productsRepository.Get(Iceberg).Returns(
            AnyProduct()
                .Named(Iceberg)
                .WithRevenue(0.05m)
                .WithTax(0.1m)
                .Costing(2m)
                .Build());

        _shoppingCart.AddItem(Iceberg);
        _shoppingCart.Checkout();

        _checkoutService.Received(1).Checkout(CreateShoppingCartDto(2.31m));
    }

    [Test]
    public void apply_available_discount()
    {
        _productsRepository.Get(Iceberg).Returns(
            TaxFreeWithNoRevenueProduct().Named(Iceberg).Costing(1.50m).Build());
        _discountsRepository.Get(DiscountCode.PROMO_5).Returns(new Discount(DiscountCode.PROMO_5, 0.5m));

        _shoppingCart.AddItem(Iceberg);
        _shoppingCart.ApplyDiscount(DiscountCode.PROMO_5);
        _shoppingCart.Checkout();

        _checkoutService.Received(1).Checkout(CreateShoppingCartDto(0.75m));
    }

    [Test]
    public void apply_two_available_discount()
    {
        _productsRepository.Get(Iceberg).Returns(
            TaxFreeWithNoRevenueProduct().Named(Iceberg).Costing(1m).Build());
        _discountsRepository.Get(DiscountCode.PROMO_5).Returns(new Discount(DiscountCode.PROMO_5, 0.5m));
        _discountsRepository.Get(DiscountCode.PROMO_10).Returns(new Discount(DiscountCode.PROMO_10, 0.01m));

        _shoppingCart.AddItem(Iceberg);
        _shoppingCart.ApplyDiscount(DiscountCode.PROMO_5);
        _shoppingCart.ApplyDiscount(DiscountCode.PROMO_10);
        _shoppingCart.Checkout();

        _checkoutService.Received(1).Checkout(CreateShoppingCartDto(0.99m));
    }

    [Test]
    public void checkout_without_products()
    {
        _shoppingCart.Checkout();

        _errorNotifier.Received(1).ShowError("No product selected, please select a product");
    }

    [Test]
    public void checkout_twice()
    {
        _productsRepository.Get(Iceberg).Returns(
            TaxFreeWithNoRevenueProduct().Named(Iceberg).Costing(1m).Build());

        _shoppingCart.AddItem(Iceberg);
        _shoppingCart.Checkout();
        _shoppingCart.Checkout();

        _checkoutService.Received(1).Checkout(CreateShoppingCartDto(1));
        _errorNotifier.Received(1).ShowError("No product selected, please select a product");
    }


    private static ShoppingCartDto CreateShoppingCartDto(decimal cost)
    {
        return new ShoppingCartDto(cost);
    }
}