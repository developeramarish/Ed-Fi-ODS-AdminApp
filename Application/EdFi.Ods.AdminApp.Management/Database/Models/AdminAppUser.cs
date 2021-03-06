﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNet.Identity.EntityFramework;

namespace EdFi.Ods.AdminApp.Management.Database.Models
{
    public class AdminAppUser : IdentityUser
    {
        public AdminAppUser() : base()
        {
        }

        public AdminAppUser(string userName) : base(userName)
        {
        }

        public bool RequirePasswordChange { get; set; }
    }
}
