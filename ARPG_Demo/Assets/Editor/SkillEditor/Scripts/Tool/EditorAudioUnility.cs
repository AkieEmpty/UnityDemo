using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkieEmpty.SkillEditor
{
   
    public static class EditorAudioUnility
    {
        private static MethodInfo playClipMethodInfo;
        private static MethodInfo stopClipMethodInfo;
        static EditorAudioUnility()
        {
            GetMethodInfo();
        }
        private static void GetMethodInfo()
        {
            //UnityEditor.AudioUtilc类 只允许本程序集内访问,但是可以通过相同程序集内其他允许访问的类并通过反射获取需要的方法
            Assembly editorAssembly = typeof(UnityEditor.AudioImporter).Assembly;
            Type utilClassType = editorAssembly.GetType("UnityEditor.AudioUtil");
            playClipMethodInfo = utilClassType.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public, null,
                                    new Type[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
            stopClipMethodInfo = utilClassType.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="start">0~1的播放进度</param>
        public static void PlayAudio(AudioClip audioClip, float start)
        {
            playClipMethodInfo.Invoke(null, new object[] { audioClip, (int)(start * audioClip.frequency), false });
        }

        public static void StopAllAudio()
        {
            stopClipMethodInfo.Invoke(null, null);
        }
    }
}
