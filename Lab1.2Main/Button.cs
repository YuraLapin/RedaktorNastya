using SFML.Graphics;
using SFML.System;

namespace RedaktorMain
{
    public class Button: IDrawable
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public SFML.Graphics.Color Color;
        public string? Txt;
        public SFML.Graphics.Font Font = new SFML.Graphics.Font(UserInterface.FONT);
        public ClickHandler ClickFunction;

        public Button()
        {
            X = 100;
            Y = 50;
            Width = 100;
            Height = 50;
            Txt = "???";
            ClickFunction = () => { };
        }

        public Button(float x, float y, float width, float height, string txt, ClickHandler clickFunction)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Txt = txt;
            this.ClickFunction = clickFunction;
        }        

        public virtual bool CheckClick(Vector2i position, uint width, uint height, float phi, float teta, float zc)
        {
            int mI = position.X;
            int mY = position.Y;
            return (mI > X - Width / 2) && (mI < X + Width / 2) && (mY > Y - Height / 2) && (mY < Y + Height / 2);
        }

        public void Click()
        {
            ClickFunction();
        }

        public void Draw(RenderWindow window, uint width, uint height, SFML.Graphics.Color color, float phi, float teta, float zc)
        {
            var frame = new RectangleShape();
            frame.FillColor = SFML.Graphics.Color.Black;
            frame.Size = new Vector2f(Width, Height);
            frame.OutlineColor = color;
            frame.OutlineThickness = 2;
            frame.Origin = new Vector2f(Width / 2, Height / 2);
            frame.Position = new Vector2f(X, Y);

            var text = new Text(Txt, Font);
            text.FillColor = color;
            text.Scale = new Vector2f(0.6f, 0.6f);
            text.Origin = new Vector2f(text.GetLocalBounds().Width / 2, text.GetLocalBounds().Height);
            text.Position = new Vector2f(X, Y);

            window.Draw(frame);
            window.Draw(text);
        }        
    }
}