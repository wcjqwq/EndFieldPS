﻿using EndFieldPS.Game;
using Google.Protobuf.WellKnownTypes;
using HttpServerLite;
using MongoDB.Bson.IO;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EndFieldPS
{
    public class Dispatch
    {
        public class Account()
        {
            [Column("account")]
            public string account { get; set; }
            [Column("password")]
            public string md5password { get; set; }
            [Column("token")]
            public string token { get; set; }
        }
        public Webserver server;
        public static void Print(string text)
        {
            Console.WriteLine($"[{Server.ColoredText("Dispatch", "0307fc")}] " + text);
        }
        public void Start()
        {
            server = new Webserver(Server.config.DispatchIp, Server.config.DispatchPort, false, null, null, DefaultRoute);
            server.Settings.Headers.Host = "http://"+ Server.config.DispatchIp + ":"+ Server.config.DispatchPort;
            server.Events.ResponseSent += Events_ResponseSent;
            server.Events.RequestReceived += Events_RequestReceived;
            server.Events.RequestDenied += Events_RequestDenied;
            server.Events.ConnectionReceived += Events_ConnectionReceived;
     
            server.Start();
            Print($"Dispatch started on {Server.config.DispatchIp}:{Server.config.DispatchPort}");
        }

        private void Events_ConnectionReceived(object? sender, ConnectionEventArgs e)
        {
            Print("Requested " + e.Ip+":"+e.Port);
        }

        private void Events_RequestDenied(object? sender, RequestEventArgs e)
        {
            Print("Denied " + e.Url);
        }

        private void Events_RequestReceived(object? sender, RequestEventArgs e)
        {
            Print("Requested " + e.Url);
        }

        private void Events_ResponseSent(object? sender, ResponseEventArgs e)
        {
            Print("Sent " + e.Url + " status: " + e.StatusCode);
        }

        static async Task DefaultRoute(HttpContext ctx)
        {
            byte[] resp;
            string curVer = "EndField PS";
            resp = System.Text.Encoding.UTF8.GetBytes(curVer);
           
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentLength = resp.Length;
                ctx.Response.SendAsync(resp);
            Console.WriteLine(ctx.Request.Method.ToString());


            // await ctx.Response.SendAsync(resp);
        }
        
        [StaticRoute(HttpServerLite.HttpMethod.POST, "/u8/pay/getAllProductList")]
        public static async Task getAllProductList(HttpContext ctx)
        {
            string resp = "{\"productList\":[]}";

            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/api/game/get_latest")]
        public static async Task get_latest(HttpContext ctx)
        {
            string requestVersion = ctx.Request.Query.Elements["version"];
            string resp = "{\"action\":0,\"version\":\""+GameConstants.GAME_VERSION+"\",\"request_version\":\""+requestVersion+"\",\"pkg\":{\"packs\":[],\"total_size\":\"0\",\"file_path\":\"" +GameConstants.GAME_VERSION_ASSET_URL+ "\",\"url\":\"\",\"md5\":\"\",\"package_size\":\"0\",\"file_id\":\"0\",\"sub_channel\":\"\"},\"patch\":null}";

            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/api/remote_config/get_remote_config/1003/prod-cbt/default/default/network_config")]
        public static async Task network_config(HttpContext ctx)
        {
            string resp = "{\"asset\":\"https://beyond.hg-cdn.com/asset/\",\"hgage\":\"\",\"sdkenv\":\"2\",\"u8root\":\"https://u8.gryphline.com/u8\",\"appcode\":4,\"channel\":\"prod\",\"netlogid\":\"GFz8RRMDN45w\",\"gameclose\":false,\"netlogurl\":\"http://native-log-collect.gryphline.com:32000/\",\"accounturl\":\"https://binding-api-account-prod.gryphline.com\",\"launcherurl\":\"https://launcher.gryphline.com\"}";
           
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/api/remote_config/get_remote_config/1003/prod-cbt1oversea/default/Windows/game_config")]
        public static async Task remote_config(HttpContext ctx)
        {
            string resp = "{\"mockLogin\": false, \"selectSrv\": false, \"enableHotUpdate\": true, \"enableEntitySpawnLog\": false}";

            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/api/gameBulletin/version")]
        public static async Task Version(HttpContext ctx)
        {
            string resp = "{\"code\":0,\"msg\":\"\",\"data\":{\"version\":30}}";
            
          
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }

       

        [StaticRoute(HttpServerLite.HttpMethod.GET, "/app/v1/config")]
        public static async Task config_check(HttpContext ctx)
        {
            string resp = "{\"data\":{\"agreementUrl\":{\"register\":\"https://user.gryphline.com/{language}/protocol/plain/terms_of_service\",\"privacy\":\"https://user.gryphline.com/{language}/protocol/plain/privacy_policy\",\"unbind\":\"https://user.gryphline.com/{language}/protocol/plain/endfield/privacy_policy\",\"account\":\"https://user.gryphline.com/{language}/protocol/plain/terms_of_service\",\"game\":\"https://user.gryphline.com/{language}/protocol/plain/endfield/privacy_policy\"},\"app\":{\"googleAndroidClientId\":\"\",\"googleIosClientId\":\"\",\"enableAutoLogin\":true,\"enablePayment\":true,\"enableGuestRegister\":false,\"needShowName\":true,\"displayName\":{\"en-us\":\"Arknights: Endfield\",\"ja-jp\":\"アークナイツ：エンドフィールド\",\"ko-kr\":\"명일방주：엔드필드\",\"zh-cn\":\"明日方舟：终末地\",\"zh-tw\":\"明日方舟：終末地\"},\"unbindAgreement\":[],\"unbindLimitedDays\":30,\"unbindCoolDownDays\":14,\"customerServiceUrl\":\"https://gryphline.helpshift.com/hc/{language}/4-arknights-endfield\",\"enableUnbindGrant\":false},\"customerServiceUrl\":\"https://gryphline.helpshift.com/hc/{language}/4-arknights-endfield\",\"thirdPartyRedirectUrl\":\"https://web-api.gryphline.com/callback/thirdPartyAuth.html\",\"scanUrl\":{\"login\":\"yj://scan_login\"},\"loginChannels\":[],\"userCenterUrl\":\"https://user.gryphline.com/pcSdk/userInfo?language={language}\"},\"msg\":\"OK\",\"status\":0,\"type\":\"A\"}";
            
                       
                  
            ctx.Response.StatusCode = 200;
            //ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }

        [StaticRoute(HttpServerLite.HttpMethod.POST, "/user/auth/v1/token_by_email_password")]
        public static async Task token_login(HttpContext ctx)
        {
            string requestBody = ctx.Request.DataAsString;
            Console.WriteLine(requestBody);
            string resp = "{    \"msg\":\"OK\",    \"status\":0,    \"type\":\"A\",    \"token\":\"/pcMj2PHpRMe9wDkZZMux7fl\"}";



            ctx.Response.StatusCode = 200;
            //ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/user/info/v1/basic")]
        public static async Task account_info_get(HttpContext ctx)
        {
            string resp = "{\"data\":{\"hgId\":\"1799321925\",\"email\":\"dispatch@endfield.ps\",\"realEmail\":\"dispatch@endfield.ps\",\"isLatestUserAgreement\":true,\"nickName\":\"Endfield PS\"},\"msg\":\"OK\",\"status\":0,\"type\":1}";



            ctx.Response.StatusCode = 200;
            //ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        public class GrantClass
        {
            public string token;
        }
        [StaticRoute(HttpServerLite.HttpMethod.POST, "/user/oauth2/v2/grant")]
        public static async Task account_ugrant(HttpContext ctx)
        {
            string requestBody = ctx.Request.DataAsString;

            GrantClass grant = Newtonsoft.Json.JsonConvert.DeserializeObject<GrantClass>(requestBody);
            string resp = "{  \"data\": {  \"uid\": \"1\",  \"code\": \"5PHvncclDlHKfx/hW11YeaZGyw9QS+HAR25lkW+cQVU71RfAi3JK9mrYtUVz81NXqLbG6jwqMZfpQ7DYWxLk+PHCZ81TyWq4RHGvBf/VZdNx13JqC9YL7wfMtpXRQ2DYT1LgMv5KNXbz8JhW+aGVt91R2XYFbqlcJ7DZqHwMTVkNpU6W1R3YC4FZ+JRqCT8KvlyNHgT8\"  },  \"msg\": \"OK\",  \"status\": 0,  \"type\": \"A\"}";


           
            ctx.Response.StatusCode = 200;
         
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.POST, "/u8/user/auth/v2/grant")]
        public static async Task account_grant(HttpContext ctx)
        {
            string requestBody = ctx.Request.DataAsString;

            GrantClass grant = Newtonsoft.Json.JsonConvert.DeserializeObject<GrantClass>(requestBody);
            string resp = "{  \"data\": {  \"uid\": \"1\",  \"code\": \"5PHvncclDlHKfx/hW11YeaZGyw9QS+HAR25lkW+cQVU71RfAi3JK9mrYtUVz81NXqLbG6jwqMZfpQ7DYWxLk+PHCZ81TyWq4RHGvBf/VZdNx13JqC9YL7wfMtpXRQ2DYT1LgMv5KNXbz8JhW+aGVt91R2XYFbqlcJ7DZqHwMTVkNpU6W1R3YC4FZ+JRqCT8KvlyNHgT8\"  },  \"msg\": \"OK\",  \"status\": 0,  \"type\": \"A\"}";



            ctx.Response.StatusCode = 200;

            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/api/remote_config/get_remote_config/1003/prod-cbt/default/default/server_config_EUAndUS")]
        public static async Task server_config_EUAndUS(HttpContext ctx)
        {
            string requestBody = ctx.Request.DataAsString;
            Console.WriteLine(requestBody);
            string resp = "{\"addr\": \""+Server.config.ServerIp+"\", \"port\": "+Server.config.LocalPort+"}";



            ctx.Response.StatusCode = 200;

            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/game/user/v1/query_zone_whitelist")]
        public static async Task query_zone_whitelist(HttpContext ctx)
        {
            string requestBody = ctx.Request.DataAsString;
            Console.WriteLine(requestBody);
            string resp = "{  \"data\": {    \"zone\": [      \"EUAndUS\"    ]  },  \"msg\": \"OK\",  \"status\": 0}";



            ctx.Response.StatusCode = 200;

            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/user/oauth2/v1/check_whitelist")]
        public static async Task check_whitelist(HttpContext ctx)
        {
            string requestBody = ctx.Request.DataAsString;
            Console.WriteLine(requestBody);
            string resp = "{\"status\":0, \"msg\":\"OK\", \"type\":\"A\"}";



            ctx.Response.StatusCode = 200;

            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.POST, "/u8/user/auth/v2/token_by_channel_token")]
        public static async Task token_channel_token(HttpContext ctx)
        {
            string requestBody = ctx.Request.DataAsString;
            Console.WriteLine(requestBody);
            string resp = "{  \"data\": {    \"token\":\"2f268842ce98e35dc22285d042b7f09dfe26cb1fe6fcd9575b5c2139e27deb55118b1c43f81806a2832809e4598ea407c97020759020bd2c979a94060090ff3573c6ea915c487935eb567a14ab2675f76d7ec384d2ccce21ab31e8b725f92aff1cb86cecc884a8d066b49156117279d8a22abdf42d941e50da5a748b67c9ce5cdc63a5386ada6b4f824dda9e4fdcb706bf572af82e23d3f9565fb516f5b66fa07a4e9e451ce5ef5029e56b5a1aa54b1fffa5008ed797fb767daab43de833e889d14dc76aac67d1158afb71b6143be644ba20c32e3d6928e74cc888a09c094365d7f2db14f5c3ac2b569ac42c07f37635ec5519f3a4b5e5d80959c18d86ee7fe5b7a81c1a9de95fb94b2a67e6bc62ce6595e688b07a62\"  },  \"msg\": \"OK\",  \"status\": 0,  \"type\": \"\"}";



            ctx.Response.StatusCode = 200;

            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/api/meta")]
        public static async Task drawButton(HttpContext ctx)
        {
            string requestBody = ctx.Request.DataAsString;
            Console.WriteLine(requestBody);
            string resp = "{\"code\":0,\"msg\":\"\",\"data\":{\"characterList\":[{\"key\":\"endministrator1\",\"code\":\"01\",\"name\":\"Endministrator\",\"codename\":\"Endministrator\",\"camp\":\"Endfield Industries\",\"race\":\"[data_missing]\",\"intro\":\"\\\"I'm ready.\\\" The Endministrator of Endfield Industries.\\nAccording to the historical records of Talos-II, the Endministrator stands as a key guardian who protected civilization on this planet and saved humanity from multiple catastrophic disasters. The Endmin's heroic deeds have given rise to many stories, tales, and even rumors.\\nHowever, the Endmin's identity remains elusive and only a handful of people have seen the legend in person.\\nOther factions within the Civilization Band are well aware that Endfield has a very special trump card in their hand, but that is all they know.\\nThe Endministrator's mysterious mastery over Originium and the Protocol network, along with the Endmin's need for periodic long-term hibernation, is one of Endfield's most important secrets.\",\"cid\":\"1m3Ajim9hj39zTho\",\"anime\":true},{\"key\":\"endministrator2\",\"code\":\"02\",\"name\":\"Endministrator\",\"codename\":\"Endministrator\",\"camp\":\"Endfield Industries\",\"race\":\"[data_missing]\",\"intro\":\"\\\"I'm ready.\\\" The Endministrator of Endfield Industries.\\nAccording to the historical records of Talos-II, the Endministrator stands as a key guardian who protected civilization on this planet and saved humanity from multiple catastrophic disasters. The Endmin's heroic deeds have given rise to many stories, tales, and even rumors.\\nHowever, the Endmin's identity remains elusive and only a handful of people have seen the legend in person.\\nOther factions within the Civilization Band are well aware that Endfield has a very special trump card in their hand, but that is all they know.\\nThe Endministrator's mysterious mastery over Originium and the Protocol network, along with the Endmin's need for periodic long-term hibernation, is one of Endfield's most important secrets.\",\"cid\":\"hhSgWdCWUtDDdMoy\",\"anime\":true},{\"key\":\"perlica\",\"code\":\"03\",\"name\":\"Perlica\",\"codename\":\"Perlica\",\"camp\":\"Endfield Industries\",\"race\":\"Liberi\",\"intro\":\"\\\"There was a time when I could only gaze upon your deeds from afar. But at this very moment, my only wish is to fight by your side.\\\"\\nPerlica is the Supervisor of Endfield Industries and the official spokesperson of the company. Her roles include promoting the development and applications of Techno-Protocol and managing the ship Dijiang.\\nAs an outstanding expert in Techno-Protocol, her studies have had a key influence on the adaptive development and application of this technology and laid the cornerstone for the Automated Industry Complex (AIC) project.\\nThere are many rumors about Perlica. One claims that she once succumbed to severe Blight infection and was taken to a medical facility for treatment where she miraculously survived. But to keep her alive, part of her spinal cord and several organs were replaced with Originium structures. Another rumor claims she is entrusted with a mysterious Directive. Despite being the official spokesperson of Endfield Industries, Perlica has never responded to such claims.\\nShe is capable of responding to various crises in a calm and decisive manner. However, Endfield often faces challenges that go beyond their expectations. Perlica often has to move between bases and outposts, doing whatever she can to maintain order and keep things running.\\n\\n\\n\\\"We don't simply talk about our dreams. No. We do what we can to make them a reality.\\\"\",\"cid\":\"loTXg3IZM6hrmI7R\",\"anime\":true},{\"key\":\"chen\",\"code\":\"04\",\"name\":\"Chen Qianyu\",\"codename\":\"Chen Qianyu\",\"camp\":\"Endfield Industries\",\"race\":\"Lung\",\"intro\":\"\\\"Detailed plans have a habit of going wrong. I prefer a more direct and physical approach...\\\"\\nChen Qianyu is a specialist operator of Endfield Industries.\\nShe began martial arts training at a young age, which gave her outstanding combat capabilities and a mental fortitude rarely seen in people her age.\\nAfter joining the Yinglung Special Task Force, her relentless commitment to perfecting her fighting skills sparked many stories about her. Everyone who had the chance to meet Chen in person agrees that, no matter the situation or circumstances, this optimistic girl will always stay true to herself and never give up in the face of adversity.\\n\\n\\n\\\"Perlica can deal with most problems on her own, but she always wants me around—Well, you should know that some issues won't be solved without getting physical.\\\"\",\"cid\":\"s4FOfmRxwR0B5XVY\",\"anime\":true},{\"key\":\"wulfgard\",\"code\":\"05\",\"name\":\"Wulfgard\",\"codename\":\"Wulfgard\",\"camp\":\"Endfield Industries\",\"race\":\"Lupo\",\"intro\":\"\\\"I know that strength isn't everything, but in Talos-II, you can't live without it.\\\"\\nAs a young mercenary living in the fringes of civilization, Wulfgard is already very familiar with the most fickle nature of humanity. But this has never consumed his sanity and instead gives him a maturity that goes far beyond his age.\\nWulfgard will not actively pick a fight with others. If he has doubts, he simply questions things in his own special way. Wulfgard is also skilled in stealth and recon combat operations. To complete his mission, the mercenary can even travel with his foes and deal the lethal strike when the enemies least expect it.\\nA former member of the Landbreaker clan known as the Pack, Wulfgard can be a reliable and trustworthy friend if you can earn his trust. However, this is no easy feat.\\n\\n\\n\\\"Idealism is always detached from reality, and it's the first step towards disappointment.\\\"\",\"cid\":\"O9BsDnWpYjpMcLmk\",\"anime\":true},{\"key\":\"dapan\",\"code\":\"06\",\"name\":\"Da Pan\",\"codename\":\"Da Pan\",\"camp\":\"Hongshan\",\"race\":\"Ursus\",\"intro\":\"\\\"Oh. Thank you. You are being too kind. I'll return this favor. Tell me if you need anything!\\\"\\nBack in his hometown, Da Pan worked at the Hongshan Academy of Sciences (HAS) in various \\\"discrete administrative support roles for key scientific research\\\", which is a fancy way of saying jobs like chef, electrician, carpenter, chauffeur, street vendor, and more. His latest assignment saw him being dispatched by the HAS (through Operator Chen Qianyu's recommendation) to serve as a specialist liaison at Endfield Industries.\\nAmong the long list of his employment history, Da Pan treats chef work as the crowning achievement of his professional career. He is extremely passionate about cooking and readily engages himself in the creation of new recipes, inventing fusion dishes, and identifying novel ingredients. His favorite pastime is to prepare palatable cuisine that diners might enjoy.\\nDa Pan carries a special bottle with him wherever he goes. It doesn't look like anything special to most people, but anyone familiar with the writing of Yan would wonder about its origins. However, if that person happens to be from Hongshan, they would immediately recognize its origins and significance, because they know very well that the bottle is an item of honor only awarded to \\\"major contributors to science\\\".\\n\\n\\n\\\"Need help at the canteen? I could make a few dishes, quick and easy.\\\"\",\"cid\":\"RPAbM0F0L5NqRflr\",\"anime\":true},{\"key\":\"lifeng\",\"code\":\"07\",\"name\":\"Lifeng\",\"codename\":\"Lifeng\",\"camp\":\"Hongshan\",\"race\":\"Anasa\",\"intro\":\"\\\"Lifeng of the Wuling Guards reporting for duty.\\\"\\nLifeng is a member of Wuling City Guards who currently interns at the Specialist Tech Division of Endfield Industries.\\nWhen he was little, Lifeng had a happy family like other kids in Wuling. However ... a horrific incident crushed his arm and took the lives of his parents. The less-than-10-year-old boy had no choice but to mature quickly.\\nHe accepted a prosthetic arm and poured effort into every step of his recovery. He endured through the pain of maturing and emerged as a capable and dependable young man. Whether serving as a Wuling City Guards reservist to help folks out, or protecting his little sister as a caring brother, the 16-year-old tackles his responsibilities with a level of thoughtfulness and attentiveness that consistently impresses—and sometimes amazes—those around him.\\nThis is why one should never dismiss Lifeng as merely a naive teenager. In fact, he carries a wealth of deep thoughts and emotions that he keeps to himself. Only when visiting his late parents' graves does he reveal a glimpse of his deep inner world.\\n\\n\\n\\\"I wanna do more, and get better at it!\\\"\",\"cid\":\"002KE14QvnejoxbZ\",\"anime\":true},{\"key\":\"xaihi\",\"code\":\"08\",\"name\":\"Xaihi\",\"codename\":\"Xaihi\",\"camp\":\"Cabal of Tranquility\",\"race\":\"Sarkaz\",\"intro\":\"\\\"We share the same knowledge and fate.\\\"\\nFew people could immediately identify Xaihi's origins upon their first meeting with her. But the girl's occasional display of \\\"Lateran vibes\\\" would quickly reveal her affiliation with the Cabal of Tranquility.\\nAccording to what Xaihi said about herself, people believe that her past gave her knowledge and skills that should not be wielded by someone her age. This is evident in Xaihi's elegant mastery of the Originium Arts and proficiency in various fields, especially those pertaining to information technology.\\nDespite her appearances, the way that Xaihi talks and acts is hardly typical of a girl who grew up in a cloister. One must get used to her vast and seemingly trivial knowledge as well as her tendency to jump from one strange topic to the next. Xaihi herself, however, is utterly unaware of these issues.\\n\\n\\n\\\"The perfect logic already exists. Our good works will slowly lift its veil.\\\"\",\"cid\":\"VMnluxdYY64Uu3Qi\",\"anime\":true},{\"key\":\"ember\",\"code\":\"09\",\"name\":\"Ember\",\"codename\":\"Ember\",\"camp\":\"Order of Steel Oath\",\"race\":\"Sankta\",\"intro\":\"\\\"Awaiting command.\\\"\\nEmber was an Oathkeeper Knight from the Northern Desolation who served a military organization known as the Order of Steel Oath. Guided by her commitment to the Final Tribulation, she visited the Civilization Band before accepting an invitation to join Endfield Industries.\\nAs a veteran who spent years on the battlefield, Ember is not skilled in conversing with others. However, if someone starts talking about combat techniques with Ember, she will give earnest descriptions of her experiences and the battles she fought. Nevertheless, these \\\"experiences\\\" sound like wild stories to most.\\n\\n\\n\\\"My blade is cast and honed by the Northern frost. I only pray that the southern sun has not softened its edge.\\\"\",\"cid\":\"Xp6Y4bSWetd5fyNB\",\"anime\":true},{\"key\":\"arclight\",\"code\":\"10\",\"name\":\"Arclight\",\"codename\":\"Arclight\",\"camp\":\"Fiannæ Circuit\",\"race\":\"Kuranta\",\"intro\":\"\\\"I am Ekut. Callsign is Arclight. I am not a trained weaver of words, but I wish you well and may you flow like a swollen river, Endfielders.\\\"\\nArclight is a scabhta of the Fiannæ Circuit. She is currently partnering with Endfield Industries as a technical consultant.\\nNo one roaming the wildlands could escape the grip of starvation and disasters, no matter how clever or capable they may think themselves to be. Misfortune struck Arclight hard. The wildlands consumed her entire family, leaving her the only survivor. However, fortune also smiled upon her when a group of Fiannæ Circuit members found and saved her from the brink of death.\\nThe Fiannæ Circuit accepted Arclight. There, she quickly learned how to survive in a harsh landscape and follow the practice of Kerns from poems to help people out. Today, she roams the wildlands to teach more souls the way of surviving in the unforgiving world of Talos-II.\\nArclight may come across as cold and brusque at first, but the minimalist philosophy underlying her cold demeanor actually proves to be exceptionally effective for surviving in the wildlands.\\n\\n\\n\\\"And it shall be like fire ... offering warmth and illumination.\\\"\",\"cid\":\"qMIjdvqY0G8ej2zT\",\"anime\":true},{\"key\":\"laevatain\",\"code\":\"11\",\"name\":\"Laevatain\",\"codename\":\"Laevatain\",\"camp\":\"Rhodes Island\",\"race\":\"Sarkaz\",\"intro\":\"\\\"Twilight of ruin!\\\"\\nLaevatain is a Rhodes Island Operator assigned to Endfield Industries where she serves as a specialist operator following the framework agreement between the two companies.\\nWith Warfarin's authorization, Laevatain once spent years in the north to carry out a solo field mission, thanks to her strong skills in independent operations. What's even more impressive is her exceptional performance in Endfield's tests on teamwork adaptability and communication skills.\\nThis is also reflected in her peer feedback. Most Endfielders who have worked with Laevatain give her high praise. They say she might come across as a \\\"fiery girl\\\" at first, but she's actually not fiery at all. Instead, she is an excellent listener and coordinator.\\nHowever, she sometimes talks about things nobody really understands, like \\\"the threat of demons\\\". Most people assume she's referring to the Aggeloi, while others just have no idea what she really means.\\nShe spends a lot of time in freezing environments and has a surprising fondness for ice cream.\\n\\n\\n\\\"Hmm... This box of ice cream would melt by the time they reach Warfarin. I'll just eat them myself.\\\"\",\"cid\":\"5uNhH6NRZpOMDORr\",\"anime\":true},{\"key\":\"yvonne\",\"code\":\"12\",\"name\":\"Yvonne\",\"codename\":\"Yvonne\",\"camp\":\"Endfield Industries\",\"race\":\"Vouivre\",\"intro\":\"\\\"Ætherside... An unknown and dangerous world. But the unknown is always exciting, right?\\\"\\nYvonne is a scientist in the Specialist Tech Division of Endfield Industries.\\nUnlike the stereotypical nerd, Yvonne comes across as a chic, fashionable girl. She dyes her horns, tail, and nails in bright colors; she modifies her attire into her favorite flamboyant designs; she would skip academic conferences to buy fashion magazines and popular albums. Yvonne is always drawn to oddities and novelties. She defies conventions and always stays ahead of the latest trends.\\nShe is not your typical straight-A student, yet no one would doubt her talent and brilliance. Even before graduating from college, she had achieved some impressive results in her field. Everyone expected her to work at a major research institute and lead the advancement of science in this era. But to people's surprise, she joined Endfield instead.\\nThe reason? She never explained it. Maybe you can ask her yourself.\\n\\n\\n\\\"I'm coming! Don't worry. I'll fix those Æther facility problems for you.\\\"\",\"cid\":\"5VPM6vufLsTLCccw\",\"anime\":true},{\"key\":\"snowshine\",\"code\":\"13\",\"name\":\"Snowshine\",\"codename\":\"Snowshine\",\"camp\":\"Rhodes Island\",\"race\":\"Ursus\",\"intro\":\"\\\"No frigid cold or blizzard is gonna stop me! I'll be your most reliable shield!\\\"\\nSnowshine is a Rhodes Island Operator assigned to Endfield Industries where she serves as a search and rescue (SAR) specialist following the framework agreement between the two companies.\\nFor well-known reasons, the polar region of Talos-II remains a forbidden area to humanity. However, as efforts to expand the frontier resume, professionals like Snowshine who specialize in the geography of polar and high-altitude regions are in high demand within the Civilization Band.\\nBecause of the significant talent gap, Snowshine and her rescue team are always faced with a busy schedule, even on days with no special missions.\\nBut despite her packed schedule, she wrote a book titled Polar Survival Guide during her free time. The book has become a source of essential guides for surviving and conducting SAR operations in the cold, snowy environments of Talos-II.\\n\\n\\n\\\"The brilliant lights of Talos remind me of the grand auroras that I once witnessed...\\\"\",\"cid\":\"8qbj4I36DfawYe78\",\"anime\":true},{\"key\":\"gilberta\",\"code\":\"14\",\"name\":\"Gilberta\",\"codename\":\"Gilberta\",\"camp\":\"Rhodes Island\",\"race\":\"Vulpo\",\"intro\":\"\\\"Greetings, Endmin! Oh, you are the smell of nostalgia and homesickness ... like an incomplete letter destined for a special place. Now I wonder where that might be?\\\"\\nGilberta is a Rhodes Island Operator assigned to Endfield Industries where she serves as a Messenger following the framework agreement between the two companies.\\nGilberta has extraordinary talents in Originium Arts and wields an extremely rare aspect of the Arts. She once made bold assumptions that her Arts might warp gravity to a certain extent, but Warfarin didn't respond to her guess.\\nAt Endfield, Gilberta is in charge of mail delivery and intel escort missions across multiple bases and outposts. Her duties often require her to travel along the boundaries between the Civilization Band and the frontier, but she seems to really enjoy the job. Recently she even started asking for more challenging missions in high-risk areas to hone her skills.\\nGilberta appears a sunny optimist with a dash of anxiety but plenty of hope for the future. Her personality sometimes took Warfarin by surprise, but also brought her deep nostalgia.\\n\\n\\n\\\"Wow. Did you just bring a bright future to me?\\\"\",\"cid\":\"wiJlBWtISnfLoGB0\",\"anime\":true},{\"key\":\"avywenna\",\"code\":\"15\",\"name\":\"Avywenna\",\"codename\":\"Avywenna\",\"camp\":\"Talos-II General Chamber of Commerce\",\"race\":\"Cautus\",\"intro\":\"\\\"Greetings, Endministrator. I knew I would find you here. There are times when you can actually rely on me, just so you know.\\\"\\nAvywenna is an Armed Messenger.\\nBefore joining Endfield Industries, she worked at the Talos-II General Chamber of Commerce (TGCC). However, she soon realized that the neon-lit city fraught with lies and cracks between people wasn't really the ideal place for her. With this realization, she departed from her job and eventually found herself at a mail depot in the wild frontier.\\nThere, she resolved to dedicate her life to the vast wilderness as well as the well-being of its troubled residents. Her noble intentions did not go unnoticed. Not wanting to see her kindness and talent go to \\\"waste\\\", a sincere and heartfelt recommendation letter was sent from the wildlands and brought Avy to Endfield Industries.\\nAvywenna is an amicable, confident, and trustworthy lady who can manage various relationships in a sophisticated and professional manner.\\nAs to whether she's truly as good-tempered as she seems, you'll have to find out by engaging with her yourself.\\n\\n\\n\\\"Does that make me your first choice?\\\"\",\"cid\":\"WuVFmRj1Th5EjBYc\",\"anime\":true}],\"cbt2\":{\"recruitment\":{\"questionnaire\":true,\"inquiry\":true,\"activity\":true}},\"sections\":[{\"key\":\"Home\",\"data\":{\"pv\":\"https://web-static.hg-cdn.com/endfield/official-v3/assets/videos/home_pc_en_us_0tGzNpwXVtOGUGcz.mp4\",\"pvH5\":\"https://web-static.hg-cdn.com/endfield/official-v3/assets/videos/home_h5_en_us_9PPrW0siccfCEwMT.mp4\"}},{\"key\":\"Trailer\",\"data\":{\"pvList\":[{\"title\":\"Arknights: Endfield Special Trailer | Endfield: Those Who Stayed\",\"embed\":\"https://www.youtube.com/embed/H-rls7leIX8\",\"cover\":\"https://web-static.hg-cdn.com/endfield/official-v3/assets/img/pv_cover_8_en_us_b9ySzeGjuKfQbSOl.png\"},{\"title\":\"Arknights: Endfield Gameplay Demo 03\",\"embed\":\"https://www.youtube.com/embed/UbMmSdOJ1Ho\",\"cover\":\"https://web-static.hg-cdn.com/endfield/official-v3/assets/img/pv_cover_7_en_us.jpg\"},{\"title\":\"Arknights: Endfield Beta Test PV\",\"embed\":\"https://www.youtube.com/embed/EfwGJH-etgk\",\"cover\":\"https://web-static.hg-cdn.com/endfield/official-v3/assets/img/pv_cover_6_en_us.jpeg\"}]}},{\"key\":\"Chars\",\"data\":{}},{\"key\":\"News\",\"data\":{}}],\"download\":true}}";


            ctx.Response.StatusCode = 200;

            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
