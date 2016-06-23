using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NbtToObj.Gui;

namespace NbtToObj.Helpers
{
    static class UnrealEditorClipboard
    {
        public static string GetClipoard(WorldState worldState, List<GroupState> groupStates)
        {
            StringBuilder clipboard = new StringBuilder();
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (TextReader tr = new StreamReader(assembly.GetManifestResourceStream(kClipboardTemplateKey)))
            {
                clipboard.Append(tr.ReadToEnd());
            }

            string actorTemplate;
            using (TextReader tr = new StreamReader(assembly.GetManifestResourceStream(kActorTemplateKey)))
            {
                actorTemplate = tr.ReadToEnd();
            }

            StringBuilder actors = new StringBuilder();
            foreach (GroupState groupState in groupStates)
            {
                string meshName = worldState.outputName + "_" + groupState.groupName;
                actors.Append(actorTemplate.Replace("<!--MESHNAME-->", meshName));
            }

            clipboard.Replace("<!--ACTORS-->", actors.ToString());
            return clipboard.ToString();
        }

        private const string kClipboardTemplateKey = "NbtToObj.Helpers.ClipboardTemplate.txt";
        private const string kActorTemplateKey = "NbtToObj.Helpers.ActorTemplate.txt";
    }
}
