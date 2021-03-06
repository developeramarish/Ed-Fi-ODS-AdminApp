﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Commands;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using VendorUser = EdFi.Admin.DataAccess.Models.User;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Commands
{
    [TestFixture]
    public class DeleteVendorCommandTests : AdminDataTestBase
    {
        [Test]
        public void ShouldDeleteVendor()
        {
            var newVendor = new Vendor();
            Save(newVendor);
            var vendorId = newVendor.VendorId;

            var deleteVendorCommand = new DeleteVendorCommand(TestContext, null);
            deleteVendorCommand.Execute(vendorId);
            TestContext.Vendors.Where(v => v.VendorId == vendorId).ShouldBeEmpty();
        }

        [Test]
        public void ShouldDeleteVendorWithApplication()
        {
            var newVendor = new Vendor {VendorName = "test vendor"};
            var newApplication = new Application {ApplicationName = "test application", OperationalContextUri = OperationalContext.DefaultOperationalContextUri };
            newVendor.Applications.Add(newApplication);
            Save(newVendor);
            var vendorId = newVendor.VendorId;
            var applicationId = newApplication.ApplicationId;
            applicationId.ShouldBeGreaterThan(0);

            var deleteApplicationCommand = new DeleteApplicationCommand(TestContext);
            var deleteVendorCommand = new DeleteVendorCommand(TestContext, deleteApplicationCommand);

            deleteVendorCommand.Execute(vendorId);
            TestContext.Vendors.Where(v => v.VendorId == vendorId).ShouldBeEmpty();
            TestContext.Applications.Where(a => a.ApplicationId == applicationId).ShouldBeEmpty();
        }

        [Test]
        public void ShouldDeleteVendorWithUser()
        {
            var newVendor = new Vendor { VendorName = "test vendor" };
            var newUser = new VendorUser { FullName = "test user" };
            newVendor.Users.Add(newUser);
            Save(newVendor);
            var vendorId = newVendor.VendorId;
            var userId = newUser.UserId;
            userId.ShouldBeGreaterThan(0);

            var deleteVendorCommand = new DeleteVendorCommand(TestContext, null);
            deleteVendorCommand.Execute(vendorId);
            TestContext.Vendors.Where(v => v.VendorId == vendorId).ShouldBeEmpty();
            TestContext.Users.Where(u => u.UserId == userId).ShouldBeEmpty();
        }
    }
}
