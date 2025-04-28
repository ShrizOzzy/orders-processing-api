using AutoMapper;
using OrderProcess.Application.Services.Commands.CreateOrder;
using OrderProcess.Core.Models;
using OrderProcess.Core.Models.DTOs;

namespace OrderProcess.Infrastructure;

public class OrderMapperProfile : Profile
{
    public OrderMapperProfile()
    {
        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.ItemTotal, opt => opt.MapFrom(src => src.Product.Price * src.Quantity));

        CreateMap<CreateOrderCommand, Order>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items));

        CreateMap<OrderItemRequestDto, OrderItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }
}