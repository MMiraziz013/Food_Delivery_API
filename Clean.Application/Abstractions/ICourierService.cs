using Clean.Application.Dtos.Courier;
using Clean.Application.Responses;

namespace Clean.Application.Abstractions;

public interface ICourierService
{
    
    public Task<Response<string>> PromoteToCourierAsync(string userId, CreateCourierDto dto);
    public Task<Response<string>> CompleteOrderAsync(int orderId);

    public Task<Response<GetCourierDto>> UpdateCourierProfileAsync(UpdateCourierDto dto);

    public Task<Response<string>> DeleteCourierAccountAsync(int courierId);

    public Task<Response<List<GetCourierDto>>> GetCouriersAsync();
}