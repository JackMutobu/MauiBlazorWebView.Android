using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
    public static class BlazorWebViewRegistrations
    {
		public static void RegisterBlazorWebView(this IServiceCollection services)
		{
			if (services is null)
			{
				throw new ArgumentNullException(nameof(services));
			}
			services.AddBlazorWebView();

		}
	}
}
