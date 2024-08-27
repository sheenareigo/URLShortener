using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SendGrid;
using Shortly.Client.Data.ViewModels;
using Shortly.Client.Helpers.Roles;
using Shortly.Data;
using Shortly.Data.Models;
using Shortly.Data.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Security.Claims;


namespace Shortly.Client.Controllers
{
    public class AuthenticationController : Controller
    {
        private IUserService _userService;
        private SignInManager<User> _signinmanager;
        private UserManager<User> _userManager;
        private IConfiguration _configuration;

        public AuthenticationController(IUserService userService, SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
        {

            _userService = userService;
            _signinmanager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            ; }
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetUsers();
            return View(users);
        }


        public async Task<IActionResult> Login()
        {
            var login = new LoginVM()
            {

                schemes = await _signinmanager.GetExternalAuthenticationSchemesAsync()
            };
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            await _signinmanager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LoginSubmitted(LoginVM loginvm)
        {
           
            loginvm.schemes = await _signinmanager.GetExternalAuthenticationSchemesAsync();
            if (!ModelState.IsValid)
            {
                return View("Login", loginvm);
            }

            var useremail = await _userManager.FindByEmailAsync(loginvm.emailaddress);

            if (useremail != null)
            {
                var passwordcheck = await _userManager.CheckPasswordAsync(useremail, loginvm.password);
                if (passwordcheck)
                {
                    var userLoggedin = await _signinmanager.PasswordSignInAsync(useremail, loginvm.password, false, false);
                    if (userLoggedin.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");

                    }

                    else if (userLoggedin.IsNotAllowed)
                    {

                        return RedirectToAction("SendConfirmationEmail", useremail);


                    }
                    else if (userLoggedin.RequiresTwoFactor)
                    {
                        return RedirectToAction("TwoFactor", new { useremail.Id });

                    }
                    else
                    {

                        ModelState.AddModelError(" ", "Invalid login. Please check your user name or password");
                        return View("Login", loginvm);
                    }
                }
                else {

                    await _userManager.AccessFailedAsync(useremail);
                    if (await _userManager.IsLockedOutAsync(useremail))
                    {
                        ModelState.AddModelError(string.Empty, "Your Account is locked out due to 5 invalid login attempts. Please try again after 10 minutes");
                        return View("Login", loginvm);


                    }
                    else {
                        ModelState.AddModelError(string.Empty, "Please check your username or password");
                        return View("Login", loginvm);
                    }
                }

            }
            ModelState.AddModelError(" ", "Unregistered user! Please register with shortly to continue.");
            return View("Login", loginvm);

        }

        public async Task<IActionResult> Register()
        {
            var register = new RegisterVM()
            {

                schemes = await _signinmanager.GetExternalAuthenticationSchemesAsync()
            };
            return View(register);
           // return View(new RegisterVM());

        }

        public async Task<IActionResult> RegisterUser(RegisterVM registerVM)
        {
            registerVM.schemes = await _signinmanager.GetExternalAuthenticationSchemesAsync();
            if (!ModelState.IsValid)
            {
                return View("Register", registerVM);
            }
            var checkuserexists = await _userManager.FindByEmailAsync(registerVM.emailaddress);
            if (checkuserexists != null)
            {
                ModelState.AddModelError(string.Empty, "Email id already exists");
                return View("Register", registerVM);

            }
            else
            {

                //create a new user and assign it to user role

                var user = new User()
                {
                    Email = registerVM.emailaddress,
                    UserName = registerVM.username,
                    PhoneNumber = registerVM.phonenumber,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    LockoutEnabled = true

                };

                var createuser = await _userManager.CreateAsync(user, registerVM.password);
                if (createuser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Role.User);
                    return RedirectToAction("Login", "Authentication");


                }
                else
                {
                    foreach (var error in createuser.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Register", registerVM);
                    //ModelState.AddModelError(" ", "Invalid data. Please check your data entered and try again");
                    //return View("Register");

                }
            }
            //return RedirectToAction("Index","Home");

        }

        public async Task<IActionResult> SendConfirmationEmail(string email)
        {
            var confirmlogin = new ConfirmEmailVM()
            {
                emailaddress = email

            };
            return View(confirmlogin);

        }

        public async Task<IActionResult> TwoFactor(string id)
        {
            //get user
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {

                var smstoken = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
                string twiliofrom = _configuration["Twilio:PhoneNumber"];
                string twilioSID = _configuration["Twilio:SID"];
                string twiliotoken = _configuration["Twilio:Token"];

                TwilioClient.Init(twilioSID, twiliotoken);
                var message = MessageResource.Create(
                    body: $"This is your verification code : {smstoken}",
                    from: new Twilio.Types.PhoneNumber(twiliofrom),
                    to: new Twilio.Types.PhoneNumber(user.PhoneNumber));
                //send sms

                var twofa = new Confirm2FA()
                {
                    userId = user.Id

                };
                return View(twofa);

            }

            else
            {
                ModelState.AddModelError(string.Empty, "Invalid details");
                var twofa = new Confirm2FA()
                {
                    userId = user.Id

                };
                return View(twofa);
                // return View("SendConfirmationEmail", email);
            }


        }

        public async Task<IActionResult> ConfirmTwoFactor(Confirm2FA twofa)
        {
            var user = await _userManager.FindByIdAsync(twofa.userId);

            if (user != null)
            {
                var tokenverify = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", twofa.code);
                if (tokenverify)
                {
                    var tokensignin = await _signinmanager.TwoFactorSignInAsync("Phone", twofa.code, false, false);

                    if (tokensignin.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {

                        ModelState.AddModelError("", "Confirmation code is not right");
                        return View("Twofactor", twofa);
                    }

                }
                else
                {

                    ModelState.AddModelError("", "Confirmation code is not right");
                    return View("Twofactor", twofa);
                }

            }
            else
            {
                ModelState.AddModelError("", "Confirmation code is not right");
                return View(twofa);
            }
        }

        public async Task<IActionResult> ConfirmationEmail(ConfirmEmailVM email)
        {
            //check user exists
            var emailexists = await _userManager.FindByEmailAsync(email.emailaddress);
            if (emailexists != null)
            {
                //generate token

                var user_token = await _userManager.GenerateEmailConfirmationTokenAsync(emailexists);

                //send email

                var apiKey = _configuration["SendGrid:ShortlyKey"];
                var confirmationlink = Url.Action("VerifyEmail", "Authentication", new { userid = emailexists.Id, token = user_token }, Request.Scheme);
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(_configuration["SendGrid:FromAddress"], "Shortly Client App");
                var subject = "[Shortly] Verify your Email";
                var to = new EmailAddress(email.emailaddress);
                var plainTextContent = $"Hello from SheenaReigo_Shortly App. Please click the link to verify your account: {confirmationlink}";
                var htmlContent = $"Hello from SheenaReigo_Shortly App. Please click the link to verify your account: <a href=\"{confirmationlink}\">Verify your account</a>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);


                if (response.IsSuccessStatusCode)
                {
                    TempData["Email"] = "Thank You. Please check your regsitered email to verify your account";
                }
                else
                {
                    var responseContent = await response.Body.ReadAsStringAsync();


                    // Additional logging for headers or other details
                    foreach (var header in response.Headers)
                    {
                        Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
                    }
                    TempData["Email"] = ($"Failed to send email. Status Code: {responseContent}");
                }
                return RedirectToAction("Index", "Home");

            }

            else {
                ModelState.AddModelError(string.Empty, "Email address " + email.emailaddress + " does not exist");
                return View("SendConfirmationEmail", email);
            }



        }


        public async Task<IActionResult> VerifyEmail(string userid, string token)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {

                TempData["EmailVerified"] = "Account Verification Failed. Please try again later";
                return RedirectToAction("Index", "Home");
            }
            else {

                var verify = await _userManager.ConfirmEmailAsync(user, token);
                TempData["EmailVerified"] = "Your email has been successfully verified. Log in to continue";
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("Callback", "Authentication", new { return_url = returnUrl });
            var properties = _signinmanager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties); // sign in popup
        }

        public async Task<IActionResult> Callback(string remoteerror="", string returnUrl="")
        {
            var loginvm = new LoginVM()
            {
                schemes = await _signinmanager.GetExternalAuthenticationSchemesAsync()
            };
            if (!string.IsNullOrEmpty(remoteerror))
            {

                ModelState.AddModelError("", $"Error from External Login provider: {remoteerror}");
                return View("Login", loginvm);

            }
             
                var info = await _signinmanager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    ModelState.AddModelError("", $"Error from External Login provider: {remoteerror}");
                    return View("Login", loginvm);

                }
            var signinresult = await _signinmanager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signinresult.Succeeded)

            {

                return RedirectToAction("Index", "Home");
            }
           

                var useremail = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(useremail))
                    {
                    var user = await _userManager.FindByEmailAsync(useremail);
                    if (user == null)
                    {

                         user = new User()
                        {

                            UserName = useremail,
                            Email = useremail,
                            EmailConfirmed = true
                        };
                        await _userManager.CreateAsync(user);
                        await _userManager.AddToRoleAsync(user, Role.User);
                    
                    }
                await _signinmanager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");

            }

            ModelState.AddModelError("", $"Something went wrong");
            return View("Login", loginvm);

        }
    } }
