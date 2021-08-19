using OptionalUI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ManyMoreFixes
{
    public class MMFOption : OptionInterface
    {
        public MMFOption() : base(mod: MMFMod.mod)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[1];
            Tabs[0] = new OpTab("Main options");
            labelID = new OpLabel(new Vector2(100f, 550f), new Vector2(400f, 40f), rwMod.ModID, 0, true);
            labelDsc = new OpLabel(new Vector2(100f, 500f), new Vector2(400f, 20f), "Extensive collection of bugfixes, performance and quality of life features for Rain World", 0, false);
            labelVersion = new OpLabel(new Vector2(410f, 565f), new Vector2(200f, 20f), "Version: " + rwMod.Version, FLabelAlignment.Center, false);
            labelAuthor = new OpLabel(new Vector2(210, 535f), new Vector2(200f, 20f), "by " + rwMod.author, FLabelAlignment.Left, false);

            labelFPS = new OpLabel(new Vector2(80, 445), new Vector2(80f, 20f), "FPS Limit (min:40 ~ max:unlimited)", FLabelAlignment.Left, false);
            labelFPS.description = "Adjusts the frames per second. (145 will uncap the limit)";

            fpsSlider = new OpSlider(new Vector2(85, 400f), "Fps", new RWCustom.IntVector2(40, 145), 1f, false, 60);
            fpsSlider.description = "Adjusts the frames per second. (145 will uncap the limit)";

            unlockedLabelFPS = new OpLabel(new Vector2(80, 370f), new Vector2(80f, 20f), "FPS Unlimited (no cap)", FLabelAlignment.Left, false);
            unlockedLabelFPS.description = "Unlocked FPS";

            rectFPS = new OpRect(new Vector2(76f, 367), new Vector2(135f, 30f), 0.3f);
            rectFPS.doesBump = false;

            radioGroupQuality = new OpRadioButtonGroup("Quality", 0);
            labelRadioGroupQuality = new OpLabel(new Vector2(360f, 420f), new Vector2(80f, 20f), "Graphics quality", FLabelAlignment.Center, false);
            labelRadioGroupQuality.description = "Change graphics quality";
            rectQuality = new OpRect(new Vector2(340f, 300f), new Vector2(130f, 150f), 0.3f);
            rectQuality.doesBump = true;
            radioHigh = new OpRadioButton(new Vector2(360f, 373f));
            radioHigh.description = "Vanilla quality";
            labelRadioHigh = new OpLabel(new Vector2(390f, 373f), new Vector2(80f, 20f), "High", FLabelAlignment.Center, false);
            labelRadioHigh.description = "Vanilla quality";
            radioMedium = new OpRadioButton(new Vector2(360f, 343f));
            radioMedium.description = "Flatmode and reduced effects";
            labelRadioMedium = new OpLabel(new Vector2(390f, 343f), new Vector2(80f, 20f), "Medium", FLabelAlignment.Center, false);
            labelRadioMedium.description = "Flatmode and reduced effects";
            radioLow = new OpRadioButton(new Vector2(360f, 313f));
            radioLow.description = "Flatmode, reduced effects and room load";
            labelRadioLow = new OpLabel(new Vector2(390f, 313f), new Vector2(80f, 20f), "Low", FLabelAlignment.Center, false);
            labelRadioLow.description = "Flatmode, reduced effects and room load";
            radioGroupQuality.SetButtons(new OpRadioButton[]
            {
                radioHigh,
                radioMedium,
                radioLow
            });

            Tabs[0].AddItems(labelID, labelDsc, labelVersion, labelAuthor, fpsSlider, labelFPS, unlockedLabelFPS, rectFPS, labelRadioGroupQuality, rectQuality, labelRadioHigh, labelRadioMedium, labelRadioLow, radioGroupQuality, radioHigh, radioMedium, radioLow);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (fpsSlider.valueInt < 145)
            {
                unlockedLabelFPS.Hide();
                rectFPS.Hide();
            }
            else
            {
                unlockedLabelFPS.Show();
                rectFPS.Show();
            }
        }

        public override void ConfigOnChange()
        {
            base.ConfigOnChange();
            MMFMod.config.configMachine = true;
            MMFMod.config.fpsCap = int.Parse(OptionInterface.config["Fps"]);
            MMFMod.config.qualityDirty = (MMFMod.config.quality != MMFMod.Quality.HIGH - int.Parse(OptionInterface.config["Quality"]));
            MMFMod.config.quality = MMFMod.Quality.HIGH - int.Parse(OptionInterface.config["Quality"]);
            Debug.Log("Quality settings: " + MMFMod.config.quality);
            Debug.Log("FPS settings: " + MMFMod.config.fpsCap);
        }

        private OpLabel labelID;
        private OpLabel labelDsc;
        private OpLabel labelVersion;
        private OpLabel labelAuthor;
        private OpSlider fpsSlider;
        private OpLabel labelFPS;
        private OpRadioButtonGroup radioGroupQuality;
        private OpLabel labelRadioGroupQuality;
        private OpRadioButton radioHigh;
        private OpLabel labelRadioHigh;
        private OpRadioButton radioMedium;
        private OpLabel labelRadioMedium;
        private OpRadioButton radioLow;
        private OpLabel labelRadioLow;
        private OpRect rectQuality;
        private OpLabel unlockedLabelFPS;
        private OpRect rectFPS;
    }
}
