using System;

namespace ERHMS.Domain
{
    public enum Phase
    {
        PreDeployment,
        Deployment,
        PostDeployment
    }

    public static class PhaseExtensions
    {
        public static CoreProject ToCoreProject(this Phase @this)
        {
            switch (@this)
            {
                case Phase.PreDeployment:
                    return CoreProject.Worker;
                case Phase.Deployment:
                case Phase.PostDeployment:
                    return CoreProject.Incident;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
