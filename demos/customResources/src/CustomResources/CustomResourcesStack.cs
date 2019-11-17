using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.S3.Notifications;

namespace CustomResources
{
    public class CustomResourcesStack : Stack
    {
        public CustomResourcesStack(Construct parent, string id, IStackProps props) : base(parent, id, props)
        {
            // The code that defines your stack goes here
            BucketProps bucketProps = new BucketProps();
            bucketProps.AccessControl = BucketAccessControl.PUBLIC_READ_WRITE;
            bucketProps.BucketName = "reinvent2019cdkdemo";
            Bucket bucket = new Bucket(this, "ReinventBucket", bucketProps);

            Queue queue = new Queue(this, "Queue");
            bucket.AddEventNotification(EventType.OBJECT_CREATED, new SqsDestination(queue)); 
        }
    }
}
