﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Models;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using FluentValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Helpers;
using FluentValidation.Validators;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Application
{
    public class AddApplicationModel : IAddApplicationModel
    {
        public string ApplicationName { get; set; }
        public int VendorId { get; set; }
        public string ClaimSetName { get; set; }

        [Display(Name = "API Profile Id")]
        public int? ProfileId { get; set; }

        public IEnumerable<int> EducationOrganizationIds { get; set; }
        public CloudOdsEnvironment Environment { get; set; }
    }

    public class AddApplicationViewModel : AddApplicationModel
    {
        public string VendorName { get; set; }
        public List<string> ClaimSetNames { get; set; }
        public List<ProfileModel> Profiles { get; set; } 
        public List<LocalEducationAgency> LocalEducationAgencies { get; set; }
        public List<School> Schools { get; set; }
        public ApplicationEducationOrganizationType EducationOrganizationType { get; set; }
    }

    public class ApplicationEducationOrganizationType : Enumeration<ApplicationEducationOrganizationType>
    {
        public static ApplicationEducationOrganizationType LocalEducationAgency = new ApplicationEducationOrganizationType(1, "Local Education Agency");
        public static ApplicationEducationOrganizationType School = new ApplicationEducationOrganizationType(2, "School");

        public ApplicationEducationOrganizationType(int value, string displayName) : base(value, displayName)
        {
        }
    }

    public class AddApplicationModelValidator : AbstractValidator<AddApplicationModel>
    {
        public AddApplicationModelValidator()
        {
            RuleFor(m => m.ApplicationName)
                .NotEmpty();

            RuleFor(m => m.ApplicationName)
                .Must(BeWithinApplicationNameMaxLength)
                .When(x => x.ApplicationName != null);

            RuleFor(m => m.ApplicationName)
                .Must(name => !ApplicationExtensions.IsSystemReservedApplicationName(name))
                .WithMessage(p => $"'{p.ApplicationName}' is a reserved name and may not be used. Please choose another name.");

            RuleFor(m => m.ClaimSetName)
                .NotEmpty()
                .WithMessage("You must choose a Claim Set");

            RuleFor(m => m.EducationOrganizationIds)
                .NotEmpty()
                .WithMessage("You must choose at least one Education Organization");

            RuleFor(m => m.Environment).NotNull();
            RuleFor(m => m.VendorId).NotEmpty();
        }

        private bool BeWithinApplicationNameMaxLength(AddApplicationModel model, string applicationName, PropertyValidatorContext context)
        {
            var persistedName = CloudOdsApplicationName.GetPersistedName(applicationName, CloudOdsEnvironment.Production);
            var extraCharactersInName = persistedName.Length - ApplicationExtensions.MaximumApplicationNameLength;
            if (extraCharactersInName <= 0)
            {
                return true;
            }

            context.Rule.MessageBuilder = c
                => $"The Application Name {applicationName} would be too long for Admin App to set up necessary Application records." +
                   $" Consider shortening the name by {extraCharactersInName} characters.";

            return false;
        }
    }
}