using SFML.Graphics;
using SFML.System;
using System.Globalization;

namespace RedaktorMain
{
    internal class TextField: IDrawable
    {
        public float x;
        public float y;

        public float frameWidth;
        public float frameHeight;

        public string text = "";
        public string def;
        public float defFloat;
        public SFML.Graphics.Font font = new SFML.Graphics.Font(UserInterface.FONT);
        public string alphabet = "1234567890";

        public bool active = false;

        private RectangleShape frame = new RectangleShape();

        public TextField()
        {
            x = 0;
            y = 0;
            frameWidth = 200;
            frameHeight = 100;
        }

        public TextField(float x, float y, float width, float height, string def, float defFloat)
        {
            this.x = x;
            this.y = y;
            this.frameWidth = width;
            this.frameHeight = height;
            this.def = def;
            this.defFloat = defFloat;
        }

        public virtual bool CheckClick(Vector2i position, uint width, uint height, float phi, float teta, float zc)
        {
            int mI = position.X;
            int mY = position.Y;
            var rectBounds = frame.GetGlobalBounds();
            return rectBounds.Contains(mI, mY);
        }

        public void Click()
        {
            active = true;
        }

        public float GetFloat()
        {
            float res;
            if (text != "")
            {
                res = float.Parse(text, CultureInfo.InvariantCulture.NumberFormat);
            }
            else
            {
                res = defFloat;
            }
            return res;
        }

        public void Draw(RenderWindow window, uint width, uint height, SFML.Graphics.Color color, float phi, float teta, float zc)
        {
            frame.FillColor = SFML.Graphics.Color.Black;
            frame.Size = new Vector2f(frameWidth, frameHeight);
            if (active)
            {
                frame.OutlineColor = SFML.Graphics.Color.Red;
            }
            else
            {
                frame.OutlineColor = SFML.Graphics.Color.White;
            }
            frame.OutlineThickness = 2;
            frame.Origin = new Vector2f(frameWidth / 2, frameHeight / 2);
            frame.Position = new Vector2f(x, y);

            string txt;
            if (text == "")
            {
                txt = def;
            }
            else
            {
                txt = text;
            }

            var textObject = new SFML.Graphics.Text(txt, font);
            textObject.FillColor = color;
            textObject.Scale = new Vector2f(0.6f, 0.6f);
            textObject.Origin = new Vector2f(textObject.GetLocalBounds().Width / 2, textObject.GetLocalBounds().Height);
            textObject.Position = new Vector2f(x, y);

            window.Draw(frame);
            window.Draw(textObject);
        }
    }
}
