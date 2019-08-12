using Microsoft.AspNetCore.DataProtection;
using PipelineSpace.Application.Interfaces;
using System;
using System.Text;

namespace PipelineSpace.Infra.CrossCutting.Security
{
    public class DataProtectorService : IDataProtectorService
    {
        private IDataProtector _dataProtector;
        public DataProtectorService(IDataProtectionProvider provider)
        {
            _dataProtector = provider.CreateProtector("PipelineSpace.Infra.CrossCutting.Security");
        }

        public string Protect(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            byte[] input = Encoding.UTF8.GetBytes(plainText);
            
            var protectedData = _dataProtector.Protect(input);

            var str = Convert.ToBase64String(protectedData);

            return str;
        }

        public string Unprotect(string protectedData)
        {
            if (string.IsNullOrEmpty(protectedData))
                return protectedData;

            var bytes = Convert.FromBase64String(protectedData);

            try
            {
                var unprotectedData = _dataProtector.Unprotect(bytes);

                return Encoding.UTF8.GetString(unprotectedData);
            }
            catch (Exception exUnprotect)
            {
                try
                {
                    IPersistedDataProtector persistedProtector = _dataProtector as IPersistedDataProtector;
                    if (persistedProtector == null)
                    {
                        throw new Exception("Can't call DangerousUnprotect.");
                    }

                    bool requiresMigration, wasRevoked;
                    var unprotectedPayload = persistedProtector.DangerousUnprotect(
                        protectedData: bytes,
                        ignoreRevocationErrors: true,
                        requiresMigration: out requiresMigration,
                        wasRevoked: out wasRevoked);

                    Console.WriteLine($"Unprotected payload: {Encoding.UTF8.GetString(unprotectedPayload)}");
                    Console.WriteLine($"Requires migration = {requiresMigration}, was revoked = {wasRevoked}");

                    return Encoding.UTF8.GetString(unprotectedPayload);
                }
                catch (Exception exUnprotectRevoked)
                {
                    
                }
            }

            return string.Empty;
        }
    }
}
