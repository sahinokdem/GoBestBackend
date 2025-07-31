using GoBest.Companies;
using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Seats;

public class SeatInventoryService
{
    private readonly SeatRepository _seatRepository;

    public SeatInventoryService(SeatRepository seatRepository)
    {
        _seatRepository = seatRepository;
    }

    public async Task AddSeatInventoryFromApi(List<SeatTypeAPIDto> seatTypeAPIDtos,
     long serviceId, CompanyMode mode)
    {
        if (seatTypeAPIDtos == null) throw new ArgumentNullException(nameof(seatTypeAPIDtos));
        // Add seat types
        List<SeatType> seatTypes = SeatMapper.ToSeatTypes(seatTypeAPIDtos, mode);
        await _seatRepository.AddOrGetSeatTypesInOrderAsync(seatTypes);

        // Add seat inventories
        List<ServiceSeatInventory> inventories = SeatMapper.ToServiceSeatInventories(seatTypes, serviceId);
        FillSeatInventory(inventories, seatTypeAPIDtos);
        await _seatRepository.AddSeatInventoriesAsync(inventories);
    }

    private void FillSeatInventory(List<ServiceSeatInventory> inventories, List<SeatTypeAPIDto> seatTypeAPIDtos)
    {
        
        if (inventories == null || inventories.Count == 0) throw new ArgumentNullException(nameof(inventories));
        if (seatTypeAPIDtos == null || seatTypeAPIDtos.Count == 0) throw new ArgumentNullException(nameof(seatTypeAPIDtos));

        for (int i = 0; i < inventories.Count; i++)
        {
            var inventory = inventories[i];
            var seatTypeDto = seatTypeAPIDtos[i];

            inventory.Capacity = seatTypeDto.Capacity;
            inventory.Available = seatTypeDto.Available; 
            inventory.Price = seatTypeDto.Price;
        }
    }
}