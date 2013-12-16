using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class FlashlightApp : PhoneApp
    {
        private const float SPAN_DEFAULT = MathHelper.Pi;
        private const float RANGE_DEFAULT = 500;
        private const float POWER_DEFAULT = 0.4f;

        private Light light { get; set; }
        public float Range { get; set; }
        public Color Colour { get; set; }
        public float Span { get; set; }

        public override int ConsumeRate
        {
            get
            {
                return Math.Max(1, (int)(base.ConsumeRate * light.Brightness)); //Math.Max ensures that it's at least 1
            }
        }


        public FlashlightApp(int type, SmartPhone phone, int consumeRate, Entity owner = null, string Title = "Flashlight v1.0")
            : base(type, phone, consumeRate, owner, Title)
        {
            this.Range = RANGE_DEFAULT;
            this.Span = SPAN_DEFAULT;
            this.Colour = Color.White;
            this.Power = POWER_DEFAULT;

            light = new Light(Power, Range, Vector2.Zero, Vector2.UnitX, Span, Colour, Level.CurrentLevel.Tiles, false);
            Level.CurrentLevel.AddLight(light);

            UIGrid flashlightControls = new UIGrid(Width: SmartPhone.SCREEN_RECT.Width, GridColumns: 1, CursorType: CursorType.Cursor, MarginBottom: 20);
            UISlider brightnessSlider = new UISlider(Color.Aqua, Color.Aqua, Textures.SmartPhoneSliderPiece, Textures.SmartPhoneSlider, CursorType: CursorType.Cursor, MarginBottom: 10);
            brightnessSlider.OnSliderChanged += o => { light.Brightness = brightnessSlider.SliderPosition / 100f; };

            flashlightControls.AddElement(new TextSprite("Brightness", 1, Color.GhostWhite, CursorType.Cursor));
            flashlightControls.AddElement(brightnessSlider);

            UISlider redSlider = new UISlider(Color.Aqua, Color.Aqua, Textures.SmartPhoneSliderPiece, Textures.SmartPhoneSlider, CursorType: CursorType.Cursor, MarginBottom: 5);
            redSlider.OnSliderChanged += o => { light.Colour = new Color((byte)(redSlider.SliderPosition * 2.55f), light.Colour.G, light.Colour.B); };

            flashlightControls.AddElement(new TextSprite("Red", 1, Color.GhostWhite, CursorType.Cursor));
            flashlightControls.AddElement(redSlider);


            UISlider greenSlider = new UISlider(Color.Aqua, Color.Aqua, Textures.SmartPhoneSliderPiece, Textures.SmartPhoneSlider, CursorType: CursorType.Cursor, MarginBottom: 5);
            greenSlider.OnSliderChanged += o => { light.Colour = new Color(light.Colour.R, (byte)(greenSlider.SliderPosition * 2.55f), light.Colour.B); };

            flashlightControls.AddElement(new TextSprite("Green", 1, Color.GhostWhite, CursorType.Cursor));
            flashlightControls.AddElement(greenSlider);


            UISlider blueSlider = new UISlider(Color.Aqua, Color.Aqua, Textures.SmartPhoneSliderPiece, Textures.SmartPhoneSlider, CursorType: CursorType.Cursor, MarginBottom: 5);
            blueSlider.OnSliderChanged += o => { light.Colour = new Color(light.Colour.R, light.Colour.G, (byte)(blueSlider.SliderPosition * 2.55f)); };

            flashlightControls.AddElement(new TextSprite("Blue", 1, Color.GhostWhite, CursorType.Cursor));
            flashlightControls.AddElement(blueSlider);
            
            PageContent.AddElement(flashlightControls);

            PageContent.AddElement(CreateSwitchAppButton("Main Menu", APP_MAINMENU));
        }
        
        ~FlashlightApp()
        {
            if (light != null)
                Level.CurrentLevel.RemoveLight(light);
        }

        public override void Update()
        {
            light.Update(Owner.Position + Owner.Origin / 2, new Vector2((float)Math.Sin(Owner.Rotation), (float)Math.Cos(Owner.Rotation)));
        }

        public override void Activate()
        {
            light.IsActive = true;
        }

        public override void Deactivate()
        {
            light.IsActive = false;
        }
    }
}
