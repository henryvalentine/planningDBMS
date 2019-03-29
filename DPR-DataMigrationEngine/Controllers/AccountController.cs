using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using DPR_DataMigrationEngine.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DPR_DataMigrationEngine.Controllers
{
    //[CustomAuthorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

      
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            //return Redirect("http://localhost:44300/UIAccount/Login2" + new QueryString(Request.GetOwinContext().Request.Uri.Query, Request.GetOwinContext().Request.Uri.AbsoluteUri));
            return View(new LoginViewModel());
        }

        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var user = await UserManager.FindAsync(model.UserName, model.Password);
                    //if (user != null)
                    //{
                    //    await SignInAsync(user, model.RememberMe);
                    //    return RedirectToLocal(returnUrl);
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("", "Invalid username or password.");
                    //    //var path1 = "http://localhost:44300/UIAccount/Login2" + new QueryString(Request.GetOwinContext().Request.Uri.Query, Request.GetOwinContext().Request.Uri.AbsoluteUri);
                    //    return View();
                    //}
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri("http://localhost:44300/api");
                        httpClient.SetBasicAuthentication(model.Username, model.Password);

                        var ccv = await httpClient.GetAsync("token");

                        if (ccv.IsSuccessStatusCode)
                        {
                            var tokenResponse = await ccv.Content.ReadAsStringAsync();
                            var json = JObject.Parse(tokenResponse);
                            var token = json["access_token"].ToString();

                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                            return Redirect("~/");
                        }
                    }

                    var headers = HttpContext.Response.Headers;

                    // Ensure that all of your properties are present in the current Request
                    if (!String.IsNullOrEmpty(headers["GUID"]) && !String.IsNullOrEmpty(headers["MEMBEROF"]) && !String.IsNullOrEmpty(headers["USERNAME"]))
                    {
                        return true; // or false based on verification
                    }
                    
                    // Request the token
                    var client = new HttpClient();
                    var response = "grant_type=password&username=" + model.UserName + "&password=" + model.Password;

                    var body = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", model.UserName),
                        new KeyValuePair<string, string>("password", model.Password)
                    };

                    var headers = HttpContext.Request.Headers;
                    headers.Add("UserName", model.UserName);
                    headers.Add("Password", model.Password);
                    const string userAuthorizationEndPoint = "http://localhost:44300/UIAccount/GetLogin";
                    var request = new HttpRequestMessage(HttpMethod.Post, userAuthorizationEndPoint);
                    //new QueryString(Request.GetOwinContext().Request.Uri.Query, Request.GetOwinContext().Request.Uri.AbsoluteUri)
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", response);
                    HttpResponseMessage graphResponse = await client.PostAsync(userAuthorizationEndPoint,new FormUrlEncodedContent(body), Request.GetOwinContext().Request.CallCancelled);
                    graphResponse.EnsureSuccessStatusCode();
                    var json = await graphResponse.Content.ReadAsStringAsync();
                    //JObject user = JObject.Parse(json);


                    var info = JsonConvert.DeserializeObject<ServiceResponse>(json);
                    if (info == null)
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error =
                            "The provided Account information could not be authenticated at the remote server.<br/> Please provide correct details and try again.";
                        return View("Login");
                    }

                    var authentication = HttpContext.GetOwinContext().Authentication;
                    //authentication.SignIn(new AuthenticationProperties { IsPersistent = false },
                    //        new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, "email") }, DefaultAuthenticationTypes.ApplicationCookie));

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, info.Name));
                    claims.Add(new Claim(ClaimTypes.Email, info.Email));
                    var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ExternalCookie);

                    var userRoles = info.UserRoles;
                    if (userRoles.Any())
                    {
                        userRoles.ForEach(m => claims.Add(new Claim(ClaimTypes.Role, m)));
                    }
                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    authenticationManager.SignIn(id);
                    //UserManager.IsLockedOut()
                }
                //var path = "http://localhost:44300/UIAccount/Login2" + new QueryString(Request.GetOwinContext().Request.Uri.Query, Request.GetOwinContext().Request.Uri.AbsoluteUri);
                return View();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(model);
            }
        }

        private async Task<ServiceResponse> GetResponse(Provider currentClient, string email, string password, string code, string state)
        {
            try
            {
                //exchange authorization code at authorization server for an access and refresh token
                var post = new Dictionary<string, string>
               {
                   {"client_id", currentClient.ProviderKey.ToString(CultureInfo.InvariantCulture)}
                   ,{"client_secret", currentClient.ProviderSecrete},
                   {"grant_type", "authorization_code"},
                   {"code", code},
                   {"redirect_uri", "http://localhost:44302/Account/AuthorizationCodeCallback"},
                   {"state", state}
               };

                var client = new HttpClient();
                var postContent = new FormUrlEncodedContent(post);
                var response = await client.PostAsync("http://localhost:44300/Token", postContent);

                if (response == null)
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The server returned an empty response. Please try again.";
                    return new ServiceResponse();
                }

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                //received tokens from authorization server
                var json = JObject.Parse(content);
                var accessToken = json["access_token"].ToString();
                var authorizationScheme = json["token_type"].ToString();
                var expiresIn = json["expires_in"].ToString();

                var serviceResponse = new ServiceResponse
                {
                    State = state,
                    AccessToken = accessToken,
                    AuthorizationScheme = authorizationScheme,
                    ExpiresIn = expiresIn
                };

                if (json["refresh_token"] != null)
                {
                    serviceResponse.RefreshToken = json["refresh_token"].ToString();
                }
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:44300/api/IdentityAccess/VerifyAccess");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Add("axud07", email);
                request.Headers.Add("pxud03", password);
                var usrResponse = await client.SendAsync(request);

                //var usrClient = new HttpClient();
                //var usrPost = new FormUrlEncodedContent(usrPayload);
                //var usrResponse = await usrClient.PostAsync("http://localhost:44300/api/IdentityAccess/VerifyAccess", usrPost);

                if (usrResponse == null)
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The server returned an empty response. Please try again.";
                    return new ServiceResponse();
                }

                usrResponse.EnsureSuccessStatusCode();

                var usrContent = await usrResponse.Content.ReadAsStringAsync();

                //var hdr = response.Headers.GetValues("AUSXDPTWX").ToList();

                if (usrContent == null)
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The server returned an empty response. Please try again.";
                    return new ServiceResponse();
                }

                var info = JsonConvert.DeserializeObject<ServiceResponse>(usrContent);
                if (info == null)
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The authenticaton server returned an empty response. Please try again.";
                    return new ServiceResponse();
                }
                
                serviceResponse.Email = info.Email;
                serviceResponse.UserId = info.UserId;
                serviceResponse.Name = info.Name;
                serviceResponse.UserRoles = info.UserRoles;
                return serviceResponse;
            }
            catch (Exception)
            {

                return new ServiceResponse();
            }
        }

        private ServiceResponse GetResponse2()
        {
            try
            {
                var vvk = Request.Params.GetValues("AUSXDPTWX");

                if (vvk == null || !vvk.Any())
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The authentication process failed. Please try again.";
                    return new ServiceResponse();
                }

                var info = JsonConvert.DeserializeObject<ServiceResponse>(vvk[0]);
                if (info == null)
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The authenticaton server returned an empty response. Please try again.";
                    return new ServiceResponse();
                }

                return info;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new ServiceResponse();
            }
        }

        private Provider GetAccessProvider()
        {
            try
            {
                var myApps = new ProviderServices().GetProviders();
                if (myApps == null || !myApps.Any())
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The application information is empty. Please contact an Administrator.";
                    return new Provider();
                }

                var currentClient = myApps[0];
                if (currentClient == null || currentClient.ProviderId < 1)
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The application information is empty. The authentication process failed.";
                    return new Provider();
                }
                return currentClient;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Provider();
            }
        }
        //[CustomAuthorize]
        public async Task<ActionResult> AuthorizationCodeCallback()
        {
            try
            {
                string state;
                string code;

                var currentClient = GetClientTicket();

                if (currentClient == null || currentClient.ProviderId < 1)
                {
                    currentClient = GetAccessProvider();
                    if (currentClient == null || currentClient.ProviderId < 1)
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error = "The authentication process failed. Please try again";
                        return View("Login");
                    }

                    SetClientTicket(currentClient);
                }

                ServiceResponse info;
                if (Session["_ssfd"] != null)
                {
                    state = (string)Session["_ssfd"];
                    
                    if (state == null)
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error = "The authentication process failed. Please try again.";
                        return View("Login");
                    }
                    var st1 = "";
                    var query = Request.GetOwinContext().Request.Query;
                    var values = query.GetValues("code");

                    if (values == null || values.Count < 1)
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error = "The authentication process failed. Please try again.";
                        return View("Login");

                    }

                    code = values[0];
                    values = query.GetValues("state");
                    if (values == null || values.Count < 1)
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error = "The authentication process failed. Please try again.";
                        return View("Login");

                    }

                    st1 = values[0];

                    if (String.IsNullOrEmpty(code) || string.IsNullOrEmpty(st1))
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error = "The server returned an empty response. Please try again.";
                        return View("Login");
                    }

                    if (state != st1)
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error = "A fraudulent activity was suspected. The process was terminated. Please try again.";
                        return View("Login");
                    }

                    if (string.IsNullOrWhiteSpace(state))
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error = "The authentication process failed. Please try again";
                        return View("Login");
                    }

                    var usrz = GetTicket();
                    if (usrz == null || string.IsNullOrWhiteSpace(usrz.EmailDPR_ID))
                    {
                        ViewBag.Code = -1;
                        ViewBag.Error = "The authentication process failed. Please try again";
                        return View("Login");
                    }
                       info = await GetResponse(currentClient, usrz.EmailDPR_ID, usrz.Password, code, state);
                    
                }

                else
                {
                    info = GetResponse2();
                }
                
                if (info == null || info.UserId < 1)
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The authentication process failed. Please try again.";
                    return View("Login");
                }
              
               var claims = new List<Claim>
               {
                   new Claim(ClaimTypes.Name, info.Name),
                   new Claim(ClaimTypes.Email, info.Email),
                   new Claim(ClaimTypes.NameIdentifier, info.UserId.ToString(CultureInfo.InvariantCulture)),
                   new Claim(ClaimTypes.Role, "Admin")
               };

               var userRoles = info.UserRoles;
               if (userRoles.Any())
               {
                   userRoles.ForEach(m => claims.Add(new Claim(ClaimTypes.Role, m)));
               }

               var claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                var principal = new ClaimsPrincipal(new[] { claimsIdentity });
                Thread.CurrentPrincipal = principal;
                HttpContext.User = principal;

                var serializeModel = new CustomPrincipalSerializeModel
                {
                    Email = info.Email,
                    UserId = info.UserId,
                    FirstName = info.Name,
                    UserRoles = info.UserRoles
                };
                
                Session["_sessAXDwxg"] = info;

                var userData = JsonConvert.SerializeObject(serializeModel);
                var authTicket = new FormsAuthenticationTicket(1, info.Email, DateTime.Now, DateTime.Now.AddMinutes(15),
                false, userData);

                var encTicket = FormsAuthentication.Encrypt(authTicket);
                var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(faCookie);

                authenticationManager.SignIn(claimsIdentity);

                return RedirectToLocal("");
            
            }
            catch (Exception ex)
            {
                ViewBag.Code = -1;
                ViewBag.Error = "The authenticaton server returned an error. Please try again.";
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View("Login");
            }
        }

        [Authorize]
        public async Task AppFeedback()
        {
            try
            {
                var currentClient = GetClientTicket();

                if (currentClient == null)
                {
                    RedirectToAction("LogOff");
                }
                string code = null;
                string state = null;

                var query = Request.GetOwinContext().Request.Query;
                var values = query.GetValues("code");
                if (values != null && values.Count == 1)
                {
                    code = values[0];
                }
                values = query.GetValues("state");
                if (values != null && values.Count == 1)
                {
                    state = values[0];
                }

                if (String.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
                {
                    RedirectToAction("LogOff");
                }

                //exchange authorization code at authorization server for an access and refresh token
                var post = new Dictionary<string, string>
               {
                   {"client_id", currentClient.ProviderKey.ToString(CultureInfo.InvariantCulture)}
                   ,{"client_secret", currentClient.ProviderSecrete},
                   {"grant_type", "authorization_code"},
                   {"code", code},
                   {"redirect_uri", currentClient.RedirectUrl},
                   {"state", state}
               };

                var client = new HttpClient();
                var postContent = new FormUrlEncodedContent(post);
                var response = await client.PostAsync("http://localhost:44300/Token", postContent);

                if (response == null)
                {
                    RedirectToAction("LogOff");
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // received tokens from authorization server
                var json = JObject.Parse(content);
                var accessToken = json["access_token"].ToString();
                var authorizationScheme = json["token_type"].ToString();
                var expiresIn = json["expires_in"].ToString();


                var lgVm = GetTicket();
                if (lgVm == null)
                {
                    RedirectToAction("LogOff");
                }

                if (Session["ausxdPO7"] == null)
                {
                    RedirectToAction("LogOff");
                }

                var serviceResponse = Session["ausxdPO7"] as ServiceResponse;
                if (serviceResponse == null || serviceResponse.UserId < 1)
                {
                    RedirectToAction("LogOff");
                }

                serviceResponse.AccessToken = accessToken;
                serviceResponse.AuthorizationScheme = authorizationScheme;
                serviceResponse.ExpiresIn = expiresIn;

                if (json["refresh_token"] != null)
                {
                    serviceResponse.RefreshToken = json["refresh_token"].ToString();
                }

                var infoStr = JsonConvert.SerializeObject(serviceResponse);
                var uSxStr = JsonConvert.SerializeObject(lgVm);
                //var fgvt = new Dictionary<string, string>
                //{
                //    {"AUSXDP7", infoStr},
                //    {"AUSXDLG9", uSxStr}
                //};

                Request.GetOwinContext().Request.Headers.Append("AUSXDPTWX", infoStr);
                Request.GetOwinContext().Request.Headers.Append("AUSXDLG9", uSxStr);

                var payLoad = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
               {
                   {"AUSXDPTWX", infoStr},
                   {"AUSXDLG9", uSxStr}
               };


                var payLoadEnc = new FormUrlEncodedContent(payLoad);
                var payLoadEncStr = await payLoadEnc.ReadAsStringAsync();

                // Request.GetOwinContext().Response.Redirect("http://localhost:44302/Account/AuthorizationCodeCallback");
                Response.RedirectPermanent("http://localhost:44302/Account/AuthorizationCodeCallback?" + payLoadEncStr);
                //return RedirectToAction("GetAppChoice");
            }
            catch (Exception ex)
            {
                Response.Redirect("LogOff");
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static readonly RandomNumberGenerator Random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                var strengthInBytes = strengthInBits / bitsPerByte;

                var data = new byte[strengthInBytes];
                Random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        private bool SetClientTicket(Provider model)
        {
            try
            {

                Session["_CTZ"] = model;
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        private bool SetTicket(LoginViewModel model)
        {
            try
            {
                Session["_usxZ"] = model;
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        private Provider GetClientTicket()
        {
            try
            {
                if (Session["_CTZ"] == null)
                {
                    return new Provider();
                }

                var ffc = Session["_CTZ"] as Provider;
                if (ffc == null || ffc.ProviderId < 1)
                {
                    return ffc;
                }

                return ffc;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Provider();
            }
        }

        private LoginViewModel GetTicket()
        {
            try
            {
                if (Session["_usxZ"] == null)
                {
                    return new LoginViewModel();
                }

                var ffc = Session["_usxZ"] as LoginViewModel;
                if (ffc == null || string.IsNullOrEmpty(ffc.EmailDPR_ID))
                {
                    return new LoginViewModel();
                }

                return ffc;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new LoginViewModel();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginConnect(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrEmpty(model.EmailDPR_ID))
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "Please provide correct details and try again.";
                    return View("Login");
                }

                var myApps = new ProviderServices().GetProviders();
                if (myApps == null || !myApps.Any())
                {
                    ViewBag.Code = -1;
                    ViewBag.Error = "The application information is empty. Please contact an Administrator.";
                    return View("Login");
                }

                var myApp = myApps[0];
                const int strengthInBits = 256;
                var state = RandomOAuthStateGenerator.Generate(strengthInBits);

                var authorizeArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    {"response_type", "code"},
                    {"redirect_uri", myApp.RedirectUrl},
                    {"client_id", myApp.ProviderKey.ToString(CultureInfo.InvariantCulture)},
                    {"client_secret", myApp.ProviderSecrete},
                    {"scope", "userId"},
                    {"state", state},
                    {"usAvx", model.EmailDPR_ID},
                    {"fssPlk", model.Password}
                };
                
                if (!SetClientTicket(myApp))
                {
                    return RedirectToAction("Login", new LoginViewModel());
                }
                
                if (!SetTicket(model))
                {
                    return RedirectToAction("Login", new LoginViewModel());
                }
                Session["_ssfd"] = state;
                var content = new FormUrlEncodedContent(authorizeArgs);
                var contentAsString = await content.ReadAsStringAsync();
                return Redirect("http://localhost:44300/UIAccount/SetupResponseChallenge?" + contentAsString);

            }
            catch (WebException ex)
            {
                ViewBag.Code = -1;
                ViewBag.Error = "An error was encountered. Please try again.";
                return View("Login");
            }
        }

        [Authorize]
        private async Task<ActionResult> SetUpChallengeAsync(Provider app)
        {
            try
            {
                const int strengthInBits = 256;
                var state = RandomOAuthStateGenerator.Generate(strengthInBits);

                var authorizeArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
               {
                   {"response_type", "code"},
                   {"redirect_uri", app.RedirectUrl},
                   {"client_id", app.ProviderKey.ToString(CultureInfo.InvariantCulture)},
                   {"client_secret", app.ProviderSecrete},
                   {"scope", "userId"},
                   {"state", state}
               };

                if (!SetClientTicket(app))
                {
                    return RedirectToAction("Login", new LoginViewModel());
                }

                var content = new FormUrlEncodedContent(authorizeArgs);
                var contentAsString = await content.ReadAsStringAsync();
                return Redirect("http://localhost:44300/OAuth/Authorize?" + contentAsString);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return RedirectToAction("LogOff");
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var newUser =  new GenericValidator();
            try
            {
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,

                    UserInfo = new ApplicationDbContext.UserProfile
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
                    }
                  };

                    var context = new ApplicationDbContext();
                    var userRoles = new List<string>();
                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        if (model.UserRoles.Any())
                        {
                            model.UserRoles.ForEach(m =>
                            {
                                if (!user.Roles.ToList().Exists(f => f.RoleId == m))
                                {
                                    var yx = context.Roles.Find(m);
                                    if (yx != null)
                                    {
                                        UserManager.AddToRole(user.Id, yx.Name);
                                        userRoles.Add(yx.Name);
                                    }
                                    
                                }
                            });
                        }

                        var appUser = new AppUser
                                        {
                                           Id = user.Id,
                                           Error = "User successfully created",
                                           Code = 5,
                                           FirstName = model.FirstName,
                                           LastName = model.LastName,
                                           Email = model.Email,
                                           UserName = model.UserName,
                                           UserRoles = userRoles
                                        };
                        return Json(appUser, JsonRequestBehavior.AllowGet);
                    }
                    var txx = newUser.Error = result.Errors.ElementAt(0);
                    newUser.Error = txx;
                    newUser.Code = -1;
                    return Json(newUser, JsonRequestBehavior.AllowGet);
               
            }
            catch (Exception ex)
            {
                newUser.Error = "User could not be created.";
                newUser.Code = -1;
                return Json(newUser, JsonRequestBehavior.AllowGet);
            }
        }

        #region Loggedon User Profile
        public ActionResult MyProfile()
        {
            try
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null && !string.IsNullOrEmpty(user.Id))
                {
                    var appUser = new RegisterViewModel
                    {
                        UserName = user.UserName,
                        Email = user.UserInfo.Email,
                        FirstName = user.UserInfo.FirstName,
                        LastName = user.UserInfo.LastName
                    };
                    ViewBag.Code = 0;
                    Session["_loggedOnUser"] = user;
                    return View(appUser);
                }

                ViewBag.Error = "Your Profile could not be retrieved";
                ViewBag.Code = -1;
                return View(new RegisterViewModel());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Your Profile could not be retrieved";
                ViewBag.Code = -1;
                return View(new RegisterViewModel());
            }
        }
        [HttpPost]
        public async Task<ActionResult> UpdateLoggedOnUserProfile(RegisterViewModel model)
        {
            try
            {
                if (Session["_loggedOnUser"] == null)
                {
                    ViewBag.Error = "Session has expired";
                    ViewBag.Code = -1;
                    return View("MyProfile", model);
                }

                var userinfo = Session["_loggedOnUser"] as ApplicationUser;
                if (userinfo == null || string.IsNullOrEmpty(userinfo.Id))
                {
                    ViewBag.Error = "Session has expired";
                    ViewBag.Code = -1;
                    return View("MyProfile", model);
                }
                var context = new ApplicationDbContext();
                var store = new UserStore<ApplicationUser>(context);
                var userId = userinfo.Id;
                var cUser = await store.FindByIdAsync(userId);
                if (cUser == null || string.IsNullOrEmpty(cUser.Id))
                {
                    ViewBag.Error = "Fatal error: User Information could not be retrieved.";
                    ViewBag.Code = -1;
                    return View("MyProfile", model);
                }
                cUser.UserName = model.UserName;
                cUser.UserInfo.Email = model.Email;
                cUser.UserInfo.FirstName = model.FirstName;
                cUser.UserInfo.LastName = model.LastName;

                await store.UpdateAsync(cUser);
 
               

                //var result = await UserManager.UpdateAsync(userinfo);

                //if (result.Succeeded)
                //{
                    ViewBag.Error = "Your Profile was successfully Updated";
                    ViewBag.Code = 5;
                    return View("MyProfile", model);
                //}
                //ViewBag.Error = "Your Profile could not be updated";
                //ViewBag.Code = -1;
                //return View("MyProfile", model);


            }
            catch (Exception ex)
            {
                ViewBag.Error = "Your Profile could not be updated";
                ViewBag.Code = -1;
                return View("MyProfile", model);
            }
        }
        #endregion

        #region Manage User Profile
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult GetUserProfile(string userId)
        {
            var validator = new AppUser();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    validator.Error = "User information could not be retrieved";
                    validator.Code = -1;
                    return Json(validator, JsonRequestBehavior.AllowGet);
                }

                var user = UserManager.FindById(userId);
                if (user != null && !string.IsNullOrEmpty(user.Id))
                {
                    var appUser = new AppUser
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.UserInfo.Email,
                        FirstName = user.UserInfo.FirstName,
                        LastName = user.UserInfo.LastName
                    };

                    Session["_user"] = user;
                    appUser.Error = "";
                    appUser.Code = 5;
                    return Json(appUser, JsonRequestBehavior.AllowGet);
                }

                validator.Error = "User information could not be retrieved";
                validator.Code = -1;
                return Json(validator, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                validator.Error = "User information could not be updated";
                validator.Code = -1;
                return Json(validator, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> UpdateUserProfile(RegisterViewModel model)
        {
            var validator = new GenericValidator();
            try
            {  
                if (Session["_user"] == null)
                    {
                        validator.Error = "Session has expired";
                        validator.Code = -1;
                        return Json(validator, JsonRequestBehavior.AllowGet);
                    }

                    var userinfo = Session["_user"] as ApplicationUser;
                    if (userinfo == null || string.IsNullOrEmpty(userinfo.Id))
                    {
                        validator.Error = "Session has expired";
                        validator.Code = -1;
                        return Json(validator, JsonRequestBehavior.AllowGet);
                    }

                    var context = new ApplicationDbContext();
                    var store = new UserStore<ApplicationUser>(context);
                    var userId = userinfo.Id;
                    var cUser = await store.FindByIdAsync(userId);

                    if (cUser == null || string.IsNullOrEmpty(cUser.Id))
                    {
                        validator.Error = "Fatal error: User Information could not be retrieved.";
                        validator.Code = -1;
                        return Json(validator, JsonRequestBehavior.AllowGet);
                    }
                    cUser.UserName = model.UserName;
                    cUser.UserInfo.Email = model.Email;
                    cUser.UserInfo.FirstName = model.FirstName;
                    cUser.UserInfo.LastName = model.LastName;

                    await store.UpdateAsync(cUser);

                    //var result = await UserManager.UpdateAsync(userinfo);

                    //if (result.Succeeded)
                    //{


                    validator.UserName = model.UserName;
                    validator.Email = model.Email;
                    validator.Error = "User successfully Updated";
                    validator.Code = 5;
                    return Json(validator, JsonRequestBehavior.AllowGet);
                    //}
                    //validator.Error = "User information could not be updated";
                    //validator.Code = -1;
                    //return Json(validator, JsonRequestBehavior.AllowGet);
                
                
            }
            catch (Exception ex)
            {
                validator.Error = ex.Message;
                validator.Code = -1;
                return Json(validator, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Manage User Roles
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult GetUserRoles(string userId)
        {
            // User.Identity.GetUserId()
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new List<string>(), JsonRequestBehavior.AllowGet);
                }

                var user = UserManager.FindById(userId);
                if (user != null && !string.IsNullOrEmpty(user.Id))
                {
                    var context = new ApplicationDbContext();
                    var roles = context.Roles.ToList();
                    var userRoles = new List<string>();

                    if (user.Roles.Any())
                    {
                        userRoles.AddRange(user.Roles.Select(userRole => roles.Find(role => role.Id == userRole.RoleId).Id));
                    }
                    Session["_userForRoles"] = user;
                    Session["_userRoles"] = userRoles;
                    return Json(userRoles, JsonRequestBehavior.AllowGet);
                }

                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateUserRoles(List<string> selecteUserRoles, string userId) 
        {
            // User.Identity.GetUserId()
            var appUser = new AppUser();
            if (string.IsNullOrEmpty(userId))
            {
                appUser.Code = -1;
                appUser.Error = "User information could not be retrieved";
                return Json(appUser, JsonRequestBehavior.AllowGet);
            }
            if (!selecteUserRoles.Any())
            {
                appUser.Code = -1;
                appUser.Error = "Please select at least one role";
                return Json(appUser, JsonRequestBehavior.AllowGet);
            }

            if (Session["_userRoles"] == null)
            {
                appUser.Code = -1;
                appUser.Error = "Session has expired.";
                return Json(appUser, JsonRequestBehavior.AllowGet);
            }

            var userRoles = Session["_userRoles"] as List<string>;

            if (userRoles == null)
            {
                appUser.Code = -1;
                appUser.Error = "Session has expired.";
                return Json(appUser, JsonRequestBehavior.AllowGet);
            }

            var context = new ApplicationDbContext();
            var user = UserManager.FindById(userId);
            if (user != null && !string.IsNullOrEmpty(user.Id))
            {
                selecteUserRoles.ForEach(m =>
                {
                    if (!user.Roles.ToList().Exists(f => f.RoleId == m))
                    {
                        var yx = context.Roles.Find(m);
                        if (yx != null)
                        {
                            UserManager.AddToRole(userId, yx.Name);
                        }
                        
                    }
                });
                
                userRoles.ForEach(k =>
                {
                    if (!selecteUserRoles.Exists(r => r == k))
                    {
                        var yx = context.Roles.Find(k);
                        if (yx != null)
                        {
                            UserManager.RemoveFromRole(userId, yx.Name);
                        }
                    }
                });

                appUser.Code = 5;
                appUser.Error = "User Roles successfully updated";
                appUser.UserRoles = new List<string>();
                foreach (var roleId in selecteUserRoles)
                {
                    var yx = context.Roles.Find(roleId);
                    if (yx != null)
                    {
                        appUser.UserRoles.Add(yx.Name);
                    }
                }
                return Json(appUser, JsonRequestBehavior.AllowGet);
            }

            appUser.Code = -1;
            appUser.Error = "User Roles information could not be updated";
            return Json(appUser, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ManageUserProfiles()
        {
            // User.Identity.GetUserId()
            
            var context = new ApplicationDbContext();
            var users = context.Users.ToList();
            var roles = context.Roles.ToList();
            if (users.Any() && roles.Any())
            {
                users.Remove(users.Find(m => m.Id == "897d2b71-43d3-4751-a672-32c5216093f5"));
                roles.Remove(roles.Find(m => m.Id == "5d411b4e-ee52-44c8-8776-511db06eceff"));
                var tp = Tuple.Create(users.OrderBy(x => x.UserName).ToList(), roles.OrderBy(m => m.Name).ToList());
                return View(tp);
            }
            return View(Tuple.Create(new List<ApplicationUser>(), roles));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }
        
        public ActionResult Manage(ManageMessageId? message)
        {
            
            if (message == ManageMessageId.ChangePasswordSuccess)
            {
                ViewBag.StatusMessage = "Your password has been changed.";
                ViewBag.ManageSuccess = 2;
            }

             if (message == ManageMessageId.SetPasswordSuccess)
            {
                ViewBag.StatusMessage = "Your password has been set.";
                ViewBag.ManageSuccess = 2;
            }

             if (message == ManageMessageId.RemoveLoginSuccess)
            {
                ViewBag.StatusMessage =  "The external login was removed.";
                ViewBag.ManageSuccess = 2;
            }

             if (message == ManageMessageId.Error)
            {
                ViewBag.StatusMessage =  "An error has occurred.";
                ViewBag.ManageSuccess = -2;
            }
           
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> UpdateUserPassword(ManageUserViewModel model)
        {
            try
            {

                if (Roles.IsUserInRole("Admin"))
                {
                    if (string.IsNullOrEmpty(model.Id))
                    {
                        return Json(new AppUser { Code = -1, Error = "Invalid selection" }, JsonRequestBehavior.AllowGet);
                    }
                    if (string.IsNullOrEmpty(model.NewPassword))
                    {
                        return Json(new AppUser { Code = -1, Error = "Please provide user's new Password" }, JsonRequestBehavior.AllowGet);
                    }
                    var context = new ApplicationDbContext();
                    var store = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(store);
                    var userId = model.Id;
                    var newPassword = model.NewPassword;
                    var hashedNewPassword = userManager.PasswordHasher.HashPassword(newPassword);
                    var cUser = await store.FindByIdAsync(userId);
                    await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                    await store.UpdateAsync(cUser);
                    var ft = new AppUser { Code = 2, Error = "User Password was successfully changed" };
                    return Json(ft, JsonRequestBehavior.AllowGet);
                }

                return Json(new AppUser { Code = -1, Error = "You do not have the privilege to Perform this Task" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                var ft = new AppUser { Code = -1, Error = "User Password could not be changed. Please try again or contact a system Admin." };
                return Json(ft, JsonRequestBehavior.AllowGet);
            }
        }
        
        [CustomAuthorize]
        [HttpPost]
       //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");

            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result =  UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        ViewBag.ManageSuccess = 2;
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                        
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }
        
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            try
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                var fgt = identity;
                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, fgt);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace,ex.Source,ex.Message);
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess = 1,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
           
        //    return RedirectToAction("Index", "Home");
        //}

        private ActionResult RedirectToLocal(string returnUrl)
        {
            var ff = Request.QueryString["ReturnUrl"];
            if (string.IsNullOrWhiteSpace(ff))
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }


            if (Request.QueryString["ReturnUrl"] == "https://localhost:7683/")
            {
                return RedirectToAction("Index", "Home");

            }

            if (Request.QueryString["ReturnUrl"] == "/")
            {
                return RedirectToAction("Index", "Home");

            }

            var redir = Request.QueryString["ReturnUrl"];

            return Redirect(redir);
            //Url.IsLocalUrl(returnUrl)
        }
        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}