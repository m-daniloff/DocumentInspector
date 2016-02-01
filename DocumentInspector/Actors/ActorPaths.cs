using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace DocumentInspector.Actors
{
    /// <summary>
    /// Static helper class used to define paths to fixed-name actors
    /// (helps eliminate errors when using <see cref="ActorSelection"/>)
    /// </summary>
    public static class ActorPaths
    {
        public static readonly ActorMetaData DocumentInspectorSupervisor = new ActorMetaData("DocumentInspectorSupervisor", "akka://github-system/user/DocumentInspectorSupervisor");
        public static readonly ActorMetaData FileValidator = new ActorMetaData("validator", "akka://github-system/user/validator");
    }
}
