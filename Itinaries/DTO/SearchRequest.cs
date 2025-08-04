public sealed record SearchRequest(
    DateOnly  TravelDate,        // 2025-08-03
    long      OriginCityId,      // Istanbul
    long      DestCityId,        // Munich
    int       Passengers = 1,
    TravelMode Mode = TravelMode.All);