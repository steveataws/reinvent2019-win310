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
            var bucket = new Bucket(this, "ReinventBucket", new BucketProps
            {
                BucketName = "reinvent2019cdkdemo-customresources"
            });

            var queue = new Queue(this, "Queue");
            bucket.AddEventNotification(EventType.OBJECT_CREATED, new SqsDestination(queue));
        }
    }
}
