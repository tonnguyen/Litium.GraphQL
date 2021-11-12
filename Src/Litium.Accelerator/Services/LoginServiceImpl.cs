﻿using Litium.Accelerator.Constants;
using Litium.Accelerator.Mailing;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels.Login;
using Litium.Customers;
using Litium.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Litium.Web;
using Microsoft.Extensions.Logging;

namespace Litium.Accelerator.Services
{
    public class LoginServiceImpl : LoginService
    {
        private readonly AuthenticationService _authenticationService;
        private readonly SecurityContextService _securityContextService;
        private readonly PersonService _personService;
        private readonly OrganizationService _organizationService;
        private readonly RoleService _roleService;
        private readonly MailService _mailService;
        private readonly UserValidationService _userValidationService;
        private readonly PersonStorage _personStorage;
        private readonly CheckoutState _checkoutState;
        private readonly ILogger<LoginServiceImpl> _logger;

        public LoginServiceImpl(
            AuthenticationService authenticationService,
            SecurityContextService securityContextService,
            PersonService personService,
            OrganizationService organizationService,
            RoleService roleService,
            MailService mailService,
            UserValidationService userValidationService,
            PersonStorage personStorage,
            CheckoutState checkoutState,
            ILogger<LoginServiceImpl> logger)
        {
            _authenticationService = authenticationService;
            _securityContextService = securityContextService;
            _personService = personService;
            _organizationService = organizationService;
            _roleService = roleService;
            _mailService = mailService;
            _userValidationService = userValidationService;
            _personStorage = personStorage;
            _checkoutState = checkoutState;
            _logger = logger;
        }

        public override string GeneratePassword()
        {
            var pass = Security.RandomStringGenerator.Generate(4, 4, 0, 2);
            return _userValidationService.IsValidPasswordComplexity(pass) ? pass : Security.RandomStringGenerator.Generate(4, 4, 1, 4);
        }

        public override void SendNewPassword(Person user, string password, Guid channelSystemId)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogError("User has no email address. Can't send new password to user ID {PersonSystemId}", user.SystemId);
                throw new Exception("Missing email address");
            }

            _mailService.SendEmail(new ForgotPasswordEmail(user, password, channelSystemId), true);
        }

        public override bool ChangePassword(Person user, bool mustChangePasswordAtNextLogon, bool sendNewPasswordEmailToUser, Guid channelSystemId)
        {
            var newPassword = GeneratePassword();

            if (sendNewPasswordEmailToUser)
            {
                try
                {
                    SendNewPassword(user, newPassword, channelSystemId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error changing password. Password for user ID {PersonSystemId} was not changed.", user.SystemId);
                    return false;
                }
            }

            try
            {
                user = user.MakeWritableClone();
                if (mustChangePasswordAtNextLogon)
                {
                    user.LoginCredential.PasswordExpirationDate = DateTimeOffset.UtcNow;
                }
                user.LoginCredential.NewPassword = newPassword;
                using (_securityContextService.ActAsSystem())
                {
                    _personService.Update(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting new password for user ID {SystemId}", user.SystemId);
                return false;
            }
            return true;
        }

        public override bool Login(string loginName, string password)
        {
            return Login(loginName, password, null);
        }

        public override bool Login(string loginName, string password, string newPassword)
        {
            var result = _authenticationService.PasswordSignIn(loginName, password, newPassword);
            switch (result)
            {
                case AuthenticationResult.Success:
                    _checkoutState.ClearState();
                    return true;
                case AuthenticationResult.RequiresChangedPassword:
                    throw new ChangePasswordException("You need to change password.");
                default:
                    return false;
            }
        }

        public override void Logout()
        {
            _checkoutState.ClearState();
            _personStorage.CurrentSelectedOrganization = null;
            _authenticationService.SignOut();
        }

        public override Person GetUser(string userInfo)
        {
            var personId = _securityContextService.GetPersonSystemId(userInfo);
            return personId != null ? _personService.Get(personId.Value) : null;
        }

        public override bool IsBusinessCustomer(Person person, out List<Organization> organizations)
        {
            _personStorage.CurrentSelectedOrganization = null;
            organizations = new List<Organization>();
            if (person.OrganizationLinks.Count == 0) return false;

            var organizationIds = new List<Guid>();
            foreach (var personToOrganizationLink in person.OrganizationLinks)
            {
                foreach (var roleSystemId in personToOrganizationLink.RoleSystemIds)
                {
                    var role = _roleService.Get(roleSystemId);
                    if (role.Id != RolesConstants.RoleOrderApprover &&
                        role.Id != RolesConstants.RoleOrderPlacer)
                    {
                        continue;
                    }
                    if (organizationIds.Contains(personToOrganizationLink.OrganizationSystemId))
                    {
                        continue;
                    }
                    organizationIds.Add(personToOrganizationLink.OrganizationSystemId);
                    break;
                }
            }

            if (organizationIds.Count <= 0)
            {
                return false;
            }
            
            organizations.AddRange(organizationIds.Select(x => _organizationService.Get(x)));
            _personStorage.CurrentSelectedOrganization = organizations[0];
            return true;
        }

        public override bool IsValidLoginForm(ModelStateDictionary modelState, LoginFormViewModel loginForm)
        {
            var prefix = nameof(loginForm);
            var userNameField = $"{prefix}.{nameof(loginForm.UserName)}";
            var passwordField = $"{prefix}.{nameof(loginForm.Password)}";

            var validationRules = new List<ValidationRuleItem<LoginFormViewModel>>()
            {
                new ValidationRuleItem<LoginFormViewModel>{Field = userNameField, Rule = model => !string.IsNullOrEmpty(model.UserName), ErrorMessage = () => "validation.required".AsWebsiteText()},
                new ValidationRuleItem<LoginFormViewModel>{Field = passwordField, Rule = model => !string.IsNullOrEmpty(model.Password), ErrorMessage = () => "validation.required".AsWebsiteText()}
            };

            return loginForm.IsValid(validationRules, modelState);
        }

        public override bool IsValidForgotPasswordForm(ModelStateDictionary modelState, ForgotPasswordFormViewModel forgotPasswordForm)
        {
            var prefix = nameof(forgotPasswordForm);
            var emailField = $"{prefix}.{nameof(forgotPasswordForm.Email)}";

            var validationRules = new List<ValidationRuleItem<ForgotPasswordFormViewModel>>()
            {
                new ValidationRuleItem<ForgotPasswordFormViewModel>{Field = emailField, Rule = model => !string.IsNullOrEmpty(model.Email), ErrorMessage = () => "validation.required".AsWebsiteText()},
                new ValidationRuleItem<ForgotPasswordFormViewModel>{Field = emailField, Rule = model => _userValidationService.IsValidEmail(model.Email), ErrorMessage = () => "validation.email".AsWebsiteText()}
            };

            return forgotPasswordForm.IsValid(validationRules, modelState);
        }
    }
}                       
