﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class GameObject
    {
        private const float MAX_BRIGHTNESS = 1;
        private const float MIN_BRIGHTNESS = 0.05f;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public virtual float Speed { get; set; }
        public virtual Color Colour { get; set; }
        public Vector2 Origin { get; private set; }
        public int Resistance { get; set; }
        public virtual Rectanglef Rect { get; private set; }
        public Texture2D Texture { get; set; }

        /// <summary>
        /// This object is visible to the player
        /// </summary>
        public bool IsVisible 
        {
            get { return StateManager.FULL_VISION || _isVisible; }
            set { _isVisible = value; } 
        }
        private bool _isVisible;
        private float _brightness;
        public float Brightness
        {
            get
            {
                if (StateManager.FULL_BRIGHT)
                    return MAX_BRIGHTNESS;
                else
                    return _brightness;
            }
            set
            {
                if (value > MAX_BRIGHTNESS)
                    _brightness = MAX_BRIGHTNESS;
                else if (value < MIN_BRIGHTNESS)
                    _brightness = MIN_BRIGHTNESS;
                else
                    _brightness = value;
            }
        }

        private Vector4 _lightColourVector;
        protected Vector4 LightColour_Vector
        {
            get { return StateManager.FULL_BRIGHT ? new Vector4(1,1,1,1) : _lightColourVector; }
            set { _lightColourVector = value; }
        }
        private Color _lightColour;
        public Color LightColour
        { 
            get { return StateManager.FULL_BRIGHT ? Color.White : _lightColour; }
            set 
            { 
                _lightColour = value;
                LightColour_Vector = value.ToVector4();
            } 
        }

        public virtual void ResetLighting()
        {
            this.Brightness = 0;
            this.LightColour = Light.DEFAULT_LIGHT;
        }
        
        public GameObject(Texture2D Texture, Vector2 startPosition, float speed, Color colour, Vector2 startVelocity, Vector2 Origin, int Resistance, int width, int height)
        {
            this.Texture = Texture;
            if (this.Texture == null) //Make sure the texture is never null
                this.Texture = Textures.Block;
            this.Brightness = 0;
            this.Position = startPosition - Origin;
            this.Origin = Origin;
            this.Speed = speed;
            this.Colour = colour;
            this.Velocity = startVelocity;
            this.Resistance = Resistance;
            this.Rect = new Rectanglef(Position.X, Position.Y, width, height);
            this.LightColour = Colour; //The lightcolour is it's own colour by default
        }


        public virtual void CheckCollisions(Tile[,] Tiles)
        {
            for (int checks = 0; checks < 2 && Velocity != Vector2.Zero; checks++)
            {
                List<Collision> collisions = new List<Collision>();
                //This rectangle is initialised here because it doesn't change until all the blocks have been checked
                Rectanglef RoughRect = new Rectanglef(Rect.Left + (Velocity.X < 0 ? Velocity.X : 0), Rect.Top + (Velocity.Y < 0 ? Velocity.Y : 0), Rect.Width + Math.Abs(Velocity.X), Rect.Height + Math.Abs(Velocity.Y));


                //The index of the tile in the array that the rectangle begins at
                int startX = (int)(RoughRect.Left / 10);
                int startY = (int)(RoughRect.Top / 10);


                //Check if the start index is out of bounds
                if (startX < 0)
                    startX = 0;
                if (startY < 0)
                    startY = 0;

                //The index of the tile in the array that the rectangle ends at
                //
                //The extra 2 is added to ensure that the area checked is big enough to detect all collisions
                //
                int endX = startX + (int)(RoughRect.Width / 10) + 2;
                int endY = startY + (int)(RoughRect.Height / 10) + 2;

                //Check if the end index is out of bounds
                if (endX >= Tiles.GetLength(0))
                    endX = Tiles.GetLength(0);
                if (endY >= Tiles.GetLength(1))
                    endY = Tiles.GetLength(1);


                //
                //The index should never be out of bounds at this stage
                //No further checking is necessary (so that should improve performance)
                //

                for (int i = startX; i < endX; i++)
                    for (int j = startY; j < endY; j++)
                    {
                        if (Tiles[i, j].Block == null) //If there is no block here,
                            continue; //go to the next tile

                        if (RoughRect.Intersects(Tiles[i, j].Rect))
                        {
                            collisions.Add(new Collision(this, Tiles[i, j], i, j));
                        }
                    }
                

                if (collisions.Count == 0) //There were no collisions
                {
                    Position += Velocity; //The object has moved the full distance available without colliding
                    return;
                }
                else
                {
                    //
                    //One important thing to note about the following is that a vertical and horizontal collision
                    //can happen at the same time. So one or the other is not enough. This has to be checked for.
                    //

                    //The least distance. (The soonest collision will get processed first)
                    int lowestIndex = 0;
                    bool collidingDiagonal = false;

                    //Start at 1, because we assume that the 0 index is the lowest first.
                    for (int i = 1; i < collisions.Count; i++)
                    {
                        if (collisions[i].pct < collisions[lowestIndex].pct)
                        {
                            lowestIndex = i;

                            //At this point, we know i is the index of soonest collision. We assume(until 
                            //another collision is found) that there is no collision perpendicular to this 
                            //one that happens at the same time so we set this variable to false
                            collidingDiagonal = false;
                        }
                        else if (collisions[i].pct == collisions[lowestIndex].pct)
                        {
                            //If they are colliding in different directions
                            if (collisions[i].IsHorizontal != collisions[lowestIndex].IsHorizontal)
                                collidingDiagonal = true;
                        }
                    }

                    if (collidingDiagonal)
                        CollideDiagonal(collisions[lowestIndex]);
                    else if (collisions[lowestIndex].IsHorizontal)
                        CollideHorizontal(collisions[lowestIndex]);
                    else
                        CollideVertical(collisions[lowestIndex]);
                }
            }
        }


        public virtual void CollideHorizontal(Collision col)
        {
            Position += new Vector2(col.S.X, col.S.Y);
            Velocity = new Vector2(0, Velocity.Y - (Math.Sign(Velocity.Y) * col.S.Y));
        }
        public virtual void CollideVertical(Collision col)
        {
            Position += new Vector2(col.S.X, col.S.Y);
            Velocity = new Vector2(Velocity.X - (Math.Sign(Velocity.X) * col.S.X), 0);
        }
        public virtual void CollideDiagonal(Collision col)
        {
            Position += new Vector2(col.S.X , col.S.Y);
            Velocity = new Vector2(Velocity.X, 0);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            ApplyLighting();
            if (IsVisible)
                spriteBatch.Draw(Texture, Position + Origin - StateManager.Offset, null, Colour, 0, Vector2.Zero, 1, SpriteEffects.None, UI.GAMEOBJECT_DEPTH_DEFAULT);
        }

        protected void ApplyLighting()
        {
            Textures.LightingShader.Parameters["lightcolor00"].SetValue(LightColour.ToVector4());
            Textures.LightingShader.Parameters["br00"].SetValue(Brightness);
            Textures.LightingShader.CurrentTechnique.Passes[1].Apply();
        }
    }
}
