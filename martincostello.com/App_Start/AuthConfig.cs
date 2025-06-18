// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthConfig.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2013-2015
// </copyright>
// <summary>
//   AuthConfig.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using MartinCostello.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Owin.Security.Providers.GitHub;

namespace MartinCostello
{
    /// <summary>
    /// A class representing the authentication and authorization configuration.  This class cannot be inherited.
    /// </summary>
    internal static class AuthConfig
    {
        /// <summary>
        /// Configures authentication and authorization for the application.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> to configure.</param>
        internal static void ConfigureAuth(IAppBuilder app)
        {
            Debug.Assert(app != null, "app cannot be null.");

            app.CreatePerOwinContext(ApplicationUserContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with
            // a third party login provider.
            app.UseCookieAuthentication(CreateCookieOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be
            // remembered on the device where you logged in from. This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            ConfigureGitHubAuth(app);
            ConfigureGoogleAuth(app);
            ConfigureMicrosoftAuth(app);
            ConfigureTwitterAuth(app);

            ConfigureAntiForgery();
        }

        /// <summary>
        /// Configures the <see cref="AntiForgeryConfig"/> class.
        /// </summary>
        internal static void ConfigureAntiForgery()
        {
            AntiForgeryConfig.CookieName = "martincostelloxsrf";                    // Match the naming scheme used for other cookies
            AntiForgeryConfig.RequireSsl = !HttpContext.Current.IsDebuggingEnabled; // The site should only ever run over HTTPS
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;                   // We already specify this in Web.config
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;          // Tell anti-forgery to use the name claim
        }

        /// <summary>
        /// Creates a new instance of <see cref="CookieAuthenticationOptions"/> with the appropriate settings.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="CookieAuthenticationOptions"/>.
        /// </returns>
        internal static CookieAuthenticationOptions CreateCookieOptions()
        {
            return new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieHttpOnly = true,
                CookieName = "martincostellologin",
                CookieSecure = CookieSecureOption.Always,
                ////ExpireTimeSpan = TimeSpan.FromMinutes(30),
                LoginPath = new PathString("/account/login"),
                Provider = new CookieAuthenticationProvider()
                {
                    OnValidateIdentity = OnValidateIdentity(),
                },
                SlidingExpiration = true,
            };
        }

        /// <summary>
        /// Configures <c>GitHub</c> authentication.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> to configure.</param>
        private static void ConfigureGitHubAuth(IAppBuilder app)
        {
            const string Name = "GitHub";

            if (IsAuthProviderEnabled(Name))
            {
                string clientId = ConfigurationManager.AppSettings[Name + "Auth:ClientId"];
                string clientSecret = ConfigurationManager.AppSettings[Name + "Auth:ClientSecret"];

                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
                {
                    var options = new GitHubAuthenticationOptions()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        ////Provider = new GitHubAuthenticationProvider()
                        ////{
                        ////    OnAuthenticated = OnAuthenticatedWithGitHubAsync,
                        ////},
                    };

                    options.Scope.Clear();  // Remove the read-write default "user" scope
                    options.Scope.Add("user:email");

                    app.UseGitHubAuthentication(options);
                }
            }
        }

        /// <summary>
        /// Configures Google authentication.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> to configure.</param>
        private static void ConfigureGoogleAuth(IAppBuilder app)
        {
            const string Name = "Google";

            if (IsAuthProviderEnabled(Name))
            {
                string clientId = ConfigurationManager.AppSettings[Name + "Auth:ClientId"];
                string clientSecret = ConfigurationManager.AppSettings[Name + "Auth:ClientSecret"];

                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
                {
                    var options = new GoogleOAuth2AuthenticationOptions()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Provider = new GoogleOAuth2AuthenticationProvider()
                        {
                            OnAuthenticated = OnAuthenticatedWithGoogleAsync,
                        },
                    };

                    options.Scope.Add("email");

                    app.UseGoogleAuthentication(options);
                }
            }
        }

        /// <summary>
        /// Configures Microsoft Account authentication.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> to configure.</param>
        private static void ConfigureMicrosoftAuth(IAppBuilder app)
        {
            const string Name = "Microsoft";

            if (IsAuthProviderEnabled(Name))
            {
                string clientId = ConfigurationManager.AppSettings[Name + "Auth:ClientId"];
                string clientSecret = ConfigurationManager.AppSettings[Name + "Auth:ClientSecret"];

                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
                {
                    var options = new MicrosoftAccountAuthenticationOptions()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Provider = new MicrosoftAccountAuthenticationProvider()
                        {
                            OnAuthenticated = OnAuthenticatedWithMicrosoftAsync,
                        },
                    };

                    options.Scope.Add("wl.emails");
                    options.Scope.Add("wl.signin"); // Allow automatic sign-in if already signed in to another site

                    app.UseMicrosoftAccountAuthentication(options);
                }
            }
        }

        /// <summary>
        /// Configures Twitter authentication.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> to configure.</param>
        private static void ConfigureTwitterAuth(IAppBuilder app)
        {
            const string Name = "Twitter";

            if (IsAuthProviderEnabled(Name))
            {
                string consumerKey = ConfigurationManager.AppSettings[Name + "Auth:ConsumerKey"];
                string consumerSecret = ConfigurationManager.AppSettings[Name + "Auth:ConsumerSecret"];

                if (!string.IsNullOrEmpty(consumerKey) && !string.IsNullOrEmpty(consumerSecret))
                {
                    // The Twitter SSL certificate thumbprints are hardcoded into OWIN.
                    // Latest list take from the following link on 28/09/15:
                    // https://stackoverflow.com/questions/25011890/owin-twitter-login-the-remote-certificate-is-invalid-according-to-the-validati
                    var twitterCerts = new[]
                    {
                        "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                        "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                        "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                        "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                        "4EB6D578499B1CCF5F581EAD56BE3D9B6744A5E5", // VeriSign Class 3 Primary CA - G5
                        "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A
                        "B13EC36903F8BF4701D498261A0802EF63642BC3", // DigiCert High Assurance EV Root CA
                    };

                    app.UseTwitterAuthentication(
                            new TwitterAuthenticationOptions()
                            {
                                ConsumerKey = consumerKey,
                                ConsumerSecret = consumerSecret,
                                Provider = new TwitterAuthenticationProvider()
                                {
                                    OnAuthenticated = OnAuthenticatedWithTwitterAsync,
                                },
                                BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(twitterCerts),
                            });
                }
            }
        }

        /// <summary>
        /// Returns whether the specified authentication provider is enabled.
        /// </summary>
        /// <param name="name">The name of the authentication provider.</param>
        /// <returns>
        /// <see langword="true"/> if the provider specified by <paramref name="name"/>
        /// is enabled; otherwise <see langword="false"/>.
        /// </returns>
        private static bool IsAuthProviderEnabled(string name)
        {
            name = string.Format(CultureInfo.InvariantCulture, "{0}Auth:Enabled", name);
            return string.Equals(ConfigurationManager.AppSettings[name], bool.TrueString, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// A method that returns an asynchronous delegate for validating identities.
        /// </summary>
        /// <returns>
        /// A <see cref="Func{T, TResult}"/> that representing the identity validator.
        /// </returns>
        private static Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            // Enables the application to validate the security stamp when the user logs in.
            // This is a security feature which is used when you change a password or add an external login to your account.
            return SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                validateInterval: TimeSpan.FromMinutes(30),
                regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager));
        }

        /////// <summary>
        /////// Handles when a user is authenticated with <c>GitHub</c> as an asynchronous operation.
        /////// </summary>
        /////// <param name="context">The authentication context.</param>
        /////// <returns>
        /////// A <see cref="Task"/> representing the asynchronous operation.
        /////// </returns>
        ////private static async Task OnAuthenticatedWithGitHubAsync(GitHubAuthenticatedContext context)
        ////{
        ////    if (!context.Identity.HasClaim(ClaimTypes.GivenName, context.Name))
        ////    {
        ////        context.Identity.AddClaim(new Claim(ClaimTypes.GivenName, context.Name));
        ////    }
        ////
        ////    if (!context.Identity.HasClaim(ClaimTypes.Surname, context.Name))
        ////    {
        ////        context.Identity.AddClaim(new Claim(ClaimTypes.Surname, context.Name));
        ////    }
        ////
        ////    await Task.FromResult(0);
        ////}

        /// <summary>
        /// Handles when a user is authenticated with Google as an asynchronous operation.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private static async Task OnAuthenticatedWithGoogleAsync(GoogleOAuth2AuthenticatedContext context)
        {
            if (!context.Identity.HasClaim(ClaimTypes.GivenName, context.GivenName))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.GivenName, context.GivenName));
            }

            if (!context.Identity.HasClaim(ClaimTypes.Surname, context.FamilyName))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.Surname, context.FamilyName));
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Handles when a user is authenticated with Microsoft as an asynchronous operation.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private static async Task OnAuthenticatedWithMicrosoftAsync(MicrosoftAccountAuthenticatedContext context)
        {
            if (!context.Identity.HasClaim(ClaimTypes.GivenName, context.FirstName))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.GivenName, context.FirstName));
            }

            if (!context.Identity.HasClaim(ClaimTypes.Surname, context.LastName))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.Surname, context.LastName));
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Handles when a user is authenticated with Twitter as an asynchronous operation.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private static async Task OnAuthenticatedWithTwitterAsync(TwitterAuthenticatedContext context)
        {
            if (!context.Identity.HasClaim(IdentityConstants.TwitterScreenNameClaim, context.ScreenName))
            {
                context.Identity.AddClaim(new Claim(IdentityConstants.TwitterScreenNameClaim, context.ScreenName));
            }

            await Task.FromResult(0);
        }
    }
}
