using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkieEmpty.SkillEditor
{
    public class SkillEditorConfig
    {
        public const int StandFrameUnitWidth = 5;  // 标准帧单位宽度
        public const int maxFrameWidthLV = 10;      //  当前帧单位宽度
        public float defaultFrameRote =10;        //默认帧率

        public int CurrentFrameUnitWidth { get; set; } = 10;//当前帧的单位宽度
    }
}
