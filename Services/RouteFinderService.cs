using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoBest.Models;
using GoBest.Routes.DTO;
using Microsoft.Extensions.Logging;

namespace GoBest.Services
{
    public class RouteFinderService
    {
        private readonly ServiceRepository _serviceRepository;
        private readonly ILogger<RouteFinderService> _logger;

        public RouteFinderService(ServiceRepository serviceRepository, ILogger<RouteFinderService> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<List<ServiceAPIDto>> FindRoutesAsync(
            long originStationId, long destStationId, 
            DateTime departureDate, string preferredMode = null)
        {
            // First try to find direct services
            var directServices = await _serviceRepository.FindDirectServicesAsync(
                originStationId, destStationId, departureDate);
                
            if (directServices.Any())
            {
                _logger.LogInformation("Found {Count} direct services", directServices.Count);
                return MapServicesToApiDto(directServices);
            }
            
            // If no direct services, find multi-leg routes using Dijkstra algorithm
            _logger.LogInformation("No direct services found, searching for multi-leg routes");
            var routes = await FindMultiLegRoutesAsync(originStationId, destStationId, departureDate, preferredMode);
            
            return routes;
        }
        
        private async Task<List<ServiceAPIDto>> FindMultiLegRoutesAsync(
            long originStationId, long destStationId, 
            DateTime departureDate, string preferredMode)
        {
            // Get all services for the departure date and future dates (within a reasonable window)
            var allServices = await _serviceRepository.GetAllServicesForRouteCalculationAsync();
            var startDate = departureDate.Date;
            var endDate = startDate.AddDays(3); // Look up to 3 days ahead for connections
            
            var relevantServices = allServices.Where(s => s.DepartureTime >= startDate && s.DepartureTime <= endDate).ToList();
            
            // Build graph of stations and connections
            var graph = BuildGraph(relevantServices);
            
            // Find shortest paths using Dijkstra's algorithm
            var paths = FindShortestPaths(graph, originStationId, destStationId);
            
            if (!paths.Any())
            {
                _logger.LogWarning("No paths found between stations {Origin} and {Destination}", 
                    originStationId, destStationId);
                return new List<ServiceAPIDto>();
            }
            
            // Convert paths to ServiceAPIDto format
            var results = new List<ServiceAPIDto>();
            
            foreach (var path in paths.Take(5)) // Limit to top 5 paths
            {
                var pathServices = new List<Service>();
                
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var fromStationId = path[i];
                    var toStationId = path[i + 1];
                    
                    // Find service connecting these stations
                    var service = relevantServices.Where(s => 
                        s.OriginStationId == fromStationId && 
                        s.DestStationId == toStationId)
                        .OrderBy(s => s.DepartureTime)
                        .FirstOrDefault();
                        
                    if (service != null)
                    {
                        pathServices.Add(service);
                    }
                }
                
                // Add the path to results if we found all connecting services
                if (pathServices.Count == path.Count - 1)
                {
                    results.AddRange(MapServicesToApiDto(pathServices));
                }
            }
            
            return results;
        }
        
        private Dictionary<long, List<(long StationId, double Distance)>> BuildGraph(List<Service> services)
        {
            var graph = new Dictionary<long, List<(long StationId, double Distance)>>();
            
            foreach (var service in services)
            {
                if (!graph.ContainsKey(service.OriginStationId.Value))
                {
                    graph[service.OriginStationId.Value] = new List<(long StationId, double Distance)>();
                }
                
                // Add edge (connection) with weight based on duration and/or price
                var duration = (service.ArrivalTime - service.DepartureTime).TotalMinutes;
                var weight = duration + (double)service.BasePrice * 0.1; // Weight as combination of time and price
                
                graph[service.OriginStationId.Value].Add((service.DestStationId.Value, weight));
            }
            
            return graph;
        }
        
        private List<List<long>> FindShortestPaths(
            Dictionary<long, List<(long StationId, double Distance)>> graph, 
            long startNode, long endNode)
        {
            var distances = new Dictionary<long, double>();
            var previousNodes = new Dictionary<long, long>();
            var unvisited = new HashSet<long>();
            
            // Initialize distances with infinity for all nodes except start
            foreach (var node in graph.Keys)
            {
                distances[node] = double.MaxValue;
                unvisited.Add(node);
            }
            
            distances[startNode] = 0;
            
            while (unvisited.Count > 0)
            {
                // Find the unvisited node with smallest known distance
                var current = unvisited.OrderBy(node => distances[node]).FirstOrDefault();
                
                // If we've processed our destination or if smallest distance is infinity, we're done
                if (current == endNode || distances[current] == double.MaxValue)
                    break;
                    
                unvisited.Remove(current);
                
                // Check all neighboring nodes
                if (graph.ContainsKey(current))
                {
                    foreach (var (neighbor, distance) in graph[current])
                    {
                        var alt = distances[current] + distance;
                        if (alt < distances.GetValueOrDefault(neighbor, double.MaxValue))
                        {
                            distances[neighbor] = alt;
                            previousNodes[neighbor] = current;
                        }
                    }
                }
            }
            
            // Reconstruct the shortest path
            var paths = new List<List<long>>();
            if (!previousNodes.ContainsKey(endNode))
            {
                return paths; // No path found
            }
            
            var path = new List<long>();
            var node = endNode;
            
            while (node != startNode)
            {
                path.Add(node);
                node = previousNodes[node];
            }
            
            path.Add(startNode);
            path.Reverse();
            paths.Add(path);
            
            return paths;
        }
        
        private List<ServiceAPIDto> MapServicesToApiDto(List<Service> services)
        {
            var result = new List<ServiceAPIDto>();
            
            foreach (var service in services)
            {
                var dto = new ServiceAPIDto
                {
                    Service_Id = (int)service.Id,
                    Service_Code = service.ServiceCode,
                    Mode = service.Mode,
                    Base_Price = service.BasePrice,
                    Currency = service.Currency,
                    Company = new CompanyAPIDto
                    {
                        Id = (int)service.CompanyId.Value,
                        Name = service.Company?.Name ?? "Unknown",
                        Iata_Code = service.Company?.IataCode,
                        Country_Code = service.Company?.CountryCode ?? "Unknown"
                    },
                    Origin = new StationAPIDto
                    {
                        Station_Id = (int)service.OriginStationId.Value,
                        Name = service.OriginStation?.Name ?? "Unknown",
                        Code = service.OriginStation?.Code ?? "Unknown",
                        Latitude = service.OriginStation?.Latitude ?? 0,
                        Longitude = service.OriginStation?.Longitude ?? 0,
                        Departure_Time = service.DepartureTime,
                        City = new CityAPIDto
                        {
                            Id = service.OriginStation?.CityId ?? 0,
                            Name = service.OriginStation?.City?.Name ?? "Unknown",
                            Country_Code = service.OriginStation?.City?.CountryCode ?? "Unknown"
                        }
                    },
                    Destination = new StationAPIDto
                    {
                        Station_Id = (int)service.DestStationId.Value,
                        Name = service.DestStation?.Name ?? "Unknown",
                        Code = service.DestStation?.Code ?? "Unknown",
                        Latitude = service.DestStation?.Latitude ?? 0,
                        Longitude = service.DestStation?.Longitude ?? 0,
                        Arrival_Time = service.ArrivalTime,
                        City = new CityAPIDto
                        {
                            Id = service.DestStation?.CityId ?? 0,
                            Name = service.DestStation?.City?.Name ?? "Unknown",
                            Country_Code = service.DestStation?.City?.CountryCode ?? "Unknown"
                        }
                    },
                    Seat_Types = service.ServiceSeatInventories.Select(s => new SeatTypeAPIDto
                    {
                        Id = (int)s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        Available = s.Available
                    }).ToList()
                };
                
                result.Add(dto);
            }
            
            return result;
        }
    }
}
