using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomResources
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App(null);
            new CustomResourcesStack(app, "CustomResourcesStack", new StackProps());
            app.Synth();
        }
    }
}
