using System;
using AspNetCore.Identity.DocumentDb.Stores;
using AspNetCore.Identity.DocumentDb.Tests.Fixtures;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Xunit;

namespace AspNetCore.Identity.DocumentDb.Tests
{
    public abstract class StoreTestsBase : IClassFixture<DocumentDbFixture>
    {
        protected DocumentDbFixture documentDbFixture;
        protected Uri collectionUri;

        protected StoreTestsBase(DocumentDbFixture documentDbFixture)
        {
            this.documentDbFixture = documentDbFixture;
        }

        protected void CreateDocument<TDocument>(TDocument document) where TDocument : DocumentBase
        {
            Document doc = this.documentDbFixture.Client.CreateDocumentAsync(collectionUri, document, new RequestOptions { PartitionKey = new PartitionKey(document.PartitionKey) }).Result;
        }

        protected DocumentDbUserStore<DocumentDbIdentityUser> CreateUserStore()
        {
            IOptions<DocumentDbOptions> documentDbOptions = Options.Create(new DocumentDbOptions()
            {
                Database = documentDbFixture.Database,
                UserStoreDocumentCollection = documentDbFixture.UserStoreDocumentCollection,
                RoleStoreDocumentCollection = documentDbFixture.RoleStoreDocumentCollection
            });

            return new DocumentDbUserStore<DocumentDbIdentityUser>(
                documentClient: documentDbFixture.Client,
                options: documentDbOptions,
                roleStore: new DocumentDbRoleStore<DocumentDbIdentityRole>(
                    documentClient: documentDbFixture.Client,
                    options: documentDbOptions)
                );
        }

        protected DocumentDbRoleStore<DocumentDbIdentityRole> CreateRoleStore()
        {
            return new DocumentDbRoleStore<DocumentDbIdentityRole>(
                documentClient: documentDbFixture.Client,
                options: Options.Create(new DocumentDbOptions()
                {
                    Database = documentDbFixture.Database,
                    UserStoreDocumentCollection = documentDbFixture.UserStoreDocumentCollection,
                    RoleStoreDocumentCollection = documentDbFixture.RoleStoreDocumentCollection
                }));
        }
    }
}
