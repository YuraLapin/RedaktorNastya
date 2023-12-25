using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RedaktorMain
{
    internal class KeyPressHandler
    {
        public List<Line> userObjects;
        public List<int> selectedObjects;

        public KeyPressHandler(List<Line> userObjects, List<int> selectedObjectIndex)
        {
            this.userObjects = userObjects;
            this.selectedObjects = selectedObjectIndex;
        }

        public void XKeyPressed()
        {
            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    line.X1 = -line.X1;
                    line.X2 = -line.X2;                    
                }

                ++currentIndex;
            }
        }

        public void YKeyPressed()
        {
            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    line.Y1 = -line.Y1;
                    line.Y2 = -line.Y2;
                }

                ++currentIndex;
            }
        }

        public void ZKeyPressed()
        {
            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    line.Z1 = -line.Z1;
                    line.Z2 = -line.Z2;
                }

                ++currentIndex;
            }
        }

        public void TextFieldType(TextField tf, Keyboard.Key key)
        {
            if (key == Keyboard.Key.Backspace && tf.text.Count() > 0) 
            {
                tf.text = tf.text.Remove(tf.text.Count() - 1);
                return;    
            }

            string abc = tf.alphabet;
            if (tf.text == "")
            {
                abc += '-';
            }
            else 
            if (!tf.text.Contains('.'))
            {
                abc += '.';
            }

            string keyString = key.ToString();
            if (UserInterface.keyCodes.Keys.Contains(keyString))
            {
                string toAdd = UserInterface.keyCodes[keyString];
                if (abc.Contains(toAdd) && tf.text.Count() < tf.frameWidth / 10)
                {
                    tf.text += toAdd;
                }
            }
        }
    }
}
