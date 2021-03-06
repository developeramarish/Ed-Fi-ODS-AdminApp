﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.Database.Queries
{
    public class GetResourceClaimsQuery
    {
        private readonly ISecurityContext _securityContext;

        public GetResourceClaimsQuery(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public IEnumerable<ResourceClaim> Execute()
        {
            var resources = new List<ResourceClaim>();
            var parentResources = _securityContext.ResourceClaims.Where(x => x.ParentResourceClaim == null).ToList();
            var childResources = _securityContext.ResourceClaims.Where(x => x.ParentResourceClaim != null).ToList();
            foreach (var parentResource in parentResources)
            {
                var children = childResources.Where(x => x.ParentResourceClaimId == parentResource.ResourceClaimId);
                resources.Add(new ResourceClaim
                {
                    Children = children.Select(child => new ResourceClaim()
                    {
                        Id = child.ResourceClaimId,
                        Name = child.ResourceName,
                        ParentId = parentResource.ResourceClaimId
                    }).ToList(),
                    Name = parentResource.ResourceName,
                    Id = parentResource.ResourceClaimId
                });
            }
            return resources
                .Distinct()
                .OrderBy(x => x.Name)
                .ToList();
        }

        public List<SelectListItem> GetSelectListForResourceClaims()
        {
            var allResourceClaims = Execute();
            var selectList = new List<SelectListItem>{
                new SelectListItem{ Text="Please select a value", Value = "0" , Disabled = true, Selected = true},
            };
            var parentGroup = new SelectListGroup
            {
                Name = "Groups"
            };
            var childGroup = new SelectListGroup
            {
                Name = "Resources"
            };
            var parentGroupList = new List<SelectListItem>();
            var childGroupList = new List<SelectListItem>();
            foreach (var resourceClaim in allResourceClaims)
            {
                var item = new SelectListItem
                {
                    Text = resourceClaim.Name,
                    Value = resourceClaim.Id.ToString(),
                    Group = parentGroup
                };
                parentGroupList.Add(item);
                if (resourceClaim.Children.Count > 0)
                {
                    childGroupList.AddRange(resourceClaim.Children.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Group = childGroup
                    }));
                }
            }
            parentGroupList = parentGroupList.OrderBy(x => x.Text).ToList();
            parentGroupList.AddRange(childGroupList.OrderBy(x => x.Text));
            selectList.AddRange(new SelectList(parentGroupList, "Value", "Text", "Group.Name", -1));
            return selectList;
        }
    }
}
