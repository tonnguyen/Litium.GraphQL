﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Litium.Common;
using Litium.Customers;
using Litium.Products;
using Litium.Runtime;
using Litium.Security;

namespace Litium.Accelerator.Definitions
{
    [Autostart]
    public class AcceleratorDefaultPermissionSetup : IAsyncAutostart
    {
        private readonly GroupService _groupService;
        private readonly DefaultAccessControlService _defaultAccessControlService;
        private readonly SettingService _settingService;
        private readonly SecurityContextService _securityContextService;

        public AcceleratorDefaultPermissionSetup(
            GroupService groupService,
            DefaultAccessControlService defaultAccessControlService,
            SettingService settingService,
            SecurityContextService securityContextService)
        {
            _groupService = groupService;
            _defaultAccessControlService = defaultAccessControlService;
            _settingService = settingService;
            _securityContextService = securityContextService;
        }

        private void SetPimDefaultPermission()
        {
            var visitorGroupSystemId = (_groupService.Get<Group>("Visitors") ?? _groupService.Get<Group>("Besökare"))?.SystemId ?? Guid.Empty;
            if (visitorGroupSystemId == Guid.Empty)
            {
                return;
            }
            SetEntityDefaultPermission<BaseProduct>(visitorGroupSystemId);
            SetEntityDefaultPermission<ProductPriceList>(visitorGroupSystemId);
            SetEntityDefaultPermission<ProductList>(visitorGroupSystemId);
        }

        private void SetEntityDefaultPermission<TEntity>(Guid visitorGroupSystemId)
            where TEntity : IDefaultAccessControlSupport
        {
            var defaultAccessControl = _defaultAccessControlService.Get<TEntity>()?.MakeWritableClone();
            if (defaultAccessControl.AccessControlList.Any(x => x.Operation == Operations.Entity.Read && x.GroupSystemId == visitorGroupSystemId))
            {
                return;
            }
            defaultAccessControl.AccessControlList.Add(new AccessControlEntry(Operations.Entity.Read, visitorGroupSystemId));
            _defaultAccessControlService.Update(defaultAccessControl);
        }

        private bool IsAlreadyExecuted()
        {
            return _settingService.Get<bool>($"AcceleratorDefaultPermissionSetup");
        }

        private void SetAlreadyExecuted()
        {
            _settingService.Set($"AcceleratorDefaultPermissionSetup", true);
        }

        public ValueTask StartAsync(CancellationToken cancellationToken)
        {
            if (!IsAlreadyExecuted())
            {
                using (_securityContextService.ActAsSystem())
                {
                    SetPimDefaultPermission();
                    SetAlreadyExecuted();
                }
            }

            return ValueTask.CompletedTask;
        }
    }
}
