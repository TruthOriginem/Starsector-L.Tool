using StarsectorLTool.Src.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace StarsectorLTool.Src.UI
{
    class ConfigItemCheckBox : CheckBox
    {
        private TextBlock linkedTextBlock;
        private ModConfigData configData;
        private string itemKey;
        public ConfigItemCheckBox(ModConfigData configData, string itemKey, TextBlock linkedTextBlock)
        {
            this.configData = configData;
            this.linkedTextBlock = linkedTextBlock;
            this.itemKey = itemKey;
            this.Content = itemKey;
            IsChecked = bool.Parse(configData[itemKey]);
        }
        protected override void OnClick()
        {
            base.OnClick();
            configData[itemKey] = (IsChecked ?? false) ? bool.TrueString : bool.FalseString;
            linkedTextBlock.Text = configData.GetConfigItemRDME(itemKey);
        }
    }
}
