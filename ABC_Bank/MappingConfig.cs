using ABC_Bank.Model;
using AutoMapper;
using ABC_Bank.Dtos;

namespace ABC_Bank
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Contacto, ContactoDto>();
            CreateMap<ContactoDto, Contacto>();
        }
    }
}
