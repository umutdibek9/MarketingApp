using AutoMapper;
using Order.API.DTOs;

namespace Order.API.Mappings
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Models.Order, OrderCreateDto>()
               .ForMember(dest => dest.orderItems, opt => opt.MapFrom(src => src.Items)) 
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dest => dest.FailureMesage, opt => opt.MapFrom(src => src.FailureMesage));

            CreateMap<OrderCreateDto, Models.Order>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.orderItems)) 
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        }
    }
}
