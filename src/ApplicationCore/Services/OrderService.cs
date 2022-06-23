using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUriComposer _uriComposer;
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;

    private readonly IAppLogger<OrderService> _logger;
    private readonly IShopBusService _shopBusService;
    private readonly IOrderItemsResolverTriggerService _orderItemsResolverTriggerService;

    public OrderService(IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IRepository<Order> orderRepository,
        IUriComposer uriComposer, 
        IShopBusService shopBusService,
        IOrderItemsResolverTriggerService orderItemsResolverTriggerService,
        IAppLogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _uriComposer = uriComposer;
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;

        _shopBusService = shopBusService;
        _orderItemsResolverTriggerService = orderItemsResolverTriggerService;

        _logger = logger;
    }

    public async Task CreateOrderAsync(int basketId, Address shippingAddress)
    {
        var basketSpec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.GetBySpecAsync(basketSpec);

        Guard.Against.NullBasket(basketId, basket);
        Guard.Against.EmptyBasketOnCheckout(basket.Items);

        var catalogItemsSpecification = new CatalogItemsSpecification(basket.Items.Select(item => item.CatalogItemId).ToArray());
        var catalogItems = await _itemRepository.ListAsync(catalogItemsSpecification);

        var items = basket.Items.Select(basketItem =>
        {
            var catalogItem = catalogItems.First(c => c.Id == basketItem.CatalogItemId);
            var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, _uriComposer.ComposePicUri(catalogItem.PictureUri));
            var orderItem = new OrderItem(itemOrdered, basketItem.UnitPrice, basketItem.Quantity);
            return orderItem;
        }).ToList();

        var order = new Order(basket.BuyerId, shippingAddress, items);

        _logger.LogInformation($"Save order with id: {order.Id}");
        await _orderRepository.AddAsync(order);

        // _logger.LogInformation($"Trigger OrderItemsBlobResolver for order id: {order.Id}");
        // await _orderItemsResolverTriggerService.TriggerOrderItemsBlobResolver(order);
        _logger.LogInformation($"Trigger OrderItemsCosmosDbResolver for order id: {order.Id}");
        await _orderItemsResolverTriggerService.TriggerDeliveryItemsProcessor(order);

        _logger.LogInformation($"Add to the orders topic order with id: {order.Id}");
        _shopBusService.EnqueueOrderForUpload(order);
    }
}
