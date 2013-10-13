using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public class UIElement
    {
        public bool IsActive { get; set; }
        public Vector2 OriginalPosition;
        public Vector2 Position;
        public Color Colour
        {
            get
            {
                return IsMouseOver && !IsHeld ? _highlightColour : _colour;
            }
        }
        private Color _colour;
        private Color _highlightColour;

        public bool IsHeld { get; set; }

        protected virtual Rectangle? SrcRect { get; set; }
        protected Texture2D Texture;

        protected int Width { get; set; }
        protected int Height { get; set; }

        public UIElement(Vector2 Position, Texture2D Texture, Color Colour, Color HighlightColour, Rectangle? SrcRect = null, int Width = -1, int Height = -1, bool IsActive = true)
        {
            this.Position = OriginalPosition = Position;
            this.Texture = Texture;
            this.SrcRect = SrcRect;
            this._colour = Colour;
            this._highlightColour = HighlightColour;

            if (Width == -1)
                this.Width = Texture.Width;
            else
                this.Width = Width;

            if (Height == -1)
                this.Height = Texture.Height;
            else
                this.Height = Height;

            this.IsActive = IsActive;
        }

        public virtual void DoubleClick() { }
        public virtual void JustLeftClicked() { }
        public virtual void JustLeftReleased(int ItemHeld, int ItemHoveringOver) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsMouseOver
        {
            get;
            set;
        }

        public virtual void Update()
        {
            //Important: Must be its original position, so that the mouse isn't counted as hovering over it while 
            //it's being dragged around by the mouse
            IsMouseOver = InputManager.MousePositionP.X > this.OriginalPosition.X
            && InputManager.MousePositionP.X < this.OriginalPosition.X + Width
            && InputManager.MousePositionP.Y > this.OriginalPosition.Y
            && InputManager.MousePositionP.Y < this.OriginalPosition.Y + Height;

            if (IsHeld)
                Position = new Vector2(InputManager.MousePositionP.X - 10, InputManager.MousePositionP.Y - 10);
            if (IsMouseOver)
            {
                if (InputManager.DoubleLeftClicked)
                    DoubleClick();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
                spriteBatch.Draw(Texture, Position, SrcRect, Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);
        }
    }

    public class InventoryElement : UIElement
    {
        protected Texture2D BackgroundTexture{get; set;}
        public Item item { get; set; }

        public InventoryElement(Vector2 Position, Texture2D Texture, Item item, Texture2D BackgroundTexture)
            : base(Position, Texture, Color.White, Color.Orange, null, BackgroundTexture.Width, BackgroundTexture.Height)
        {
            this.item = item;
            this.BackgroundTexture = BackgroundTexture;
        }

        protected override Rectangle? SrcRect
        {
            get { return Textures.GetItemRectangle(item.Type); }
        }

        public override void DoubleClick()
        {
            if (item != null)
                item.Activate();
        }

        public override void JustLeftClicked()
        {
            //The player is picking up an item
            if (item != null) //Can only pick up non null items
                IsHeld = true;
        }

        public override void JustLeftReleased(int ItemHeld, int ItemHoveringOver)
        {
            if (ItemHoveringOver != -1) //The item wasn't dropped into a place that it can go
            {
                //They swap places only if they are different
                if (ItemHoveringOver != ItemHeld)
                {
                    //The item is being dropped onto an item slot
                    Item temp = Level.CurrentLevel.Players[0].Inventory[ItemHoveringOver];

                    //Swap the items in the UI
                    UI.Inventory_UI[ItemHoveringOver].item = Level.CurrentLevel.Players[0].Inventory[ItemHeld];
                    this.item = temp;

                    //Swap the items in the inventory
                    Level.CurrentLevel.Players[0].Inventory[ItemHoveringOver] = Level.CurrentLevel.Players[0].Inventory[ItemHeld];
                    Level.CurrentLevel.Players[0].Inventory[ItemHeld] = temp;
                }
            }

            if (InputManager.MousePositionP.X < UI.ScreenX && InputManager.MousePositionP.X > 0
                && InputManager.MousePositionP.Y < UI.ScreenY && InputManager.MousePositionP.Y > 0)
            {
                //Dropping the item onto the floor
                Level.CurrentLevel.FloorItems.Add(new FloorItem(Level.CurrentLevel.Players[0].Inventory[ItemHeld], Level.CurrentLevel.Players[0].Position + Level.CurrentLevel.Players[0].Origin));

                Level.CurrentLevel.Players[0].Inventory[ItemHeld] = null;
                UI.Inventory_UI[ItemHeld].item = null;
            }

            IsHeld = false; //The player is no longer holding the item

            this.Position = this.OriginalPosition; //So the item is returned to its original position
        }

        /// <summary>
        /// Gets the colour the item should be displayed as. It should be darker if the gun is reloading
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private Color GetReloadColour(Gun gun, Color original)
        {
            Color temp = original;

            temp *= 1 - ((float)gun.ReloadCooldown / (gun.ReloadTime * 1.3f));
            //Restore the alpha
            temp.A = original.A;
            temp.B = 255;
            return temp;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(BackgroundTexture, OriginalPosition, null, Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);

                if (item != null)
                {
                    try
                    {
                        Color colour = Colour;
                        if (item is Gun)
                            colour = GetReloadColour((Gun)item, Colour);

                        if (Textures.Items != null)
                            spriteBatch.Draw(Textures.Items, Position + new Vector2(2, 2), SrcRect, colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
                        spriteBatch.DrawString(Textures.Fonts[0], item.Amount.ToString(), Position + new Vector2(4, 22), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
                    }
                    catch (NullReferenceException ex)
                    {
                        Exception e = ex;
                    }
                }
            }
        }
    }
}

