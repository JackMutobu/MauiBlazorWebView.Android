using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.FileProviders;
using AWebView = Android.Webkit.WebView;
using AndroidResource = BlazorWebView.Resource;
using Android.Views;
using Android.Webkit;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
    public class BlazorWebView : AndroidX.Fragment.App.Fragment, IBlazorWebView
	{
		private readonly JSComponentConfigurationStore _jSComponents = new();
		private AWebView? _webview;
        private AndroidWebKitWebViewManager? _webviewManager;
        private WebKitWebViewClient? _webKitWebViewClient;
        private WebChromeClient? _webChromeClient;
        private readonly IServiceProvider _services;

        public BlazorWebView(string hostPageRelativePath, IServiceProvider services)
		{
			RootComponents = new RootComponentsCollection(_jSComponents);
			HostPage = hostPageRelativePath;
			_services = services;
		}

		private void Initialize()
        {
			if(_webview is not null && _webviewManager == null)
            {
                AWebView.SetWebContentsDebuggingEnabled(true);

                if (_webview.Settings != null)
                {
                    _webview.Settings.JavaScriptEnabled = true;
                    _webview.Settings.DomStorageEnabled = true;
                }

                StartWebViewIfPossible();

            }
        }

        private void StartWebViewIfPossible()
        {
            var contentRootDir = Path.GetDirectoryName(HostPage!) ?? string.Empty;
            var hostPageRelativePath = Path.GetRelativePath(contentRootDir, HostPage!);

            var customFileProvider = CreateFileProvider(contentRootDir);
            var mauiAssetFileProvider = new AndroidMauiAssetFileProvider(Context.Assets, contentRootDir);
            IFileProvider fileProvider = customFileProvider == null
                ? mauiAssetFileProvider
                : new CompositeFileProvider(customFileProvider, mauiAssetFileProvider);

            _webviewManager = new AndroidWebKitWebViewManager(this, _webview!, _services,new MauiDispatcher(), fileProvider, _jSComponents, hostPageRelativePath);

            _webKitWebViewClient = new WebKitWebViewClient(_webviewManager!);
            _webview!.SetWebViewClient(_webKitWebViewClient);

            _webChromeClient = new WebChromeClient();
            _webview.SetWebChromeClient(_webChromeClient);

            if (RootComponents != null)
            {
                foreach (var rootComponent in RootComponents)
                {
                    // Since the page isn't loaded yet, this will always complete synchronously
                    _ = rootComponent.AddToWebViewManagerAsync(_webviewManager);
                }
            }

            _webviewManager.Navigate("/");
        }

        JSComponentConfigurationStore IBlazorWebView.JSComponents => _jSComponents;

		public string? HostPage { get; set; }

        public RootComponentsCollection RootComponents { get; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(AndroidResource.Layout.BlazorWebView, container, false);
            this._webview = view!.FindViewById<AWebView>(AndroidResource.Id.innerWebView);
            Initialize();
            return view;
        }
        /// <inheritdoc/>
        public virtual IFileProvider? CreateFileProvider(string contentRootDir)
		{
			return null;
		}

	}
}