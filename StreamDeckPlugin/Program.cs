﻿using Emo.Common.Services;
using Emo.Common.Tcp;
using Emo.Common.Utils;
using StreamDeckPlugin.Services;
using StreamDeckPlugin.Utils;

namespace StreamDeckPlugin {
    class Program {
        public static void Main(string[] args) {
#if DEBUG
            // optional, but recommended
            System.Diagnostics.Debugger.Launch();
#endif
            var container = new StructureMap.Container(x => {
                x.Scan(y => {
                    y.TheCallingAssembly();
                    y.WithDefaultConventions();
                });
            });

            var eventBus = new EventBus();

            container.Configure(x => {
                x.For<IEventBus>().Use(eventBus);
                x.For<ICrossAppEventBus>().Use(eventBus);
                x.For<ICardGroupStore>().Use<CardGroupStore>().Singleton();
                x.For<IDynamicActionManager>().Use<DynamicActionManager>().Singleton();
                x.For<IDynamicActionInfoStore>().Use<DynamicActionInfoStore>().Singleton();
                x.For<ISendEventHandler>().Use<SendEventHandler>().Singleton();
                x.For<IImageService>().Use<ImageService>().Singleton();
                x.For<ITcpRequestHandler>().Use<TcpRequestHandler>().Singleton();
                x.For<IRequestHandler>().Use(c => c.GetInstance<ITcpRequestHandler>());
                x.For<IReceiveSocketService>().Use<ReceiveSocketService>();
                x.For<IEstablishConnectionToUiService>().Use<EstablishConnectionToUiService>();
            });

            ServiceLocator.Container = container;

            //keep references or garbage collection will clean this up and we'll stop receiving events
            var sendEventHandler = container.GetInstance<ISendEventHandler>();
            var receiveSocketService = container.GetInstance<IReceiveSocketService>();
            receiveSocketService.StartListening(StreamDeckTcpInfo.Port);

            var establishConnectionToUiService = container.GetInstance<IEstablishConnectionToUiService>();
            establishConnectionToUiService.AttemptToEstablishConnection();

            // register actions and connect to the Stream Deck
            SharpDeck.StreamDeckPlugin.Run();
        }
    }
}
