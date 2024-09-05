using AutoMapper;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ElasticModel;

namespace CleanArch.Core.Entities.Mapper 
{
    public class CreateProfileMapper : Profile
    {
        public CreateProfileMapper()
        {
            CreateMap<CallbackRequest, CallbackELKModel>();
            CreateMap<CallbackELKModel, CallbackRequest>();
        }
    }
}


