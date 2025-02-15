﻿using EndFieldPS.Network;
using EndFieldPS.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static EndFieldPS.Resource.ResourceManager;

namespace EndFieldPS.Packets.Sc
{
    public class PacketScSyncAllRoleScene : Packet
    {

        public PacketScSyncAllRoleScene(Player client) {

            ScSyncAllRoleScene role = new ScSyncAllRoleScene()
            {
                SceneGradeInfo =
                {

                },
                UnlockAreaInfo =
                {

                },
                SubmitEtherCount = 0,
                SubmitEtherLevel = 1,

            };
            foreach (var scene in levelDatas)
            {
                role.SceneGradeInfo.Add(new SceneGradeInfo()
                {
                    Grade=1,
                    LastDownTs=DateTime.UtcNow.Ticks,
                    SceneNumId=scene.idNum,
                });
            }
            foreach (var area in sceneAreaTable)
            {
                AreaUnlockInfo u = new()
                {
                    SceneId = area.Value.sceneId,
                    UnlockAreaId =
                    {
                        area.Value.areaId,
                    }
                };
                role.UnlockAreaInfo.Add(u);
            }
            SetData(ScMessageId.ScSyncAllRoleScene, role);
        }

    }
}
