using System.Collections.Generic;
using System.Linq;

using TodayOrdeal;

namespace AutoInority.Extentions
{
    public static class SefiraExtensions
    {
        private static readonly Dictionary<SefiraEnum, List<SefiraEnum>> _neighborDict = new Dictionary<SefiraEnum, List<SefiraEnum>>()
        {
            { SefiraEnum.MALKUT, new List<SefiraEnum>(){SefiraEnum.YESOD, SefiraEnum.NETZACH, SefiraEnum.HOD} },
            { SefiraEnum.YESOD, new List<SefiraEnum>(){SefiraEnum.MALKUT, SefiraEnum.NETZACH, SefiraEnum.HOD} },
            { SefiraEnum.NETZACH, new List<SefiraEnum>(){SefiraEnum.YESOD, SefiraEnum.MALKUT} },
            { SefiraEnum.HOD, new List<SefiraEnum>(){SefiraEnum.YESOD, SefiraEnum.MALKUT} },
            { SefiraEnum.TIPERERTH1, new List<SefiraEnum>(){SefiraEnum.GEBURAH, SefiraEnum.CHESED} },
            { SefiraEnum.TIPERERTH2, new List<SefiraEnum>(){SefiraEnum.GEBURAH, SefiraEnum.CHESED} },
            { SefiraEnum.GEBURAH, new List<SefiraEnum>(){SefiraEnum.TIPERERTH1, SefiraEnum.TIPERERTH2, SefiraEnum.BINAH} },
            { SefiraEnum.CHESED, new List<SefiraEnum>(){SefiraEnum.TIPERERTH1, SefiraEnum.TIPERERTH2, SefiraEnum.CHOKHMAH} },
            { SefiraEnum.CHOKHMAH, new List<SefiraEnum>(){SefiraEnum.CHESED} },
            { SefiraEnum.BINAH, new List<SefiraEnum>(){SefiraEnum.GEBURAH} },
        };

        public static IEnumerable<SefiraEnum> GetNeighborEnums(SefiraEnum sefiraEnum)
        {
            if (_neighborDict.TryGetValue(sefiraEnum, out var sefiraEnums))
            {
                return sefiraEnums;
            }
            return new SefiraEnum[] { };
        }

        public static int GetPriority(this Sefira sefira)
        {
            switch ((SefiraEnum)sefira.index)
            {
                case SefiraEnum.TIPERERTH1:
                case SefiraEnum.TIPERERTH2:
                    return 10;
                case SefiraEnum.CHOKHMAH:
                case SefiraEnum.BINAH:
                    return 20;
                case SefiraEnum.CHESED:
                case SefiraEnum.GEBURAH:
                    return 30;
                case SefiraEnum.NETZACH:
                case SefiraEnum.HOD:
                    return 40;
                case SefiraEnum.MALKUT:
                case SefiraEnum.YESOD:
                default:
                    return 50;
            }
        }

        public static void MoveToNeighborPassage(this Sefira sefira)
        {
            var passage = sefira.passageList.Where(x => x.isActivate && x.type == PassageType.HORIZONTAL);
            if (passage.Any())
            {
                sefira.agentList.ForEach(x => x.SetWaitingPassage(passage.First()));
            }
        }

        public static void MoveToNetzachElevator(this Sefira sefira)
        {
            var passages = SefiraManager.instance.GetSefira(SefiraEnum.YESOD).passageList;
            var elevator = passages.Where(x => x.isActivate && x.GetSrc() == "Map/Passage/Yesod/PassageYesodElevator" && x.passageGroup == "1").First();
            sefira.agentList.ForEach(x => x.SetWaitingPassage(elevator));
        }

        public static void MoveToSefira(this Sefira sefira, SefiraEnum sefiraId)
        {
            var target = SefiraManager.instance.GetSefira(sefiraId);
            MapNode sepiraNodeByRandom = MapGraph.instance.GetSepiraNodeByRandom(target.indexString);
            if (sepiraNodeByRandom == null)
            {
                return;
            }
            var passage = sepiraNodeByRandom.GetAttachedPassage();
            sefira.agentList.ForEach(x => x.SetWaitingPassage(passage));
        }
    }
}