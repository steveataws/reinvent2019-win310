using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Containers
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App(null);
            new ContainersStack(app, "ContainersStack", new StackProps());
            app.Synth();
        }
    }
}
