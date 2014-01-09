using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public abstract class Raycaster
    {
        public float Radius { get; set; }
        public bool IsActive { get; set; }

        public Vector2 Position { get; private set; }
        protected Vector2 Direction { get; set; }
        public float Span { get; private set; }
        protected Tile[,] _tiles; //A reference to the tiles in the current level

        protected int startX, startY, centreX, centreY, endX, endY;
        protected Vector2 currentRayDirection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Position">The location the light is projected from</param>
        /// <param name="Direction">The direction the light will go in</param>
        /// <param name="Span">The amount of space the light will span, i.e. higher values -> wider area covered
        /// \n []</param>
        public Raycaster(float Radius, Vector2 Position, Vector2 Direction, float Span, Tile[,] tiles, bool IsActive = true)
        {
            this.Direction = Vector2.Normalize(Direction);
            this.Position = Position;
            this.Radius = Radius;
            this.Span = Span;
            this._tiles = tiles;
            this.IsActive = IsActive;

            Update(Position, Direction);
        }

        /// <summary>
        /// Update the position and direction of the Raycaster
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="newDirection"></param>
        /// <param name="tiles"></param>
        public void Update(Vector2 newPosition, Vector2 newDirection)
        {
            if (!IsActive)
                return;

            this.Direction = Vector2.Normalize(newDirection);
            this.Position = newPosition;

            //
            //The upper left corner of the square that contains the circle projected by the light
            //
            startX = (int)((Position.X - Radius) / Tile.WIDTH);
            startY = (int)((Position.Y - Radius) / Tile.HEIGHT);

            //
            //The upper left corner of the square that contains the circle projected by the light
            //
            endX = (int)Math.Ceiling((Position.X + Radius) / Tile.WIDTH);
            endY = (int)Math.Ceiling((Position.Y + Radius) / Tile.HEIGHT);

            //
            //The coordinates of the light source itself
            //
            centreX = (int)(Position.X / 10);
            centreY = (int)(Position.Y / 10);
        }


        /// <summary>
        ///  Action to be performed upon a tile when hit by a ray
        /// </summary>
        /// <param name="tileX">The x coordinate of the tile</param>
        /// <param name="tileY">The y coordinate of the tile</param>
        /// <returns>False if a blocking tile is found. This terminates the raycast algorithm</returns>
        protected abstract bool OnNextTile(int tileX, int tileY);

        protected bool isWithinSpan(int tileX, int tileY)
        {
            //Get the offset from the tile to the centre of the raycaster
            //Note: The y coordinate is negated because the formula on the next line works with a coordinate system 
            //in which the y axis points down, whereas with XNA, the y axis points up 
            Vector2 voffset = new Vector2((tileX * Tile.WIDTH) - Position.X, Position.Y - (tileY * Tile.HEIGHT));

            //Calculate the angle between the direction and this offset
            double angle = Math.Acos(Vector2.Dot(voffset, Direction) / (voffset.Length() * Direction.Length()));

            //If the angle is small enough this tile is within the span of the light
            return (angle < Span);
        }

        public void CastOne(int srcX, int srcY, int destX, int destY)
        {
            rayCast(srcX, srcY, destX, destY);
        }

        /// <summary>
        /// Applies the raycast algorithm
        /// </summary>
        public void CastAll()
        {
            if (!IsActive)
                return;

            int srcX = (int)(Position.X / Tile.WIDTH);
            int srcY = (int)(Position.Y / Tile.HEIGHT);

            //
            //Use the raycast algorithm on the outer edges of the ray source
            //
            for (int i = startX; i < endX; i++)
            {
                if (isWithinSpan(i, startY))
                    rayCast(srcX, srcY, i, startY);
                if (isWithinSpan(i, endY - 1))
                    rayCast(srcX, srcY, i, endY - 1);
            }

            for (int j = startY + 1; j < endY - 1; j++)
            {
                if (isWithinSpan(startX, j))
                    rayCast(srcX, srcY, startX, j);

                if (isWithinSpan(endX - 1, j))
                    rayCast(srcX, srcY, endX - 1, j);
            }

            OnCasted();
        }

        /// <summary>
        /// The action to be performed when the raycasting algorithm has been completed
        /// </summary>
        protected abstract void OnCasted();

        /// <summary>
        /// Perform the raycast algorithm from one point to another
        /// </summary>
        /// <returns>False if the raycast terminated before reaching the destination</returns>
        protected bool rayCast(int srcX, int srcY, int destX, int destY)
        {
            currentRayDirection = new Vector2(destX - srcX, destY - srcY);
            //
            //Check each square from the source tile to the destination tile until a blocking square is found; using the Bresenham line algorithm
            //
            bool isSteep = Math.Abs(destY - srcY) > Math.Abs(destX - srcX);
            if (isSteep)
            {
                int temp = srcX;
                srcX = srcY;
                srcY = temp;

                temp = destX;
                destX = destY;
                destY = temp;
            }

            float deltaX = Math.Abs(destX - srcX);
            float deltaY = Math.Abs(destY - srcY);

            float error = deltaX / 2;
            int y = srcY;

            int xstep = srcX > destX ? -1 : 1;
            int ystep = srcY > destY ? -1 : 1;

            for (int x = srcX; x != destX; x += xstep)
            {
                int i;
                int j;
                if (isSteep)
                {
                    i = y;
                    j = x;
                }
                else
                {
                    i = x;
                    j = y;
                }

                //Perform the action that was a parameter of this method.
                //Return false if we want to terminate the raycast early
                if (!OnNextTile(i, j))
                    return false;

                error -= deltaY;

                if (error < 0)
                {
                    y += ystep;
                    error += deltaX;
                }
            }
            return true;
        }

    }

}