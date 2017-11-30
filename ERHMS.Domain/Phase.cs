using System;
using System.ComponentModel;

namespace ERHMS.Domain
{
    public enum Phase
    {
        [Description("Pre-deployment")]
        PreDeployment,

        [Description("Deployment")]
        Deployment,

        [Description("Post-deployment")]
        PostDeployment,

        [Obsolete("Use 'Phase.PostDeployment' instead.")]
        [Description("Closed")]
        Closed
    }
}
