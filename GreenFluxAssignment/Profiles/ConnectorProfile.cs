using AutoMapper;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;

namespace GreenFluxAssignment.Profiles
{
    public class ConnectorProfile : Profile
    {
        public ConnectorProfile()
        {
            CreateMap<ConnectorDataModel, ConnectorDto>()
                .ReverseMap();

            CreateMap<CreateOrUpdateConnectorDto, ConnectorDataModel>();
        }
    }

    public class ChargeStationProfile : Profile
    {
        public ChargeStationProfile()
        {
            CreateMap<ChargeStationDataModel, ChargeStationDto>()
                .ReverseMap()
                .ForMember(dm => dm.Id, opt => opt.Ignore());

            CreateMap<CreateChargeStationDto, ChargeStationDataModel>();
        }
    }

    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<GroupDataModel, GroupDto>()
                .ReverseMap()
                .ForMember(dm => dm.Id, opt => opt.Ignore());

            CreateMap<CreateOrUpdateGroupDto, GroupDataModel>();
        }
    }
}
