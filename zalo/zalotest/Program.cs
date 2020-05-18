using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZaloCSharpSDK;
using ZaloDotNetSDK;

namespace zalotest
{
    class Program
    {
        static void Main(string[] args)
        {
            ////ZaloAppInfo appInfo = new ZaloAppInfo(4493888734077794545, "2KOY8CIBqKEwbGJ7TV1k", "http://zalo.iot.vn");
            ////Zalo3rdAppClient appClient = new Zalo3rdAppClient(appInfo);
            ////string loginUrl = appClient.getLoginUrl();
            //////Process.Start(loginUrl);
            ////string code = "bNd-9ViT9GcWOAm1xH8SHRf2vrcVA1n0_t62LgTtP4dnBwTJdrfyPEWlyNE2J0zXgYdpJTXXJ6QzSvXDvIjNMejGtrdM6sPzwLYXQSSaM7oxHjTGnKmn3uivrssw3mKxgogJAjfDU2QGTjXZhL8LMRCquKJjUofGanU1TTXd0t7_1lnymXL2BVmZgrBEMXOAZmxU4lznKmYgVg9uxsGbSD9QWZhbANj5rd30G9XIKnsfSuq8umKQ79nmtGgt328Pr7_FOk8iQISiUv89Ka4e0oibbMTySbPZPYFGMOmiJaNS7fJaEKDlosSFzQ1Q76_18Hc1iM8IQSKQ2F-GOsfw_5eenFTMGIkJMIRuE0bemK0g2G";
            ////JObject token = appClient.getAccessToken(code);
            ////var access_token = token["access_token"].ToString();

            //////JObject profile = appClient.getProfile(access_token, "id, name, birthday");
            ////JObject friends = appClient.getFriends(access_token, 0, 100, "id, name, picture");
            //////JObject sendMessage = appClient.sendMessage(access_token, 3852331461584449386, textBox1.Text, "");

            string access_token = "0kFN5_hltI83vzmRyk_P8GhatXZ-zRbdL_k_6ywgltTkuTqkoeg_LthDnX2AmP0i28_lQftyfnDnyTDOv9QlONhOoGdmj9DbK-xcFjwQlb5qyS8oZOIK22_Ezr6caeyR3-7kHeAfhGOjeQeoX_d-2tMfb33Q-CvlOv7C1CtwoqTUbhSkmCBkKLYsj6J0qTSuRR-zGDItvZXu_BDDqeZq5apufNdIYleIGOAkTy7op2bqXOXTpOdZQ4loZZR0kjTELkQ63F6UuN1Wq958DC4_CWtmyj4k";
            ZaloClient zc = new ZaloClient(access_token);
            //var p1 = JsonConvert.SerializeObject(zc.getProfileOfFollower("7287083737778696997"), Formatting.Indented);
            //var p2 = JsonConvert.SerializeObject(zc.getProfileOfFollower("7900271606406461606"), Formatting.Indented);

            var p3 = JsonConvert.SerializeObject(zc.getProfileOfFollower("0926111347"), Formatting.Indented); 
        }
    }
}
