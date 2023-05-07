using JellyFish.Handler.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.Handler.Device.Sensor
{
    public class GpsHandler : AbstractDeviceActionHandler<Permissions.LocationWhenInUse> 
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        public async Task<Location> GetCurrentLocation()
        {
            Location location = null;   
            if(!_isCheckingLocation)
            {
                try
                {
                    _isCheckingLocation = true;
                    _cancelTokenSource = new CancellationTokenSource();

                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                    location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _isCheckingLocation = false;
                }
            }
            return location;
        }
        private async Task<Placemark> GetGeocodeReverseData(Location location)
        {
            var latitude = location.Latitude;
            var longitude = location.Longitude;
            IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);
            Placemark placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                /*
                    $"AdminArea:       {placemark.AdminArea}\n" +
                    $"CountryCode:     {placemark.CountryCode}\n" +
                    $"CountryName:     {placemark.CountryName}\n" +
                    $"FeatureName:     {placemark.FeatureName}\n" +
                    $"Locality:        {placemark.Locality}\n" +
                    $"PostalCode:      {placemark.PostalCode}\n" +
                    $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                    $"SubLocality:     {placemark.SubLocality}\n" +
                    $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                    $"Thoroughfare:    {placemark.Thoroughfare}\n";*/
                return placemark;

            }

            return null;
        }

    }
}
