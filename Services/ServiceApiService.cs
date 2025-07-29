using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GoBest.Companies;
using GoBest.Data;
using GoBest.Models;
using GoBest.Routes.DTO;
using GoBest.Stations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GoBest.Services
{
    public class ServiceApiService : IHostedService, IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServiceApiService> _logger;
        private Timer _timer;

        public ServiceApiService(
            IHttpClientFactory httpClientFactory,
            IServiceProvider serviceProvider,
            ILogger<ServiceApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service API Service is starting.");
            
            // Run immediately, then every hour
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service API Service is stopping.");
            
            _timer?.Change(Timeout.Infinite, 0);
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Fetching services from API at: {time}", DateTimeOffset.Now);
            FetchAndSaveServicesAsync().Wait();
        }

        public async Task FetchAndSaveServicesAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var companyRepository = scope.ServiceProvider.GetRequiredService<CompanyRepository>();
                var stationRepository = scope.ServiceProvider.GetRequiredService<StationRepository>();
                var serviceRepository = scope.ServiceProvider.GetRequiredService<ServiceRepository>();
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                
                var client = _httpClientFactory.CreateClient("ServiceAPI");
                var response = await client.GetAsync("api/services"); // Replace with actual endpoint
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get services. Status code: {StatusCode}", response.StatusCode);
                    return;
                }
                
                var content = await response.Content.ReadAsStringAsync();
                var serviceApiDtos = JsonSerializer.Deserialize<List<ServiceAPIDto>>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                if (serviceApiDtos == null || !serviceApiDtos.Any())
                {
                    _logger.LogWarning("No services returned from the API");
                    return;
                }
                
                _logger.LogInformation("Fetched {Count} services from the API", serviceApiDtos.Count);
                
                // Begin transaction
                using var transaction = await dbContext.Database.BeginTransactionAsync();
                
                try
                {
                    foreach (var serviceDto in serviceApiDtos)
                    {
                        // Save company first to get ID
                        var company = await companyRepository.SaveCompanyFromApiAsync(serviceDto.Company);
                        
                        // Save stations to get IDs
                        var originStation = await stationRepository.SaveStationFromApiAsync(serviceDto.Origin);
                        var destStation = await stationRepository.SaveStationFromApiAsync(serviceDto.Destination);
                        
                        // Check if service already exists
                        var existingService = await serviceRepository.GetServiceByCodeAsync(serviceDto.Service_Code);
                        
                        if (existingService != null)
                        {
                            // Update existing service
                            existingService.CompanyId = company.Id;
                            existingService.OriginStationId = originStation.Id;
                            existingService.DestStationId = destStation.Id;
                            existingService.BasePrice = serviceDto.Base_Price;
                            existingService.Mode = serviceDto.Mode;
                            existingService.Currency = serviceDto.Currency;
                            existingService.DepartureTime = serviceDto.Origin.Departure_Time;
                            existingService.ArrivalTime = serviceDto.Destination.Arrival_Time;
                            
                            await serviceRepository.UpdateServiceAsync(existingService);
                            
                            // Update seat inventories
                            var existingInventories = await dbContext.ServiceSeatInventories
                                .Where(s => s.ServiceId == existingService.Id)
                                .ToListAsync();
                                
                            // Remove existing inventories that are no longer in API response
                            foreach (var inventory in existingInventories)
                            {
                                if (!serviceDto.Seat_Types.Any(st => st.Name == inventory.Name))
                                {
                                    dbContext.ServiceSeatInventories.Remove(inventory);
                                }
                            }
                            
                            // Add or update seat inventories
                            foreach (var seatType in serviceDto.Seat_Types)
                            {
                                var existingInventory = existingInventories
                                    .FirstOrDefault(i => i.Name == seatType.Name);
                                    
                                if (existingInventory != null)
                                {
                                    existingInventory.Price = seatType.Price;
                                    existingInventory.Available = seatType.Available;
                                    dbContext.ServiceSeatInventories.Update(existingInventory);
                                }
                                else
                                {
                                    var newInventory = new ServiceSeatInventory
                                    {
                                        ServiceId = existingService.Id,
                                        Name = seatType.Name,
                                        Price = seatType.Price,
                                        Available = seatType.Available
                                    };
                                    dbContext.ServiceSeatInventories.Add(newInventory);
                                }
                            }
                        }
                        else
                        {
                            // Create new service
                            var newService = new Service
                            {
                                ServiceCode = serviceDto.Service_Code,
                                CompanyId = company.Id,
                                OriginStationId = originStation.Id,
                                DestStationId = destStation.Id,
                                BasePrice = serviceDto.Base_Price,
                                Mode = serviceDto.Mode,
                                Currency = serviceDto.Currency,
                                DepartureTime = serviceDto.Origin.Departure_Time,
                                ArrivalTime = serviceDto.Destination.Arrival_Time,
                                Sold = false,
                                SalesCount = 0
                            };
                            
                            dbContext.Services.Add(newService);
                            await dbContext.SaveChangesAsync(); // Save to get the ID
                            
                            // Add seat inventories
                            foreach (var seatType in serviceDto.Seat_Types)
                            {
                                var newInventory = new ServiceSeatInventory
                                {
                                    ServiceId = newService.Id,
                                    Name = seatType.Name,
                                    Price = seatType.Price,
                                    Available = seatType.Available
                                };
                                dbContext.ServiceSeatInventories.Add(newInventory);
                            }
                        }
                        
                        await dbContext.SaveChangesAsync();
                    }
                    
                    // Commit transaction if all successful
                    await transaction.CommitAsync();
                    _logger.LogInformation("Successfully saved all services to the database");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error saving services to database");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services from API");
            }
        }
    }
}
