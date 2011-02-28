﻿using System;
using System.Collections.Generic;
using System.Threading;
using Apache.NMS;
using OGDotNet.Mappedtypes.Core.Position;
using OGDotNet.Mappedtypes.engine.value;
using OGDotNet.Mappedtypes.engine.view;
using OGDotNet.Mappedtypes.LiveData;

namespace OGDotNet.Model.Resources
{
    public class RemoteView
    {
        private readonly OpenGammaFudgeContext _fudgeContext;
        private readonly RestTarget _rest;
        private readonly string _activeMqSpec;
        private readonly string _name;
        private readonly MQTemplate _mqTemplate;

        public RemoteView(OpenGammaFudgeContext fudgeContext, RestTarget rest, string activeMqSpec, string name)
        {
            _fudgeContext = fudgeContext;
            _rest = rest;
            _activeMqSpec = activeMqSpec;
            _name = name;
            _mqTemplate = new MQTemplate(_activeMqSpec);
        }

        public string Name
        {
            get { return _name; }
        }

        public void Init(CancellationToken cancellationToken = new CancellationToken())
        {
            _mqTemplate.Do(delegate(ISession session)
                {
                    var temporaryTopic = session.CreateTemporaryTopic();
                    try
                    {
                        using (var consumer = session.CreateConsumer(temporaryTopic))
                        {
                            //This post responds via JMS
                            //See the java comments for explanation
                            _rest.Resolve("init").Post(temporaryTopic.TopicName);
                            IMessage message= null;
                            while (message == null)//TODO make this cancellable in a more sane way
                            {
                                message = consumer.Receive(TimeSpan.FromMilliseconds(1000));
                                cancellationToken.ThrowIfCancellationRequested();
                            }


                            bool value = (bool) message.Properties["init"];

                            if (!value)
                            {
                                throw new ArgumentException("View failed to initialize");
                            }
                        }
                    }
                    finally
                    {
                        temporaryTopic.Delete();//Oh how I wish this was a dipose call
                    }
                }
            );
        }

        public IPortfolio Portfolio
        {
            get
            {
                return _rest.Resolve("portfolio").Get<IPortfolio>();
            }
        }

        public ViewDefinition Definition
        {
            get
            {
                return _rest.Resolve("definition").Get <ViewDefinition>();
            }
        }

        public RemoteLiveDataInjector LiveDataOverrideInjector {
            get
            {
                return new RemoteLiveDataInjector(_rest.Resolve("liveDataOverrideInjector"));
            }
        }

        public RemoteViewClient CreateClient()
        {
            
            var clientUri = _rest.Resolve("clients").Create(UserPrincipal.DefaultUser);

            return new RemoteViewClient(_fudgeContext, clientUri, _mqTemplate);
        }

        public void AssertAccessToLiveDataRequirements(UserPrincipal user)
        {
            _rest.Resolve("hasAccessToLiveData", Tuple.Create("userIp", user.IpAddress), Tuple.Create("userName", user.UserName)).Post();
        }


        public IEnumerable<ValueRequirement> GetRequiredLiveData()
        {
            var target = _rest.Resolve("requiredLiveData");
            return new List<ValueRequirement>(target.Get<ValueRequirement[]>());
        }

        public HashSet<String> GetAllSecurityTypes()
        {
            var target = _rest.Resolve("allSecurityTypes");
            return new HashSet<String>(target.Get<String[]>());
        }

        public bool IsLiveComputationRunning()
        {
            var target = _rest.Resolve("liveComputationRunning");
            var reponse = target.GetFudge();
            return 1 == (sbyte)(reponse.GetByName("value").Value);
        }


        public override string ToString()
        {
            return string.Format("[View {0}]", _name);
        }
    }
}