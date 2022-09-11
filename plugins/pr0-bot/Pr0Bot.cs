using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using TS3AudioBot;
using TS3AudioBot.Audio;
using TS3AudioBot.Plugins;
using TS3AudioBot.CommandSystem;
using TS3AudioBot.Helper;
using TS3AudioBot.Config;

using TSLib.Full;
using TSLib.Messages;

using Nett;

namespace Pr0Bot
{
    public class Pr0Bot : IBotPlugin
    {

        private const string PR0GRAMM_GENERAL = "https://pr0gramm.com/api/items/get?id=";
        private const string PR0GRAMM_DETAIL = "https://pr0gramm.com/api/items/info?itemId=";
        private const string PR0GRAMM_COOKIE_FILE = "pr0gramm.cookie";

        private readonly Player player;
        private readonly Ts3Client client;
        private readonly ConfBot config;
        private readonly TsFullClient tsClient;

        public Pr0Bot(Player player, Ts3Client client, TsFullClient ts3FullClient, ConfBot config)
        {
            this.player = player;
            this.client = client;
            this.config = config;
            this.tsClient = ts3FullClient;

        }

        public ConfBot botConf { get; set; }
        public ConfRoot rootConf { get; set; }


        public void Initialize()
        {
            tsClient.OnEachTextMessage += OnTextMessage;
        }

        [Command("pr0gramm cookie")]
        public static string CommandCookie(ConfBot botConf, ConfRoot rootConf, string value)
        {
            string configPath = Path.Combine(rootConf.Configs.BotsPath.Value, botConf.Name, PR0GRAMM_COOKIE_FILE);
            try
            {
                File.WriteAllText(configPath, value);
            }
            catch (Exception ex)
            {
                return "Failed to write " + configPath + "\n" + ex.Message;
            }
            return "Successfully saved cookie";
        }

        private async void OnTextMessage(object sender, TextMessage client)
        {
            //if (client.InvokerId == tsClient.ClientId) return;
            string cleanMessage = client.Message.Replace("[URL]", "").Replace("[/URL]", "").ToLower();
            if (!cleanMessage.StartsWith("https://") && !cleanMessage.StartsWith("http://")) return;
            if (!cleanMessage.Contains("pr0gramm.")) return;

            Regex word = new Regex(@"\d+$");
            Match m = word.Match(cleanMessage);
            string POST_ID = m.Value;

            if (POST_ID == "") return;

            string configPath = Path.Combine(rootConf.Configs.BotsPath.Value, botConf.Name, PR0GRAMM_COOKIE_FILE);
            string savedCookie = "";
            if (File.Exists(configPath))
            {
                savedCookie = File.ReadAllText(configPath);
            }

            if (savedCookie == "")
            {
                tsClient.SendChannelMessage("Require Pr0gramm Cookie for API requests. Use !pr0gramm cookie  <ME-COOKIE>");
                return;
            }

            if (cleanMessage.Contains("/new/") || cleanMessage.Contains("/top/"))
            {
                string cookie = "me=" + savedCookie + ";";

                try
                {
                    var generalData = await WebWrapper.Request(PR0GRAMM_GENERAL + POST_ID + "&flags=15").WithHeader("Cookie", cookie).AsJson<PostInfo>();
                    if (generalData.items == null)
                    {
                        tsClient.SendChannelMessage("No posts items found for post " + POST_ID + ". Error: " + generalData.error);
                        return;
                    }

                    var detailData = await WebWrapper.Request(PR0GRAMM_DETAIL + POST_ID).WithHeader("Cookie", cookie).AsJson<TagsInfo>();
                    if (detailData.tags == null)
                    {
                        tsClient.SendChannelMessage("No posts tags found for post " + POST_ID + ". Error: " + generalData.error);
                        return;
                    }

                    var post = generalData.items[0];

                    string output = "\nPost by {user} ({benis}) in category {flag} ({imgurl}).\n- {first_tag}\n- {second_tag}\n- {third_tag}";

                    string flagstr = "nsfp";
                    if (post.flags == 1) flagstr = "sfw";
                    else if (post.flags == 2) flagstr = "nsfw";
                    else if (post.flags == 3 || post.flags == 4) flagstr = "nsfl";

                    List<Tag> sortedTags = new List<Tag>(detailData.tags);
                    sortedTags.Sort((x, y) => y.confidence.CompareTo(x.confidence));

                    output = output.Replace("{user}", post.user);
                    output = output.Replace("{id}", POST_ID);
                    output = output.Replace("{benis}", (post.up - post.down).ToString());
                    output = output.Replace("{up}", post.up.ToString());
                    output = output.Replace("{down}", post.down.ToString());
                    output = output.Replace("{flag}", flagstr);
                    output = output.Replace("{imgurl}", "[URL]https://img.pr0gramm.com/" + post.image + "[/URL]");
                    output = output.Replace("{first_tag}", sortedTags[0].tag);
                    output = output.Replace("{second_tag}", sortedTags[1].tag);
                    output = output.Replace("{third_tag}", sortedTags[2].tag);

                    tsClient.SendChannelMessage(output);
                }
                catch (Exception ex)
                {
                    tsClient.SendChannelMessage("Failed to request pr0gramm api: " + ex.Message);
                }

            }
            else if (cleanMessage.Contains("/user/"))
            {
                tsClient.SendChannelMessage("[URL]https://pr0gramm.com/new/" + POST_ID + "[/URL]");
            }
        }

        public void Dispose()
        {
            tsClient.OnEachTextMessage -= OnTextMessage;
        }

#pragma warning disable CS0649, CS0169, IDE1006, CS8632
        private class Pr0Config
        {
            public string cookie { get; set; } = "";
        }

        private class ConfPr0 : ConfigTable
        {
            public ConfigValue<string> cookie { get; }
        }

        private class PostInfo
        {
            public string? error { get; set; }
            public int ts { get; set; }
            public Item[]? items { get; set; }
        }

        private class Item
        {
            public int up { get; set; }
            public int down { get; set; }
            public int flags { get; set; }
            public string image { get; set; }
            public string user { get; set; }
        }

        private class TagsInfo
        {
            public string? error { get; set; }
            public int ts { get; set; }
            public Tag[]? tags { get; set; }
        }

        private class Tag
        {
            public string tag { get; set; }
            public int id { get; set; }
            public float confidence { get; set; }
        }
#pragma warning restore CS0649, CS0169, IDE1006, CS8632

    }

}