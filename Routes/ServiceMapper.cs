using GoBest.Models;
using GoBest.Routes.DTO;

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
            DepartureTime = (DateTime)serviceDto.Origin.DepartureTime,
            ArrivalTime = (DateTime)serviceDto.Destination.ArrivalTime,
            BasePrice = serviceDto.Base_Price,
            CompanyId = companyId
        };
    }
}