using GoBest.Models;
using GoBest.Routes.DTO;
using GoBest.Util;

namespace GoBest.Routes;

public class ServiceMapper
{
    public static Service ToService(ServiceAPIDto serviceDto,
     long companyId, long originStationId, long destStationId)
    {
        if (serviceDto == null)
        {
            throw new ArgumentNullException(nameof(serviceDto));
        }

        return new Service
        {
            ServiceCode = serviceDto.Service_Code,
            OriginStationId = originStationId,
            DestStationId = destStationId,
            DepartureTime = (DateTime)(serviceDto.Origin.DepartureTime?.AsUtc()),
            ArrivalTime = (DateTime)(serviceDto.Destination.ArrivalTime?.AsUtc()),
            BasePrice = serviceDto.Base_Price,
            CompanyId = companyId
        };
    }

    public static List<ServiceResponse> ToResponses(List<Service> services)
    {
        var responses = new List<ServiceResponse>();
        foreach (var service in services)
        {
            responses.Add(new ServiceResponse
            {
                Id = service.Id,
                ServiceCode = service.ServiceCode,
                OriginCity = service.OriginStation?.City?.Name ?? "Unknown",
                DestCity = service.DestStation?.City?.Name ?? "Unknown",
                CompanyName = service.Company?.Name ?? "Unknown"
            });
        }
        return responses;
    }
}