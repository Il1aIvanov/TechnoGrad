using TechnoGrad.Models;
namespace TechnoGrad.Services.Interfaces;

public interface IMapLocationService
{
    Task<CustomMapPoint> GetLatLongFromAddressAsync(string address);
}