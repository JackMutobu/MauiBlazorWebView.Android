using Xamarin.Essentials;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
    internal sealed class MauiDispatcher : Dispatcher
	{
#pragma warning disable CA1416 // Validate platform compatibility
		public override bool CheckAccess()
		{
			return true;
		}

		public override Task InvokeAsync(Action workItem)
		{
			MainThread.BeginInvokeOnMainThread(workItem);
			return Task.CompletedTask;
		}

		public override Task InvokeAsync(Func<Task> workItem)
		{
			MainThread.BeginInvokeOnMainThread(() => workItem.Invoke());
			return Task.CompletedTask;
		}

		public override Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem)
		{
			var tcs = new TaskCompletionSource<TResult>();
			MainThread.BeginInvokeOnMainThread(() =>
            {
				tcs.SetResult(workItem());
            });

			return tcs.Task.WaitAsync(TimeSpan.FromMilliseconds(100));
		}

		public override Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
		{
			var tcs = new TaskCompletionSource<Task<TResult>>();
			MainThread.BeginInvokeOnMainThread(() =>
			{
				tcs.SetResult(workItem());
			});

			return tcs.Task.GetAwaiter().GetResult();
		}
#pragma warning restore CA1416 // Validate platform compatibility
	}
}