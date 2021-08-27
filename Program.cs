using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureBlobApp.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureBlobApp
{
    class Program
    {
        
        static void Main(string[] args)
        {

            var accountName = "spfirstblobstorage";
            var key = "CgbMOYcgMNP0hrfM4b21qVbeDgNYndU7tLgcQztD2cvdNM/Ev5JVXRN7/HFUGNc3hgIbrCSCYVPBkT4l6r1E0A==";
            var connString = "DefaultEndpointsProtocol=https;AccountName=spfirstblobstorage;AccountKey=CgbMOYcgMNP0hrfM4b21qVbeDgNYndU7tLgcQztD2cvdNM/Ev5JVXRN7/HFUGNc3hgIbrCSCYVPBkT4l6r1E0A==;EndpointSuffix=core.windows.net";

            //try
            //{
            //    //create azure blob container

            //    //method 1
            //    //CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, key), true); //use https should me true  // CloudStorageAccount.Parse(storageConnection);
            //    //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //    //CloudBlobContainer blobContainer = blobClient.GetContainerReference("testblob1111"); //blobContainer should be lowercase
            //    //blobContainer.CreateIfNotExistsAsync();

            //    //method 2
            //    //var storageAccount = CloudStorageAccount.Parse(connString);
            //    //var blobClient = storageAccount.CreateCloudBlobClient();
            //    //var blobContainer = blobClient.GetContainerReference("firstblob11");
            //    //blobContainer.CreateIfNotExistsAsync();

            //    //blobContainer.SetPermissionsAsync(new BlobContainerPermissions()
            //    //{
            //    //   PublicAccess= BlobContainerPublicAccessType.Blob
            //    //});

            //    ////CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference("azureraviblob");

            //}
            //catch (Exception ex) { }


            //create azure table
            TableManager tableManager = new TableManager("student");

            //insert entity to table
            //Student student = new Student();
            //student.PartitionKey = "student";
            //student.RowKey = Guid.NewGuid().ToString();
            //student.Name = "testuser";
            //student.Address = "testadd";
            //student.Age = 19;
            //tableManager.Insert<Student>(student);
            //Console.ReadKey();

            //update entity
            //Student st = new Student();
            //st.PartitionKey = "student"; 
            //st.RowKey =  new Guid("41828ca1-356b-4cc3-8097-3b38770a992c").ToString();
            //st.Name = "pasindufernando";
            //st.Address = "mora";
            //st.Age = 24;
            //st.IsActive = true;
            //tableManager.Update<Student>(st);

            //get  list
            // var list = tableManager.GetList<Student>();

            //get item by query
            //var id = "41828ca1-356b-4cc3-8097-3b38770a992c";
            //TableManager tableManager1 = new TableManager("student");
            //// Student studentObj = tableManager1.GetItem<Student>("RowKey eq '" + id + "'");
            //var address = "colombo";
            //Student studentObj = tableManager1.GetItem<Student>("Address eq '" +address +"'");

            TableManager tableManager1 = new TableManager("student");
            var partialKey = "student";
            var rowKey = "c0cd93b8-7151-4f47-9e06-6e3382632323";
            tableManager.Delete<Student>(partialKey, rowKey);

        }


       
    }

    public class TableManager
    {
        private static readonly string  connString = "DefaultEndpointsProtocol=https;AccountName=spfirstblobstorage;AccountKey=CgbMOYcgMNP0hrfM4b21qVbeDgNYndU7tLgcQztD2cvdNM/Ev5JVXRN7/HFUGNc3hgIbrCSCYVPBkT4l6r1E0A==;EndpointSuffix=core.windows.net";
        private CloudTable table;
        public TableManager(string _CloudTableName)
        {
             
            if (string.IsNullOrEmpty(_CloudTableName))
            {
                throw new ArgumentNullException("Table", "Table Name can't be empty");
            }
            try
            {
              
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                table = tableClient.GetTableReference(_CloudTableName);
                table.CreateIfNotExistsAsync();
            }
            catch (StorageException StorageExceptionObj)
            {
                throw StorageExceptionObj;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        public void Insert<T>(T entity) where T : TableEntity, new()
        {
            try
            {
                var tableOperation = TableOperation.Insert(entity);
                table.ExecuteAsync(tableOperation);
            }
            catch (Exception ex) { }
        }

        public void Update<T>(T entity) where T :TableEntity,new()
        {
            try
            {
                var tableOperation = TableOperation.InsertOrMerge(entity);
                table.ExecuteAsync(tableOperation);
            }
            catch(Exception ex) { }
        }

        public List<T> GetList<T>() where T : TableEntity, new()
        {
           
            List<T> entities = new List<T>();
            TableContinuationToken token = null;
            do
            {
                var dataTableQuery = new TableQuery<T>();
                var queryResult = Task.Run(() => table.ExecuteQuerySegmentedAsync(dataTableQuery, token)).GetAwaiter().GetResult();
                foreach (var item in queryResult.Results)
                {
                    // yield return item;
                    entities.Add(item);
                }
                token = queryResult.ContinuationToken;
            } while (token != null);
            return entities;

        }

        public T GetItem<T>(string query) where T : TableEntity, new()
        {
            TableQuery<T> dataTableQuery = new TableQuery<T>();
            List<T> entities = new List<T>();
            if (!String.IsNullOrEmpty(query))
            {
                dataTableQuery = new TableQuery<T>().Where(query);
            }
            TableContinuationToken token = null;
            do
            {
                var queryResult = Task.Run(() => table.ExecuteQuerySegmentedAsync(dataTableQuery, token)).GetAwaiter().GetResult();
                if (queryResult.Results.Count > 0)
                {
                    foreach (var item in queryResult.Results)
                    {
                        entities.Add(item);
                    }
                }
                else
                {
                    return null;
                }
               
                token = queryResult.ContinuationToken;
            } while (token != null);
            return entities[0];
        }


       
       

        public void Delete<T>(string partionKey, string rowKey) where T : TableEntity, new()
        {
            var entity = default(T);
            //TableOperation tableOperation = TableOperation.Retrieve<T>(partionKey, rowKey);
            //TableResult tableResult =await table.ExecuteAsync(tableOperation);
            string query = "PartitionKey eq '"+partionKey+ "' and  RowKey eq '" + rowKey + "' ";
            entity = GetItem<T>(query);
            if (entity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(entity);
                try
                {
                    table.ExecuteAsync(deleteOperation);
                }catch(Exception ex) {  }
            }
           
        }

        

    }

    
}
