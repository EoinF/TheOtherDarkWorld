using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Light
    {
        private const float RotationSpeed = 0.02f;
        public float Brightness { get; private set; }
        public float Depth { get; private set; }

        public Vector2 Position { get; private set; }
        private Vector2 Direction;
        private float Span;
        private float RotationOffset;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Brightness">The depth the light will reach before having no effect</param>
        /// <param name="Position">The location the light is projected from</param>
        /// <param name="Direction">The direction the light will go in</param>
        /// <param name="Span">The amount of space the light will span, i.e. higher values -> wider area covered
        /// \n []</param>
        public Light(float Brightness, float Depth, Vector2 Position, Vector2 Direction, float Span)
        {
            this.Direction = Vector2.Normalize(Direction);
            this.Position = Position;
            this.Brightness = Brightness;
            this.Depth = Depth;
            this.Span = 2 * Span > MathHelper.TwoPi ? MathHelper.TwoPi : 2 * Span;
            this.RotationOffset = Span;
        }

        /*
        public void Update(Tile[,] Tiles, Vector2 Position, float Rotation)
        {
            this.Position = Position;
            
            //
            //The tile in the array that the light originates
            //
            int arrayPositionX = (int)(Position.X / 10) + 1;
            int arrayPositionY = (int)(Position.Y / 10) + 1;

            int radius = ((int)(Depth / 10f) + 1);

            int startX = arrayPositionX - radius;
            if (startX < 0)
                startX = 0;

            int endX = arrayPositionX + radius;
            if (endX >= Level.CurrentLevel.Width)
                endX = Level.CurrentLevel.Width - 1;

            int startY = arrayPositionY - radius;
            if (startY < 0)
                startY = 0;

            int endY = arrayPositionY + radius;
            if (endY >= Level.CurrentLevel.Height)
                endY = Level.CurrentLevel.Height - 1;

            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    float DistanceFromCentre = Vector2.Distance(Tiles[i, j].Position, this.Position);
                    if (DistanceFromCentre <= Depth)
                        Level.CurrentLevel.LightTile(i, j, (Brightness * (Depth / DistanceFromCentre)));
                }
            }
            
            
        }*/

        
        /// <summary>
        /// Calculates what tiles are affected by this light
        /// </summary>
        public void Update(Tile[,] Tiles, Vector2 Position, float Rotation)
        {
            //
            //The tile in the array that the light originates
            //
            int arrayPositionX = (int)(Position.X / 10);
            int arrayPositionY = (int)(Position.Y / 10);

            
            Rotation += RotationOffset;

            //for (int i = arrayPositionX; i < arrayPositionX + 1; i++)
            //{
            //    for (int j = arrayPositionY; j < arrayPositionY + 1; j++)
            //    {
            //        Level.CurrentLevel.LightTile(i, j, Brightness);
            //    }
            //}

            float AddedRotation = 0;
            for (int i = 0; AddedRotation + RotationSpeed < Span; i++)
            {
                float pctx = (float)Math.Sin(Rotation);
                float pcty = -(float)Math.Cos(Rotation);

                Vector2 RayVector = Vector2.Normalize(new Vector2(pctx, pcty));

                AddedRotation += RotationSpeed;
                Rotation -= RotationSpeed;

                Ray(Tiles, arrayPositionX, arrayPositionY, RayVector.X, RayVector.Y, Brightness);
            }
        }
        
        private void Ray(Tile[,] Tiles, int i, int j, float Vx, float Vy, float Brightness)
        {
            float Sx = i;
            float Sy = j;
            float light = 1f;

            while (light > 0)
            {
                Sx += Vx;
                Sy += Vy;

                if ((int)Sx < Tiles.GetLength(0) & (int)Sx >= 0 && (int)Sy < Tiles.GetLength(1) && (int)Sy >= 0)
                {
                    Level.CurrentLevel.LightTile((int)Sx, (int)Sy, light);
                    if (Tiles[(int)Sx, (int)Sy].Block != null)
                        return;
                }
                

                light -= 0.02f / Brightness;
            }
        }
    }
}
