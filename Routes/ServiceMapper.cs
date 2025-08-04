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
            DepartureTime    = (DateTime)(serviceDto.Origin.DepartureTime?.AsUtc()),
            ArrivalTime      = (DateTime)(serviceDto.Destination.ArrivalTime?.AsUtc()),
            BasePrice = serviceDto.Base_Price,
            CompanyId = companyId
        };
    }
}