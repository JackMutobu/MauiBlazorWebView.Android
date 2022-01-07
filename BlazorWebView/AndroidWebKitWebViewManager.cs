using Android.Webkit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.FileProviders;
using AWebView = Android.Webkit.WebView;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
    /// <summary>
    /// An implementation of <see cref="WebViewManager"/> that uses the Android WebKit WebView browser control
    /// to render web content.
    /// </summary>
    public class AndroidWebKitWebViewManager : WebViewManager
	{
		// Using an IP address means that WebView doesn't wait for any DNS resolution,
		// making it substantially faster. Note that this isn't real HTTP traffic, since
		// we intercept all the requests within this origin.
		private const string AppOrigin = "https://0.0.0.0/";
		private static readonly Android.Net.Uri AndroidAppOriginUri = Android.Net.Uri.Parse(AppOrigin)!;
		private readonly BlazorWebView _blazorWebView;
		private readonly AWebView _webview;

		/// <summary>
		/// Constructs an instance of <see cref="AndroidWebKitWebViewManager"/>.
		/// </summary>
		/// <param name="webview">A wrapper to access platform-specific WebView APIs.</param>
		/// <param name="services">A service provider containing services to be used by this class and also by application code.</param>
		/// <param name="dispatcher">A <see cref="Dispatcher"/> instance that can marshal calls to the required thread or sync context.</param>
		/// <param name="fileProvider">Provides static content to the webview.</param>
		/// <param name="hostPageRelativePath">Path to the host page within the <paramref name="fileProvider"/>.</param>
		public AndroidWebKitWebViewManager(BlazorWebView blazorWebView, AWebView webview, IServiceProvider services, Dispatcher dispatcher, IFileProvider fileProvider, JSComponentConfigurationStore jsComponents, string hostPageRelativePath)
			: base(services, dispatcher, new Uri(AppOrigin), fileProvider, jsComponents, hostPageRelativePath)
		{
			_blazorWebView = blazorWebView ?? throw new ArgumentNullException(nameof(blazorWebView));
			_webview = webview ?? throw new ArgumentNullException(nameof(webview));
		}

		/// <inheritdoc />
		protected override void NavigateCore(Uri absoluteUri)
		{
			_webview.LoadUrl(absoluteUri.AbsoluteUri);
		}

		/// <inheritdoc />
		protected override void SendMessage(string message)
		{
#pragma warning disable CA1416 // Validate platform compatibility
            _webview.PostWebMessage(new WebMessage(message), AndroidAppOriginUri);
#pragma warning restore CA1416 // Validate platform compatibility
        }

		internal bool TryGetResponseContentInternal(string uri, bool allowFallbackOnHostPage, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers) =>
			TryGetResponseContent(uri, allowFallbackOnHostPage, out statusCode, out statusMessage, out content, out headers);

		internal void SetUpMessageChannel()
		{
#pragma warning disable CA1416 // Validate platform compatibility
            var nativeToJsPorts = _webview.CreateWebMessageChannel();
#pragma warning restore CA1416 // Validate platform compatibility

            var nativeToJs = new BlazorWebMessageCallback(message =>
			{
				MessageReceived(new Uri(AppOrigin), message!);
			});

			var destPort = new[] { nativeToJsPorts[1] };

#pragma warning disable CA1416 // Validate platform compatibility
            nativeToJsPorts[0].SetWebMessageCallback(nativeToJs);
#pragma warning restore CA1416 // Validate platform compatibility

#pragma warning disable CA1416 // Validate platform compatibility
            _webview.PostWebMessage(new WebMessage("capturePort", destPort), AndroidAppOriginUri);
#pragma warning restore CA1416 // Validate platform compatibility
        }

		private class BlazorWebMessageCallback : WebMessagePort.WebMessageCallback
		{
			private readonly Action<string?> _onMessageReceived;

			public BlazorWebMessageCallback(Action<string?> onMessageReceived)
			{
				_onMessageReceived = onMessageReceived ?? throw new ArgumentNullException(nameof(onMessageReceived));
			}

			public override void OnMessage(WebMessagePort? port, WebMessage? message)
			{
				if (message is null)
				{
					throw new ArgumentNullException(nameof(message));
				}

#pragma warning disable CA1416 // Validate platform compatibility
                _onMessageReceived(message.Data);
#pragma warning restore CA1416 // Validate platform compatibility
            }
		}
	}
}