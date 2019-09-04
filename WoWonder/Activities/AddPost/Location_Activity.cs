using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using Debug = System.Diagnostics.Debug;

namespace WoWonder.Activities.AddPost
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Location_Activity : AppCompatActivity, IOnMapReadyCallback, ILocationListener,
        GoogleMap.IOnInfoWindowClickListener
    {
        public AutoCompleteTextView AutoCompleteSearchView;
        public double CurrentLatitude;
        public double CurrentLongitude;

        public LocationManager locationManager;
        public GoogleMap Map;
        public string provider;
        public List<string> Result;


        public void OnLocationChanged(Location location)
        {
            try
            {
                double lat, lng;
                lat = location.Latitude;
                lng = location.Longitude;

                var makerOptions = new MarkerOptions();
                makerOptions.SetPosition(new LatLng(lat, lng));
                makerOptions.SetTitle(GetText(Resource.String.Lbl_Title_Location));
                makerOptions.SetSnippet(GetText(Resource.String.Lbl_Snippet_Location));
                Map.AddMarker(makerOptions);
                Map.SetOnInfoWindowClickListener(this); // Add event click on marker icon
                Map.MapType = GoogleMap.MapTypeNormal;

                var builder = CameraPosition.InvokeBuilder();
                builder.Target(new LatLng(lat, lng));
                var cameraPosition = builder.Zoom(17).Target(new LatLng(lat, lng)).Build();
                cameraPosition.Zoom = 18;

                var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                Map.MoveCamera(cameraUpdate);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        public void OnInfoWindowClick(Marker marker)
        {
            // Toast.MakeText(this, $"Icon {marker.Title} is clicked", ToastLength.Short).Show();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                Map = googleMap;

                //Optional
                googleMap.UiSettings.ZoomControlsEnabled = true;
                googleMap.UiSettings.CompassEnabled = true;
                googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());
                googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());
                googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.LocationMap_Activity);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.LocationMap_Activity);


                Result = new List<string>();
                locationManager = (LocationManager) GetSystemService(LocationService);
                AutoCompleteSearchView = FindViewById<AutoCompleteTextView>(Resource.Id.searchView);

                var mapFragment = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
                mapFragment.GetMapAsync(this);

                provider = locationManager.GetBestProvider(new Criteria(), false);

                var location = locationManager.GetLastKnownLocation(provider);
                if (location == null)
                    Debug.WriteLine("No Location");

                location = locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                if (location != null)
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Finding_your_Location), ToastLength.Short).Show();

                AutoCompleteSearchView.Hint = GetText(Resource.String.Lbl_SearchForPlace);
                AutoCompleteSearchView.Adapter =
                    new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, Result);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                    CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                {
                    var location = locationManager.GetLastKnownLocation(LocationManager.PassiveProvider);
                    if (location != null)
                    {
                        CurrentLatitude = location.Latitude;
                        CurrentLongitude = location.Longitude;
                        OnLocationChanged(location);
                    }

                    locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 400, 1, this);
                    locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 400, 1, this);
                }
                else
                {
                    RequestPermissions(new[]
                    {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation
                    }, 201);

                    Toast.MakeText(this, GetText(Resource.String.Lbl_NotEnoughPermission), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                locationManager.RemoveUpdates(this);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 201)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var location = locationManager.GetLastKnownLocation(LocationManager.PassiveProvider);
                        if (location != null)
                        {
                            CurrentLatitude = location.Latitude;
                            CurrentLongitude = location.Longitude;
                            OnLocationChanged(location);
                        }

                        locationManager.RequestLocationUpdates(provider, 400, 1, this);
                        locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 400, 1, this);
                        locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 400, 1, this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public async Task<string> GetNameLocation(string address)
        {
            try
            {
                var http = new HttpDataHandler();
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}";
                var responese = http.GetHTTPDataAsync(url);

                return responese;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return "";
            }
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
    }
}