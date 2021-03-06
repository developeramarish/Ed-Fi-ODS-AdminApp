﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Models;
using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels
{
    public class EducationOrganizationViewModel
    {
        public List<School> Schools { get; set; }
        public List<LocalEducationAgency> LocalEducationAgencies { get; set; }
        public bool ShouldAllowMultipleDistricts { get; set; }
    }
}