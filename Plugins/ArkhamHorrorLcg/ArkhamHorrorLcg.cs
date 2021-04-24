﻿using ArkhamHorrorLcg.ArkhamDb;
using EideticMemoryOverlay.PluginApi;
using EideticMemoryOverlay.PluginApi.Buttons;
using Emo.Common.Enums;
using StructureMap;
using System;
using System.Reflection;

namespace ArkhamHorrorLcg {
    public class ArkhamHorrorLcg : PlugIn {
        public static string PlugInName = Assembly.GetExecutingAssembly().GetName().Name;

        public ArkhamHorrorLcg() : base ("Arkham Horror: The Card Game") {
        }

        public override void SetUp(IContainer container) {
            container.Configure(x => {
                x.For<IArkhamConfiguration>().Use<ArkhamConfiguration>().Singleton();
                x.For<IArkhamDbService>().Use<ArkhamDbService>();
                x.For<IPackLoader>().Use<PackLoader>();
            });

            var packLoader = container.GetInstance<IPackLoader>();
            packLoader.FindMissingEncounterSets();
        }

        public override CardInfoButton CreateCardInfoButton(CardInfo cardInfo, CardGroupId cardGroupId) {
            return new ArkhamCardInfoButton (cardInfo as ArkhamCardInfo, cardGroupId);
        }

        public override Type LocalCardType { get { return typeof(ArkhamLocalCard); } }
    }
}
