using Android.Webkit;
using AndroidX.Fragment.App;
using Microsoft.AspNetCore.Components.WebView.Maui;
using BlazorWebView;
using BlazorCustomWebView = Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebView;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : FragmentActivity
{
    private BlazorCustomWebView blazorWebView = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);
        blazorWebView = new BlazorCustomWebView("wwwroot/index.html", Startup.Initialize(new CancellationToken()));
        blazorWebView.RootComponents.Add(new RootComponent() { Selector = "app", ComponentType = typeof(App) });
        this.SupportFragmentManager
            .BeginTransaction()
            .Add(Resource.Id.blazorfragment,blazorWebView)
            .Commit();
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
    {
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

#pragma warning disable CA1416 // Validate platform compatibility
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
#pragma warning restore CA1416 // Validate platform compatibility
    }
}