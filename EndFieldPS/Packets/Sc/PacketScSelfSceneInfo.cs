﻿using EndFieldPS.Network;
using EndFieldPS.Protocol;
using EndFieldPS.Resource;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EndFieldPS.Packets.Sc
{
    public class PacketScSelfSceneInfo : Packet
    {

        public PacketScSelfSceneInfo(Player session, bool isTeamSpawn=false) {
            if (isTeamSpawn)
            {
                ScSelfSceneInfo sceneInfo = new()
                {
                    SceneId = 0,
                    SceneNumId = session.curSceneNumId,
                    SelfInfoReason = 0,
                    
                    TeamInfo = new()
                    {
                        CurLeaderId = session.teams[session.teamIndex].leader,
                        TeamIndex = session.teamIndex,
                        TeamType = CharBagTeamType.Main

                    },
                    SceneGrade = 1,
                    
                    Detail = new()
                    {
                        TeamIndex = session.teamIndex,

                        CharList =
                        {

                        },
                        
                    }
                };
                foreach (var item in ResourceManager.sceneAreaTable)
                {
                    sceneInfo.UnlockArea.Add(item.Value.areaId);
                }
                session.teams[session.teamIndex].members.ForEach(m =>
                {
                    sceneInfo.Detail.CharList.Add(session.chars.Find(c => c.guid == m).ToSceneProto());
                });
                //  session.Send(Packet.EncodePacket((int)ScMessageId.ScObjectEnterView, test));
                SetData(ScMessageId.ScSelfSceneInfo, sceneInfo);
            }
           
        }

    }
}
