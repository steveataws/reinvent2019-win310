using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using System.IO;
using System;

namespace Containers
{
    public class ContainersStack : Stack
    {
        public ContainersStack(Construct parent, string id, IStackProps props) : base(parent, id, props)
        {
            // The code that defines your stack goes here
            Vpc vpc = new Vpc(this, "VPC");
            ClusterProps clusterProps = new ClusterProps();
            clusterProps.Vpc = vpc;
            Cluster cluster = new Cluster(this, "Cluster", clusterProps);
            FargateTaskDefinition taskDef = new FargateTaskDefinition(this, "FargateTaskDefinition");
         
            ContainerDefinitionOptions containerOptions = new ContainerDefinitionOptions();
            var rootDirectory = Directory.GetCurrentDirectory();
       
            var path = Path.GetFullPath(Path.Combine(rootDirectory, @"dotnetapp/"));
            //var image = ContainerImage.FromAsset(path);
            var image = ContainerImage.FromAsset("dotnetapp");
            containerOptions.Image = image;
            var portMapping = new PortMapping() {
                ContainerPort = 80,
                HostPort = 80
            };
            taskDef.AddContainer("Container", containerOptions).AddPortMappings(portMapping);
            var serviceProps = new ApplicationLoadBalancedFargateServiceProps() {
                MemoryLimitMiB = 512,
                Cpu = 256,
                TaskDefinition = taskDef
            };
            ApplicationLoadBalancedFargateService service = new ApplicationLoadBalancedFargateService(this, "DotnetFargateApp", serviceProps);
            
        }
    }
}
