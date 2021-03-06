﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetProfilesQueryTests : AdminDataTestBase
    {
        [Test]
        public void Should_retreive_profiles()
        {
            var profile1 = CreateProfile();
            var profile2 = CreateProfile();

            Save(profile1, profile2);

            var query = new GetProfilesQuery(TestContext);
            var results = query.Execute();

            results.Any(p => p.ProfileName == profile1.ProfileName).ShouldBeTrue();
            results.Any(p => p.ProfileName == profile2.ProfileName).ShouldBeTrue();
        }

        private static int _profileId = 0;
        private Profile CreateProfile()
        {
            return new Profile
            {
                ProfileName = $"Test Profile {_profileId++}-{DateTime.Now:O}"
            };
        }
    }
}
