using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;
using Amazon.S3;
using Amazon.S3.Model;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using PipelineSpace.Worker.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class CPSAWSService : ICPSService
    {
        public async Task DeleteService(string name, CPSAuthModel auth)
        {
            try
            {
                AmazonCloudFormationClient cloudFormationClient = new AmazonCloudFormationClient(auth.AccessId, auth.AccessSecret, Amazon.RegionEndpoint.GetBySystemName(auth.AccessRegion));
                var responseCloudFormationClient = await cloudFormationClient.DeleteStackAsync(new DeleteStackRequest() { StackName = name.ToLower() });

                string bucketName = name.ToLower();

                AmazonS3Client s3Client = new AmazonS3Client(auth.AccessId, auth.AccessSecret, Amazon.RegionEndpoint.GetBySystemName(auth.AccessRegion));
                ListObjectsResponse response = await s3Client.ListObjectsAsync(new ListObjectsRequest() { BucketName = bucketName });
                
                if (response.S3Objects.Count > 0)
                {
                    List<KeyVersion> keys = response.S3Objects.Select(obj => new KeyVersion() { Key = obj.Key }).ToList();
                    DeleteObjectsRequest deleteObjectsRequest = new DeleteObjectsRequest
                    {
                        BucketName = bucketName, 
                        Objects = keys
                    };
                    await s3Client.DeleteObjectsAsync(deleteObjectsRequest);
                }

                //Delete Bucket
                DeleteBucketRequest request = new DeleteBucketRequest
                {
                    BucketName = bucketName
                };

                await s3Client.DeleteBucketAsync(request);
            }
            catch (Exception ex)
            {
                TelemetryClientManager.Instance.TrackException(ex);

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
