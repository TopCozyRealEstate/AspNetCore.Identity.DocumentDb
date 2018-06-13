using System;
using AspNetCore.Identity.DocumentDb.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace AspNetCore.Identity.DocumentDb.Tests.Fixtures
{
    public class DocumentDbFixture : IDisposable
    {
        public DocumentClient Client { get; private set; }
        public ILookupNormalizer Normalizer { get; private set; }
        public string Database { get; } = "AspNetCore.Identity.DocumentDb.Tests";
        public string UserStoreDocumentCollection { get; private set; } = "AspNetCore.Identity";
        public string RoleStoreDocumentCollection { get; private set; } = "AspNetCore.Identity";
        public string UserStorePartitionKeyPath { get; private set; } = "/partitionKey";
        public string RoleStorePartitionKeyPath { get; private set; } = "/partitionKey";
        public Uri DatabaseLink { get; private set; }
        public Uri UserStoreCollectionLink { get; private set; }
        public Uri RoleStoreCollectionLink { get; private set; }

        public DocumentDbFixture()
        {
            Client = DocumentClientFactory.CreateClient(
                serviceEndpoint: new Uri("https://localhost:8081", UriKind.Absolute),
                authKeyOrResourceToken: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                connectionPolicy: new ConnectionPolicy() { EnableEndpointDiscovery = false });

            Normalizer = new LookupNormalizer();

            DatabaseLink = UriFactory.CreateDatabaseUri(this.Database);
            UserStoreCollectionLink = UriFactory.CreateDocumentCollectionUri(this.Database, this.UserStoreDocumentCollection);
            RoleStoreCollectionLink = UriFactory.CreateDocumentCollectionUri(this.Database, this.RoleStoreDocumentCollection);

            CreateTestDatabase();
        }

        private void CreateTestDatabase()
        {
            CleanupTestDatabase();

            Database db = this.Client.CreateDatabaseAsync(new Database() { Id = this.Database }).Result;

            DocumentCollection userCollection = new DocumentCollection() { Id = this.UserStoreDocumentCollection };
            userCollection.PartitionKey.Paths.Add(this.UserStorePartitionKeyPath);

            userCollection = this.Client.CreateDocumentCollectionAsync(DatabaseLink, userCollection).Result;

            if (this.UserStoreDocumentCollection != this.RoleStoreDocumentCollection)
            {
                DocumentCollection roleCollection = new DocumentCollection() { Id = this.RoleStoreDocumentCollection };
                roleCollection.PartitionKey.Paths.Add(this.RoleStorePartitionKeyPath);

                roleCollection = this.Client.CreateDocumentCollectionAsync(DatabaseLink, roleCollection).Result;
            }
        }

        private void CleanupTestDatabase()
        {
            try
            {
                var existingDb = this.Client.ReadDatabaseAsync(this.DatabaseLink).Result;
            }
            catch (Exception)
            {
                // Suppose DB does not exist (anymore)
                return;
            }

            var db = this.Client.DeleteDatabaseAsync(this.DatabaseLink).Result;
        }

        public void Dispose()
        {
            CleanupTestDatabase();
        }
    }
}
