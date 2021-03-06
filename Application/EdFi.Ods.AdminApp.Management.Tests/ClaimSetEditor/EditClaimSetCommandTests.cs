﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using Shouldly;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using Application = EdFi.Security.DataAccess.Models.Application;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;


namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class EditClaimSetCommandTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldEditClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var alreadyExistingClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(alreadyExistingClaimSet);

            var editModel = new EditClaimSetModel {ClaimSetName = "TestClaimSetEdited", ClaimSetId = alreadyExistingClaimSet.ClaimSetId};
            
            var command = new EditClaimSetCommand(TestContext);

            command.Execute(editModel);

            var editedClaimSet = TestContext.ClaimSets.Single(x => x.ClaimSetId == alreadyExistingClaimSet.ClaimSetId);
            editedClaimSet.ClaimSetName.ShouldBe(editModel.ClaimSetName);
        }

        [Test]
        public void ShouldNotEditClaimSetIfNameNotUnique()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var alreadyExistingClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet1", Application = testApplication };
            Save(alreadyExistingClaimSet);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet2", Application = testApplication };
            Save(testClaimSet);

            var editModel = new EditClaimSetModel { ClaimSetName = "TestClaimSet1", ClaimSetId = testClaimSet.ClaimSetId };

            var validator = new EditClaimSetModelValidator(TestContext);
            var validationResults = validator.Validate(editModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("A claim set with this name already exists in the database. Please enter a unique name.");
        }

        [Test]
        public void ShouldNotEditClaimSetIfNameEmpty()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet1", Application = testApplication };
            Save(testClaimSet);

            var editModel = new EditClaimSetModel { ClaimSetName = "", ClaimSetId = testClaimSet.ClaimSetId };

            var validator = new EditClaimSetModelValidator(TestContext);
            var validationResults = validator.Validate(editModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("'Claim Set Name' must not be empty.");
        }
    }
}