﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Litium.Accelerator.Builders.Login;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels.Login;
using Litium.Customers;
using Litium.Globalization;
using Litium.Sales;
using Litium.Security;
using Litium.Web;
using Microsoft.AspNetCore.Mvc;

namespace Litium.Accelerator.Mvc.Controllers.Login
{
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;
        private readonly MyPagesViewModelService _myPagesViewModelService;
        private readonly LoginViewModelBuilder _loginViewModelBuilder;
        private readonly ForgotPasswordViewModelBuilder _forgotPasswordViewModelBuilder;
        private readonly OrganizationService _organizationService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly AddressTypeService _addressTypeService;
        private readonly PersonStorage _personStorage;
        private readonly CountryService _countryService;
        private readonly SecurityContextService _securityContextService;
        private readonly PersonService _personService;

        public LoginController(
            LoginService loginService,
            LoginViewModelBuilder loginViewModelBuilder,
            ForgotPasswordViewModelBuilder forgotPasswordViewModelBuilder,
            OrganizationService organizationService,
            MyPagesViewModelService myPagesViewModelService,
            RequestModelAccessor requestModelAccessor,
            AddressTypeService addressTypeService,
            CountryService countryService,
            SecurityContextService securityContextService,
            PersonStorage personStorage,
            PersonService personService)
        {
            _loginService = loginService;
            _loginViewModelBuilder = loginViewModelBuilder;
            _forgotPasswordViewModelBuilder = forgotPasswordViewModelBuilder;
            _organizationService = organizationService;
            _myPagesViewModelService = myPagesViewModelService;
            _requestModelAccessor = requestModelAccessor;
            _addressTypeService = addressTypeService;
            _personStorage = personStorage;
            _personService = personService;
            _countryService = countryService;
            _securityContextService = securityContextService;
        }

        [HttpGet]
        public virtual ActionResult Login(string redirectUrl, string code)
        {
            var model = _loginViewModelBuilder.Build(redirectUrl ?? string.Empty);
            if (code != null && int.TryParse(code, out var resultCode) && Enum.IsDefined(typeof(HttpStatusCode), resultCode))
            {
                model.ErrorMessage = $"error.{resultCode}".AsWebsiteText();
                if (User.Identity.IsAuthenticated && (resultCode == (int)HttpStatusCode.Unauthorized || resultCode == (int)HttpStatusCode.Forbidden))
                {
                    model.InsufficientPermissions = true;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            var model = _loginViewModelBuilder.Build(loginViewModel);
            model.RedirectUrl = loginViewModel.RedirectUrl;

            if (!_loginService.IsValidLoginForm(ModelState, model.LoginForm))
            {
                return View(model);
            }

            var cartContext = HttpContext.GetCartContext();
            try
            {
                var loginSuccessfull = _loginService.Login(model.LoginForm.UserName, model.LoginForm.Password);
                if (loginSuccessfull)
                {
                    var person = _loginService.GetUser(model.LoginForm.UserName);
                    var addressType = _addressTypeService.Get(AddressTypeNameConstants.Address);

                    if (!_loginService.IsBusinessCustomer(person, out var organizations))
                    {
                        await SetPersonAsync(cartContext, person);
                        await SetCountryByAddressAsync(cartContext, person.Addresses.FirstOrDefault(x => x.AddressTypeSystemId == addressType.SystemId));
                        return new RedirectResult(model.RedirectUrl);
                    }

                    if (organizations.Count == 1)
                    {
                        await SetOrganizationAsync(cartContext, organizations.First().SystemId);
                        await SetCountryByAddressAsync(cartContext, organizations.First().Addresses.FirstOrDefault(x => x.AddressTypeSystemId == addressType.SystemId));
                        return new RedirectResult(model.RedirectUrl);
                    }

                    return RedirectToAction(nameof(SelectOrganization), new { redirectUrl = model.RedirectUrl });
                }
                model.ErrorMessage = "login.failed".AsWebsiteText();
            }
            catch (ChangePasswordException)
            {
                return View(nameof(ChangePassword), model);
            }

            return View(model);
        }


        [HttpGet]
        public virtual ActionResult SelectOrganization(string redirectUrl)
        {
            var model = new LoginViewModel();
            var currentPersonId = _securityContextService.GetIdentityUserSystemId();
            if (currentPersonId.HasValue)
            {
                var person = _personService.Get(currentPersonId.Value);
                if (_loginService.IsBusinessCustomer(person, out var organizations))
                {
                    model.Organizations = _loginViewModelBuilder.GetOrganizations(organizations);
                    model.RedirectUrl = redirectUrl;
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LoginViewModel loginViewModel)
        {
            if (string.IsNullOrWhiteSpace(loginViewModel.LoginForm.UserName) || string.IsNullOrWhiteSpace(loginViewModel.LoginForm.Password))
            {
                return RedirectToAction(nameof(Login));
            }

            var model = _loginViewModelBuilder.Build(loginViewModel);
            model.RedirectUrl = loginViewModel.RedirectUrl;

            if (!_myPagesViewModelService.IsValidPasswordForm(ModelState, model.ChangePasswordForm,
                model.LoginForm.Password))
            {
                return View(model);
            }

            var loginSuccessfull = _loginService.Login(model.LoginForm.UserName, model.LoginForm.Password, model.ChangePasswordForm.Password);
            if (!loginSuccessfull)
            {
                return View(model);
            }
            var person = _loginService.GetUser(model.LoginForm.UserName);
            if (!_loginService.IsBusinessCustomer(person, out var organizations))
            {
                return new RedirectResult(model.RedirectUrl);
            }

            if (organizations.Count <= 1)
            {
                return new RedirectResult(model.RedirectUrl);
            }
            model.Organizations = _loginViewModelBuilder.GetOrganizations(organizations);
            return View(nameof(SelectOrganization), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectOrganization(LoginViewModel loginViewModel)
        {
            var cartContext = HttpContext.GetCartContext();
            await SetOrganizationAsync(cartContext, loginViewModel.SelectedOrganization);
            if (_personStorage.CurrentSelectedOrganization != null)
            {
                var addressType = _addressTypeService.Get(AddressTypeNameConstants.Address);
                await SetCountryByAddressAsync(cartContext, _personStorage.CurrentSelectedOrganization.Addresses.FirstOrDefault(x => x.AddressTypeSystemId == addressType.SystemId));
                return new RedirectResult(loginViewModel.RedirectUrl);
            }

            _loginService.Logout();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public virtual ActionResult ForgotPassword()
        {
            var model = _forgotPasswordViewModelBuilder.Build();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var model = _forgotPasswordViewModelBuilder.Build(forgotPasswordViewModel.ForgotPasswordForm);

            if (_loginService.IsValidForgotPasswordForm(ModelState, model.ForgotPasswordForm))
            {
                var user = _loginService.GetUser(model.ForgotPasswordForm.Email?.ToLower());
                if (user == null)
                {
                    model.ErrorMessage = "login.usernotfound".AsWebsiteText();
                }
                else
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        if (_loginService.ChangePassword(user, false, true, model.ChannelSystemId))
                        {
                            model.Message = "login.passwordsent".AsWebsiteText();
                        }
                        else
                        {
                            model.ErrorMessage = "login.passwordcouldnotbesent".AsWebsiteText();
                        }
                    }
                    else
                    {
                        model.ErrorMessage = "login.passwordcouldnotbesent".AsWebsiteText();
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public RedirectResult Logout(string redirectUrl)
        {
            _loginService.Logout();

            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = "~/";
            }
            return new RedirectResult(redirectUrl);
        }

        private async Task SetCountryByAddressAsync(CartContext cartContext, Customers.Address address)
        {
            //Check if the country in the address is same as channel has.
            if (address != null && !string.IsNullOrEmpty(address.Country) && !address.Country.Equals(_requestModelAccessor.RequestModel.CountryModel.Country.Id, StringComparison.CurrentCultureIgnoreCase))
            {
                var country = _countryService.Get(address.Country);
                //Check if country is connected to the channel
                if (country != null && _requestModelAccessor.RequestModel.ChannelModel.Channel.CountryLinks.Any(x => x.CountrySystemId == country.SystemId))
                {
                    // Set user's country to the channel
                    await cartContext.SelectCountryAsync(new SelectCountryArgs { CountryCode = country.Id });
                }
            }
        }

        private async Task SetOrganizationAsync(CartContext cartContext, Guid organizationSystemId)
        {
            _personStorage.CurrentSelectedOrganization = _organizationService.Get(organizationSystemId);

            if (cartContext != null && cartContext.OrganizationSystemId != organizationSystemId)
            {
                await cartContext.ChangeCustomerAsync(new ChangeCustomerArgs
                                                   {
                                                       CustomerNumber = _personStorage.CurrentSelectedOrganization.Id,
                                                       CustomerType = CustomerType.Company,
                                                       PersonSystemId = _securityContextService.GetIdentityUserSystemId().GetValueOrDefault(),
                                                       OrganizationSystemId = organizationSystemId
                                                   });
            }
        }

        private async Task SetPersonAsync(CartContext cartContext, Person person)
        {
            if (cartContext != null && cartContext.PersonSystemId != person.SystemId)
            {
                await cartContext.ChangeCustomerAsync(new ChangeCustomerArgs
                                                   {
                                                       CustomerNumber = person.Id,
                                                       CustomerType = CustomerType.PrivatePerson,
                                                       PersonSystemId = person.SystemId,
                                                       OrganizationSystemId = null
                                                   });
            }
        }
    }
}
