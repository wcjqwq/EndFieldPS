﻿using EndFieldPS.Network;
using EndFieldPS.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EndFieldPS.Packets.Sc
{
    public class PacketScEnterSceneNotify : Packet
    {

        public PacketScEnterSceneNotify(Player client, int sceneNumId = 21) {

            ScEnterSceneNotify proto = new ScEnterSceneNotify()
            {
                Position = client.position.ToProto(),
                
                SceneId = 0,
                RoleId = client.roleId,
                SceneNumId = sceneNumId,

            };

            SetData(ScMessageId.ScEnterSceneNotify, proto);
        }

    }
}
