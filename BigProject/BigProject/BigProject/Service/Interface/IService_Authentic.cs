using BigProject.Payload.Response;
using BigProject.PayLoad.DTO;
using BigProject.PayLoad.Request;

namespace BigProject.Service.Interface
{
    public interface IService_Authentic
    {
        Task <ResponseObject<DTO_Register>> Register(Request_Register request);
    }
}
