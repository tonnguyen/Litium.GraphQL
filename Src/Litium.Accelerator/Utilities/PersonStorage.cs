using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Customers;
using Litium.Accelerator.Constants;
using Litium.Security;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Utilities
{
    /// <summary>
    /// Storage utility class for storing person information in session.
    /// </summary>
    [Service(ServiceType = typeof(PersonStorage), Lifetime = DependencyLifetime.Singleton)]
    public class PersonStorage
    {
        private const string _hasUserApproverRole = "HasUserApproverRole";
        private const string _hasUserPlacerRole = "HasUserPlacerRole";
        private const string _selectedOrganization = "SelectedOrganization";
        private readonly SessionStorage _sessionStorage;
        private readonly OrganizationService _organizationService;
        private readonly PersonService _personService;
        private readonly RoleService _roleService;
        private readonly SecurityContextService _securityContextService;

        public PersonStorage(
            SessionStorage sessionStorage,
            OrganizationService organizationService,
            PersonService personService,
            RoleService roleService,
            SecurityContextService securityContextService)
        {
            _sessionStorage = sessionStorage;
            _organizationService = organizationService;
            _personService = personService;
            _roleService = roleService;
            _securityContextService = securityContextService;
        }

        /// <summary>
        /// Gets or sets the <see cref="Organization" /> stored in session. This <see cref="Organization" /> contains
        /// information about selected organization for the logged in customer.
        /// </summary>
        public Organization CurrentSelectedOrganization
        {
            get
            {
                var organizationSystemId = _sessionStorage.GetValue<Guid?>(_selectedOrganization);
                return organizationSystemId != null ? _organizationService.Get(organizationSystemId.Value) : null;
            }
            set
            {
                _sessionStorage.SetValue<object>(_selectedOrganization, null);
                _sessionStorage.SetValue<object>(_hasUserApproverRole, null);
                _sessionStorage.SetValue<object>(_hasUserPlacerRole, null);

                var personSystemId = _securityContextService.GetIdentityUserSystemId();
                if (value == null || personSystemId == null)
                {
                    return;
                }

                var person = _personService.Get(personSystemId.Value);
                var organizationLink = person.OrganizationLinks.FirstOrDefault(x => x.OrganizationSystemId == value.SystemId);
                var roles = new List<Role>();
                if (organizationLink != null)
                {
                    roles = organizationLink.RoleSystemIds.Select(x => _roleService.Get(x)).ToList();
                }

                var hasplacerRole = roles.Exists(item => item.Id == RolesConstants.RoleOrderPlacer);
                var hasApproverRole = roles.Exists(item => item.Id == RolesConstants.RoleOrderApprover);

                // Update the session with the values
                _sessionStorage.SetValue(_selectedOrganization, value.SystemId);
                _sessionStorage.SetValue(_hasUserApproverRole, hasApproverRole);
                _sessionStorage.SetValue(_hasUserPlacerRole, hasplacerRole);
            }
        }

        /// <summary>
        ///     Define if user has approver role
        /// </summary>
        public bool HasApproverRole => _sessionStorage.GetValue<bool?>(_hasUserApproverRole).GetValueOrDefault();

        /// <summary>
        ///     Define if user has placer role
        /// </summary>
        public bool HasPlacerRole => _sessionStorage.GetValue<bool?>(_hasUserPlacerRole).GetValueOrDefault();
    }
}
