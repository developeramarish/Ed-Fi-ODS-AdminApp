// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;

namespace EdFi.Ods.AdminApp.Management.Instances
{
    public class RegisterOdsInstanceCommand
    {
        private readonly IOdsInstanceFirstTimeSetupService _odsInstanceFirstTimeSetupService;
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public RegisterOdsInstanceCommand(IOdsInstanceFirstTimeSetupService odsInstanceFirstTimeSetupService
            , IDatabaseConnectionProvider connectionProvider)
        {
            _odsInstanceFirstTimeSetupService = odsInstanceFirstTimeSetupService;
            _connectionProvider = connectionProvider;
        }

        public async Task<int> Execute(IRegisterOdsInstanceModel instance, ApiMode mode, string userId, CloudOdsClaimSet cloudOdsClaimSet = null)
        {
            var instanceName = InferInstanceDatabaseName(instance.NumericSuffix.Value, mode);

            var newInstance = new OdsInstanceRegistration
            {
                Name = instanceName,
                Description = instance.Description
            };
            await _odsInstanceFirstTimeSetupService.CompleteSetup(newInstance, cloudOdsClaimSet, mode);
            using (var identityDbContext = AdminAppIdentityDbContext.Create())
            {
                identityDbContext.UserOdsInstanceRegistrations.Add(new UserOdsInstanceRegistration
                {
                    OdsInstanceRegistrationId = newInstance.Id,
                    UserId = userId
                });

                identityDbContext.SaveChanges();
            }
            return newInstance.Id;
        }

        private string InferInstanceDatabaseName(int odsInstanceNumericSuffix, ApiMode mode)
        {
            using (var connection = _connectionProvider.CreateNewConnection(odsInstanceNumericSuffix, mode))
                return connection.Database;
        }
    }

    public interface IRegisterOdsInstanceModel
    {
        int? NumericSuffix { get; }
        string Description { get; }
    }
}
