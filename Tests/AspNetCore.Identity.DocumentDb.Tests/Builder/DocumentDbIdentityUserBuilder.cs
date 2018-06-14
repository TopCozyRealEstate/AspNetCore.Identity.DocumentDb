﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.DocumentDb.Tests.Builder
{
    public class DocumentDbIdentityUserBuilder
    {
        protected DocumentDbIdentityUser identityUser;

        public DocumentDbIdentityUserBuilder(DocumentDbIdentityUser identityUser)
        {
            this.identityUser = identityUser;
        }

        public static implicit operator DocumentDbIdentityUser(DocumentDbIdentityUserBuilder builder)
        {
            return builder.identityUser;
        }

        public static DocumentDbIdentityUserBuilder Create(string userName = null)
        {
            string email = userName;

            if (userName == null)
            {
                userName = Guid.NewGuid().ToString().ToUpper();
                email = userName + "@test.at";
            }

            return new DocumentDbIdentityUserBuilder(new DocumentDbIdentityUser()
            {
                UserName = userName,
                Email = userName
            });
        }

        public DocumentDbIdentityUserBuilder WithId(string id = null)
        {
            identityUser.Id = id ?? Guid.NewGuid().ToString().ToUpper();
            identityUser.PartitionKey = identityUser.Id;
            return this;
        }

        public DocumentDbIdentityUserBuilder WithNormalizedUserName()
        {
            LookupNormalizer normalizer = new LookupNormalizer();

            identityUser.NormalizedUserName = normalizer.Normalize(identityUser.UserName);
            return this;
        }

        public DocumentDbIdentityUserBuilder IsLockedOut(DateTime? until = null)
        {
            identityUser.LockoutEnabled = true;
            identityUser.LockoutEndDate = until.HasValue ? until.Value : DateTime.Now.AddMinutes(10);
            return this;
        }

        public DocumentDbIdentityUserBuilder IsNotLockedOut()
        {
            identityUser.LockoutEnabled = false;
            identityUser.LockoutEndDate = null;
            return this;
        }

        public DocumentDbIdentityUserBuilder AddClaim(string type = null, string value = null)
        {
            Claim claim = new Claim(type ?? Guid.NewGuid().ToString(), value ?? Guid.NewGuid().ToString());
            identityUser.Claims.Add(claim);

            return this;
        }

        public DocumentDbIdentityUserBuilder AddClaim(Claim claim)
        {
            identityUser.Claims.Add(claim);
            return this;
        }

        public DocumentDbIdentityUserBuilder AddRole(DocumentDbIdentityRole role = null)
        {
            if (role == null)
            {
                LookupNormalizer normalizer = new LookupNormalizer();
                string newRoleName = Guid.NewGuid().ToString().ToUpper();

                role = new DocumentDbIdentityRole()
                {
                    Id = Guid.NewGuid().ToString().ToUpper(),
                    Name = newRoleName,
                    NormalizedName = normalizer.Normalize(newRoleName)
                };
                role.PartitionKey = role.Id;
            }
            else
            {
                identityUser.Roles.Add(role);
            }

            return this;
        }

        public DocumentDbIdentityUserBuilder WithNormalizedEmail()
        {
            LookupNormalizer normalizer = new LookupNormalizer();

            identityUser.NormalizedEmail = normalizer.Normalize(identityUser.Email);
            return this;
        }

        public DocumentDbIdentityUserBuilder WithUserLoginInfo(int amount = 1)
        {
            List<UserLoginInfo> logins = new List<UserLoginInfo>();

            for (int i = 0; i < amount; i++)
            {
                logins.Add(new UserLoginInfo(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()));
            }

            this.identityUser.Logins = logins;
            return this;
        }

        public DocumentDbIdentityUserBuilder WithAccessFailedCountOf(int count)
        {
            this.identityUser.AccessFailedCount = count;
            return this;
        }

        public DocumentDbIdentityUser Build()
        {
            return this.identityUser;
        }
    }
}
