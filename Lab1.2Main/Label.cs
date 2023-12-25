using SFML.Graphics;
using SFML.System;


namespace RedaktorMain
{
    public class Label: IDrawable
    {
        public float X;
        public float Y;
        public string Txt;
        public SFML.Graphics.Font Font = new SFML.Graphics.Font(UserInterface.FONT);
        public SFML.Graphics.Color Color = SFML.Graphics.Color.Black;

        public Label(float x, float y, string txt)
        {
            X = x;
            Y = y;
            Txt = txt;
        }

        public virtual bool CheckClick(Vector2i position, uint width, uint height, float phi, float teta, float zc)
        {
            int mX = position.X;
            int mY = position.Y;

            var text = new SFML.Graphics.Text(Txt, Font);
            text.Scale = new Vector2f(0.6f, 0.6f);
            text.Position = new Vector2f(X, Y);

            var bounds = text.GetGlobalBounds();
            return bounds.Contains(mX, mY);
        }

        public void Click()
        {

        }

        public void Draw(RenderWindow window, uint width, uint height, SFML.Graphics.Color color, float phi, float teta, float zc)
        {
            var text = new SFML.Graphics.Text(Txt, Font);
            text.FillColor = color;
            text.Scale = new Vector2f(0.6f, 0.6f);
            text.Position = new Vector2f(X, Y);

            window.Draw(text);
        }
    }
}