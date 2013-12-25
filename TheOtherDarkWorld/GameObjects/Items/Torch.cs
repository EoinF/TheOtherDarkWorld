using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Torch : Togglable
    {
        private const float SPAN_DEFAULT = MathHelper.Pi;
        private const float RANGE_DEFAULT = 100;

        private Light light { get; set; }
        public float Range { get; set; }
        public Color Colour { get; set; }
        public float Span { get; set; }


        public Torch(int type, Entity owner = null)
            : base(type, owner: owner)
        {
            Torch characteristics = GameData.GameItems[type] as Torch;

            this.Span = characteristics.Span;
            this.Range = characteristics.Range;
            this.Colour = characteristics.Colour;

            light = StateManager.CreateLight(Power, Range, Vector2.Zero, Vector2.UnitX, Span, Colour, false);
        }

        public Torch()  //Parameterless constructor for Xml serialization
        {
            this.Colour = Color.White;
            this.Span = SPAN_DEFAULT;
            this.Range = RANGE_DEFAULT;
        }

        ~Torch()
        {
            if (light != null)
                StateManager.RemoveLight(light);
        }

        public override void Deactivate()
        {
            IsActive = false;
            light.IsActive = false;
        }

        protected override void Toggle()
        {
            IsActive = !IsActive;
            light.IsActive = IsActive;
        }

        protected override void ApplyToggledPassive()
        {
            light.Update(Owner.Position + Owner.Origin / 2, new Vector2((float)Math.Sin(Owner.Rotation), (float)Math.Cos(Owner.Rotation)));
        }
    }
}