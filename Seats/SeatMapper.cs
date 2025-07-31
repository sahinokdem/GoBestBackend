using GoBest.Companies;
using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Seats;

public class SeatMapper
{

    public static List<SeatType> ToSeatTypes(List<SeatTypeAPIDto> seatTypeAPIDtos, CompanyMode mode)
    {
        if (seatTypeAPIDtos == null || !seatTypeAPIDtos.Any())
        {
            throw new ArgumentNullException(nameof(seatTypeAPIDtos));
        }

        return seatTypeAPIDtos.Select(dto => new SeatType
        {
            Mode = mode,
            Name = dto.Name,
            PriceMultiplier = 1,
        }).ToList();
    }

    public static List<ServiceSeatInventory> ToServiceSeatInventories(List<SeatType> seatTypes, long serviceId)
    {
        if (seatTypes == null) throw new ArgumentNullException(nameof(seatTypes));


        return seatTypes.Select(seatType => new ServiceSeatInventory
        {

            ServiceId = serviceId,
            SeatTypeId = seatType.Id

        }).ToList();
    }
}