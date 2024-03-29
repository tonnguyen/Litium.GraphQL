﻿using Litium.Accelerator.ViewModels.Login;
using Litium.Customers;
using Litium.Runtime.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(LoginService), Lifetime = DependencyLifetime.Singleton)]
    public abstract class LoginService
    {
        /// <summary>
        /// Sends password to the user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="password">The password of the user</param>
        /// <param name="channelSystemId">The current channel system id</param>
        public abstract void SendNewPassword(Person user, string password, Guid channelSystemId);

        /// <summary>
        /// Changes the password of the user to a generated one.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="mustChangePasswordAtNextLogon"></param>
        /// <param name="sendNewPasswordEmailToUser"></param>
        /// <param name="channelSystemId">The current channel system id</param>
        /// <returns>True if the password changes successfully, otherwise false</returns>
        public abstract bool ChangePassword(Person user, bool mustChangePasswordAtNextLogon, bool sendNewPasswordEmailToUser, Guid channelSystemId);

        /// <summary>
        /// Attempts to log in the user with the supplied <paramref name="loginName" /> and <paramref name="password" />.
        /// The logged in token is saved in the session state.
        /// </summary>
        /// <param name="loginName">The user's login name</param>
        /// <param name="password">The user's password</param>
        /// <returns>True if the log in attempt was successful, otherwise false</returns>
        /// <exception cref="ChangePasswordException">Thrown if the user must change their password.</exception>
        public abstract bool Login(string loginName, string password);

        /// <summary>
        /// Attempts to log in the user with the supplied <paramref name="loginName" /> and <paramref name="password" />.
        /// The logged in token is saved in the session state.
        /// </summary>
        /// <param name="loginName">The user's login name</param>
        /// <param name="password">The user's password</param>
        /// <param name="newPassword">The new password</param>
        /// <returns>True if the log in attempt was successful, otherwise false</returns>
        /// <exception cref="ChangePasswordException">Thrown if the user must change their password.</exception>
        public abstract bool Login(string loginName, string password, string newPassword);

        /// <summary>
        /// Attempts to log out the user
        /// </summary>
        public abstract void Logout();

        /// <summary>
        /// Gets the user information.
        /// </summary>
        /// <param name="userInfo">The user info as user name</param>
        /// <returns>The person information</returns>
        public abstract Person GetUser(string userInfo);

        /// <summary>
        /// Check if the user is a business customer or not.
        /// </summary>
        /// <param name="person">The person information</param>
        /// <param name="organizations">List of organizations that the user belongs to</param>
        /// <returns>True if the user belongs to any organization, otherwise false</returns>
        public abstract bool IsBusinessCustomer(Person person, out List<Organization> organizations);

        /// <summary>
        /// Validate the login form
        /// </summary>
        /// <param name="modelState">The model state</param>
        /// <param name="loginForm">The login form view model</param>
        /// <returns>True if the form is valid, otherwise false</returns>
        public abstract bool IsValidLoginForm(ModelStateDictionary modelState, LoginFormViewModel loginForm);

        /// <summary>
        /// Validate the forgot password form
        /// </summary>
        /// <param name="modelState">The model state</param>
        /// <param name="forgotPasswordForm">The forgot password form view model</param>
        /// <returns>True if the form is valid, otherwise false</returns>
        public abstract bool IsValidForgotPasswordForm(ModelStateDictionary modelState, ForgotPasswordFormViewModel forgotPasswordForm);

        /// <summary>
        ///     Generates the password.
        /// </summary>
        /// <returns></returns>
        public abstract string GeneratePassword();
    }
}
