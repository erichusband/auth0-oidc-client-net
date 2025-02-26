﻿using AuthenticationServices;
using Foundation;
using IdentityModel.OidcClient.Browser;
using System.Threading.Tasks;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the Browser interface using ASWebAuthenticationSession for support on iOS 12+.
    /// </summary>
    public class ASWebAuthenticationSessionBrowser : IOSBrowserBase
    {
        protected override Task<BrowserResult> Launch(BrowserOptions options)
        {
            return Launch(options);
        }

        internal static Task<BrowserResult> Start(BrowserOptions options)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            ASWebAuthenticationSession asWebAuthenticationSession = null;
            asWebAuthenticationSession = new ASWebAuthenticationSession(
                new NSUrl(options.StartUrl),
                options.EndUrl,
                (callbackUrl, error) =>
                {
                    tcs.SetResult(CreateBrowserResult(callbackUrl, error));
                    asWebAuthenticationSession.Dispose();
                });

            asWebAuthenticationSession.Start();

            return tcs.Task;
        }

        private static BrowserResult CreateBrowserResult(NSUrl callbackUrl, NSError error)
        {
            if (error == null)
                return Success(callbackUrl.AbsoluteString);

            if (error.Code == (long)ASWebAuthenticationSessionErrorCode.CanceledLogin)
                return Canceled();

            return UnknownError(error.ToString());
        }
    }
}
