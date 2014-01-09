using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Block
    {
        public float Health { get; set; }
        public int Resistance { get; set; }
        public Color Colour { get;  set; }
        public Item[] Drops;
        public float Opacity { get; set; }

        public float Brightness {
            get
            {
                return (_brightness[0, 0] + _brightness[0, 1] + _brightness[1, 0] + _brightness[1, 1]) 
                    / 4;
            }
            set
            {
                _brightness[0, 0] = value;
                _brightness[0, 1] = value;
                _brightness[1, 0] = value;
                _brightness[1, 1] = value;
            } 
        }
        private float[,] _brightness { get; set; }
        public void SetBrightness(int x, int y, float value)
        {
            _brightness[x, y] = value;
        }
        public float GetBrightness(int x, int y)
        {
            return _brightness[x, y];
        }

        private Vector4[,] LightColour_Vector;
        public void SetLightColour(int x, int y, Color Colour)
        {
            LightColour_Vector[x, y] = Colour.ToVector4();
        }


        public bool IsVisible
        {
            get { return _isVisible[0, 0] || _isVisible[0, 1] || _isVisible[1, 0] || _isVisible[1, 1]; }
            set
            {
                _isVisible[0, 0] = value;
                _isVisible[0, 1] = value;
                _isVisible[1, 0] = value;
                _isVisible[1, 1] = value;
            }
        }
        private bool[,] _isVisible { get; set; }

        public void SetVisible(int x, int y, bool value)
        {
            _isVisible[x, y] = value;
        }
        public bool GetVisible(int x, int y)
        {
            return _isVisible[x, y];
        }

        public Block(int x, int y, byte Type)
        {
            Block characteristics = GameData.GameBlocks[Type];
            this.Colour = characteristics.Colour;
            this.Health = characteristics.Health;
            this.Drops = characteristics.Drops;
            this.Resistance = characteristics.Resistance;
            this.Opacity = characteristics.Opacity;
            this._brightness = new float[2, 2];
            this._isVisible = new bool[2, 2];
            this.LightColour_Vector = new Vector4[2, 2];
        }

        public Block() { }

        public void ApplyLighting()
        {
            Textures.LightingShader.Parameters["vis00"].SetValue(_isVisible[0, 0]);
            Textures.LightingShader.Parameters["vis01"].SetValue(_isVisible[0, 1]);
            Textures.LightingShader.Parameters["vis10"].SetValue(_isVisible[1, 0]);
            Textures.LightingShader.Parameters["vis11"].SetValue(_isVisible[1, 1]);
            Textures.LightingShader.Parameters["br00"].SetValue(_brightness[0, 0]);
            Textures.LightingShader.Parameters["br01"].SetValue(_brightness[0, 1]);
            Textures.LightingShader.Parameters["br10"].SetValue(_brightness[1, 0]);
            Textures.LightingShader.Parameters["br11"].SetValue(_brightness[1, 1]);
            Textures.LightingShader.Parameters["lightcolor00"].SetValue(LightColour_Vector[0,0]);
            Textures.LightingShader.Parameters["lightcolor01"].SetValue(LightColour_Vector[0,1]);
            Textures.LightingShader.Parameters["lightcolor10"].SetValue(LightColour_Vector[1,0]);
            Textures.LightingShader.Parameters["lightcolor11"].SetValue(LightColour_Vector[1,1]);
            Textures.LightingShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}

