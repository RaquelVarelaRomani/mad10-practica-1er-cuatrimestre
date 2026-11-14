using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Security;
using Google.Apis.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    [TestClass]
    public class GoogleOAuthTest
    {
        private sealed class TestSession : HttpSessionStateBase
        {
            private readonly Dictionary<string, object> values = new Dictionary<string, object>();
            public override object this[string key]
            {
                get { object value; return values.TryGetValue(key, out value) ? value : null; }
                set { values[key] = value; }
            }
            public override void Remove(string key) { values.Remove(key); }
        }

        [TestMethod]
        public void ValidStateCanOnlyBeConsumedOnce()
        {
            var session = new TestSession();
            var request = GoogleLoginState.Begin(session, "/Pages/User/UpdateProfile.aspx", "/");
            Assert.AreSame(request, GoogleLoginState.Consume(session, request.State));
            Assert.IsNull(GoogleLoginState.Consume(session, request.State));
        }

        [TestMethod]
        public void MissingMismatchedOrForeignStateCannotBeConsumed()
        {
            var session = new TestSession();
            Assert.IsNull(GoogleLoginState.Consume(session, null));
            var request = GoogleLoginState.Begin(session, null, "/");
            Assert.IsNull(GoogleLoginState.Consume(new TestSession(), request.State));
            Assert.IsNull(GoogleLoginState.Consume(session, "attacker-state"));
            Assert.IsNull(GoogleLoginState.Consume(session, request.State));
        }

        [TestMethod]
        public void StateExpiresAfterTenMinutes()
        {
            var request = GoogleLoginState.Begin(new TestSession(), null, "/");
            Assert.IsTrue(request.IsValid(request.State, request.CreatedUtc.AddMinutes(5)));
            Assert.IsFalse(request.IsValid(request.State, request.CreatedUtc.AddMinutes(11)));
            Assert.IsFalse(request.IsValid(request.State, request.CreatedUtc.AddMinutes(-1)));
        }

        [TestMethod]
        public void AuthorizationUsesCodeFlowNonceAndPkceWithoutExposingSecret()
        {
            var request = GoogleLoginState.Begin(new TestSession(), null, "/");
            var config = new GoogleOAuthConfiguration("test-client", "test-secret", "https://localhost/callback");
            var uri = new Uri(request.GetAuthorizationUrl(config));
            var query = HttpUtility.ParseQueryString(uri.Query);
            Assert.AreEqual("accounts.google.com", uri.Host);
            Assert.AreEqual("code", query["response_type"]);
            Assert.AreEqual("openid email profile", query["scope"]);
            Assert.AreEqual("S256", query["code_challenge_method"]);
            Assert.AreEqual(request.Nonce, query["nonce"]);
            Assert.AreEqual(request.State, query["state"]);
            Assert.AreEqual(43, query["code_challenge"].Length);
            Assert.AreNotEqual(request.CodeVerifier, query["code_challenge"]);
            Assert.IsFalse(uri.Query.Contains("test-secret"));
            Assert.IsFalse(uri.Query.Contains(request.CodeVerifier));
            Assert.AreNotEqual(request.State, request.Nonce);
        }

        [TestMethod]
        public void ExternalOrMalformedReturnUrlsFallBackToMainPage()
        {
            foreach (string url in new[] { "https://evil.example", "//evil.example", "/\\evil.example",
                "/%5cevil.example", "/%2fevil.example", "/%0d%0aLocation:evil", "javascript:alert(1)",
                "/foo/..//evil.example" })
                Assert.AreEqual("/Pages/MainPage.aspx", GoogleLoginState.GetLocalReturnUrl(url, "/"), url);
        }

        [TestMethod]
        public void ReturnUrlsStayWithinVirtualApplication()
        {
            Assert.AreEqual("/shop/Pages/User/UpdateProfile.aspx",
                GoogleLoginState.GetLocalReturnUrl("~/Pages/User/UpdateProfile.aspx", "/shop"));
            Assert.AreEqual("/shop/Pages/MainPage.aspx", GoogleLoginState.GetLocalReturnUrl("/other-app/page", "/shop"));
            Assert.AreEqual("/shop/Pages/MainPage.aspx", GoogleLoginState.GetLocalReturnUrl("/shop/../other-app", "/shop"));
            Assert.AreEqual("/Pages/Product/FindProducts.aspx?keyword=phone",
                GoogleLoginState.GetLocalReturnUrl("/Pages/Product/FindProducts.aspx?keyword=phone", "/"));
            Assert.AreEqual("/Pages/Product/FindProducts.aspx?keyword=a%26b",
                GoogleLoginState.GetLocalReturnUrl("/Pages/Product/FindProducts.aspx?keyword=a%26b", "/"));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NonLocalRedirectUriRequiresHttps()
        {
            new GoogleOAuthConfiguration("client", "secret", "http://example.com/callback");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void EmptyCredentialsDisableGoogleOAuth()
        {
            new GoogleOAuthConfiguration("", "", "https://localhost/callback");
        }

        private static GoogleJsonWebSignature.Payload ValidIdentity()
        {
            return new GoogleJsonWebSignature.Payload
            {
                Subject = "123456789012345678901", Email = "user@example.com", EmailVerified = true,
                Nonce = "expected-nonce", GivenName = "Usuario", FamilyName = "Google", Locale = "gl-ES"
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidJwtException))]
        public void WrongNonceIsRejected()
        {
            GoogleOAuthClient.ValidateIdentity(ValidIdentity(), "other-request");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidJwtException))]
        public void UnverifiedEmailIsRejected()
        {
            var identity = ValidIdentity();
            identity.EmailVerified = false;
            GoogleOAuthClient.ValidateIdentity(identity, "expected-nonce");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidJwtException))]
        public void MissingSubjectIsRejected()
        {
            var identity = ValidIdentity();
            identity.Subject = null;
            GoogleOAuthClient.ValidateIdentity(identity, "expected-nonce");
        }

        [TestMethod]
        public void ProfileFitsExistingFieldsAndKeepsSupportedLanguage()
        {
            var identity = ValidIdentity();
            GoogleOAuthClient.ValidateIdentity(identity, "expected-nonce");
            identity.GivenName = new string('a', 50);
            identity.FamilyName = new string('b', 60);
            var details = GoogleOAuthClient.GetProfileDetails(identity);
            Assert.AreEqual(30, details.FirstName.Length);
            Assert.AreEqual(40, details.LastName.Length);
            Assert.AreEqual("gl", details.Language);
            Assert.AreEqual("ES", details.Country);
            identity.GivenName = null;
            identity.Locale = "fr-FR";
            Assert.AreEqual("user", GoogleOAuthClient.GetProfileDetails(identity).FirstName);
            Assert.AreEqual("es", GoogleOAuthClient.GetProfileDetails(identity).Language);
        }
    }
}
