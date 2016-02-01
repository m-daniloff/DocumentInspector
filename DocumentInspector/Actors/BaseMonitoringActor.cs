using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Monitoring;

namespace DocumentInspector.Actors
{
    public class BaseMonitoringActor : ReceiveActor
    {
        protected override void PreStart()
        {
            Context.IncrementActorCreated();
            base.PreStart();
        }

        protected override void PostStop()
        {
            Context.IncrementActorStopped();
            base.PostStop();
        }

        public void IncrementMessagesReceived()
        {
            Context.IncrementMessagesReceived();
        }
    }
}
