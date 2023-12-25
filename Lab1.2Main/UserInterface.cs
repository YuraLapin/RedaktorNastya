using SFML.Graphics;
using SFML.Window;
using System.Numerics;
using System.Text.Json;

namespace RedaktorMain
{
    public delegate void ClickHandler();

    public static class UserInterface
    {
        public static List<Line> userObjects = new List<Line>();
        public static Dictionary<string, IDrawable> ui = new Dictionary<string, IDrawable>()
        {
            { "LMB Prompt", new Label(10, 430, "ЛКМ - Выделить объект") },
            { "Ctrl Prompt", new Label(10, 450, "Ctrl + ЛКМ - Выделить несколько объектов") },
            { "D prompt", new Label(10, 490, "D - удалить выделенные объекты") },

            { "Draw button", new Button(60, 50, 100, 80, "Нарисовать", DrawLine) },
            { "draw x1", new TextField(170, 20, 100, 20, "X1", 0.0f) },
            { "draw y1", new TextField(170, 50, 100, 20, "Y1", 0.0f) },
            { "draw z1", new TextField(170, 80, 100, 20, "Z1", 0.0f) },
            { "draw x2", new TextField(280, 20, 100, 20, "X2", 0.0f) },
            { "draw y2", new TextField(280, 50, 100, 20, "Y2", 0.0f) },
            { "draw z2", new TextField(280, 80, 100, 20, "Z2", 0.0f) },

            { "Transition button", new Button(60, 140, 100, 80, "Переместить", Transit) },
            { "trans x", new TextField(225, 110, 210, 20, "X", 0.0f) },
            { "trans y", new TextField(225, 140, 210, 20, "Y", 0.0f) },
            { "trans z", new TextField(225, 170, 210, 20, "Z", 0.0f) },

            { "Rotation button X", new Button(60, 200, 100, 20, "Повернуть X", RotateX) },
            { "Rotation button Y", new Button(60, 230, 100, 20, "Повернуть Y", RotateY) },
            { "Rotation button Z", new Button(60, 260, 100, 20, "Повернуть Z", RotateZ) },
            { "rotate x", new TextField(225, 200, 210, 20, "X", 0.0f) },
            { "rotate y", new TextField(225, 230, 210, 20, "Y", 0.0f) },
            { "rotate z", new TextField(225, 260, 210, 20, "Z", 0.0f) },

            { "Scaling button", new Button(60, 320, 100, 80, "Масштаб", Scale) },
            { "scale x", new TextField(225, 290, 210, 20, "X", 1.0f) },
            { "scale y", new TextField(225, 320, 210, 20, "Y", 1.0f) },
            { "scale z", new TextField(225, 350, 210, 20, "Z", 1.0f) },

            { "X prompt", new Label(10, 510, "X - отразить по X") },
            { "Y prompt", new Label(10, 530, "Y - отразить по Y") },
            { "Z prompt", new Label(10, 550, "Z - отразить по Z") },

            { "finish prompt", new Label(10, 470, "ЛКМ - закончить начатое изменение") },

            { "Save button", new Button(170, 380, 320, 20, "Сохранить", Save) },
            { "Load button", new Button(170, 410, 320, 20, "Загрузить", Load) },

            { "oX", new Line(-WINDOW_WIDTH / 2, 0, 0, WINDOW_WIDTH / 2, 0, 0) },
            { "oY", new Line(0, -WINDOW_HEIGHT / 2, 0, 0, WINDOW_HEIGHT / 2, 0) },
            { "oZ", new Line(0, 0, -500, 0, 0, 500) },
        };

        public static Dictionary<string, string> keyCodes = new Dictionary<string, string>()
        {
            {"Num1", "1" },
            {"Num2", "2" },
            {"Num3", "3" },
            {"Num4", "4" },
            {"Num5", "5" },
            {"Num6", "6" },
            {"Num7", "7" },
            {"Num8", "8" },
            {"Num9", "9" },
            {"Num0", "0" },
            {"Dash", "-" },
            {"Period", "." },
        };

        public const float RADTODEG = 180 / MathF.PI;

        public const uint WINDOW_WIDTH = 1100;
        public const uint WINDOW_HEIGHT = 580;
        public const string WINDOW_TITLE = "Графический редактор";
        public const string FONT = "calibri.ttf";

        public static RenderWindow MAIN_WINDOW = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), WINDOW_TITLE, Styles.Close);

        public static bool controlPressed = false;
        public static bool rmbPressed = false;

        public static List<int> selectedObjects = new List<int>();
        public static float startX;
        public static float startY;

        public static float phi = -30;
        public static float teta = 45;
        public static float zc = 1000;

        static MainLoopHandler mainLoopHandler = new MainLoopHandler(MAIN_WINDOW, userObjects, RADTODEG, selectedObjects);
        static KeyPressHandler keyPressHandler = new KeyPressHandler(userObjects, selectedObjects);
        static MousePressHandler mousePressHandler = new MousePressHandler(userObjects, selectedObjects, ui);

        public static void Draw(RenderWindow window)
        {
            window.SetActive(true);
            window.Clear(SFML.Graphics.Color.Black);

            foreach (KeyValuePair<string, IDrawable> uiElement in ui)
            {
                uiElement.Value.Draw(window, WINDOW_WIDTH, WINDOW_HEIGHT, SFML.Graphics.Color.White, phi, teta, zc);
            }

            var curIndex = 0;
            foreach (IDrawable userObject in userObjects)
            {
                if (selectedObjects.Contains(curIndex))
                {
                    userObject.Draw(window, WINDOW_WIDTH, WINDOW_HEIGHT, SFML.Graphics.Color.Red, phi, teta, zc);
                }
                else
                {
                    userObject.Draw(window, WINDOW_WIDTH, WINDOW_HEIGHT, SFML.Graphics.Color.White, phi, teta, zc);
                }

                curIndex++;
            }

            window.Display();
        }

        public static void Close(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var window = (RenderWindow)sender;
                window.Close();
            }
        }

        public static void MousePressed(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var window = (RenderWindow)sender;
                var position = Mouse.GetPosition(window);

                if (((MouseButtonEventArgs)e).Button == Mouse.Button.Left)
                {
                    if (controlPressed)
                    {
                        mousePressHandler.ControlLMBPressed(position, phi, teta, zc);
                    }
                    else
                    {
                        mousePressHandler.LMBPressed(position, phi, teta, zc);
                    }
                }

                if (((MouseButtonEventArgs)e).Button == Mouse.Button.Right && !rmbPressed)
                {
                    mousePressHandler.RMBPressed(MAIN_WINDOW, ref startX, ref startY, ref rmbPressed);
                }
            }
        }

        public static void MouseWheelScrolled(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var d = ((MouseWheelScrollEventArgs)e).Delta;
                mousePressHandler.MouseWheelScroll(d, ref zc);
            }
        }

        public static void MouseReleased(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                if (((MouseButtonEventArgs)e).Button == Mouse.Button.Right && rmbPressed)
                {
                    rmbPressed = false;
                }
            }
        }

        public static void KeyPressed(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var window = (RenderWindow)sender;
                var key = ((SFML.Window.KeyEventArgs)e).Code;

                foreach (KeyValuePair<string, IDrawable> element in ui)
                {
                    if (element.Value is TextField tf)
                    {
                        if (tf.active)
                        {
                            keyPressHandler.TextFieldType(tf, key);
                            return;
                        }
                    }
                }


                if ((selectedObjects.Count > 0) && (key == Keyboard.Key.D) && !rmbPressed)
                {
                    DeleteSelectedObjects();
                    return;
                }

                if (key == Keyboard.Key.LControl)
                {
                    controlPressed = true;
                    return;
                }

                if (key == Keyboard.Key.X && !rmbPressed)
                {
                    keyPressHandler.XKeyPressed();
                    return;
                }

                if (key == Keyboard.Key.Y && !rmbPressed)
                {
                    keyPressHandler.YKeyPressed();
                    return;
                }

                if (key == Keyboard.Key.Z && !rmbPressed)
                {
                    keyPressHandler.ZKeyPressed();
                    return;
                }
            }
        }

        public static void KeyReleased(object? sender, EventArgs e)
        {
            if (((SFML.Window.KeyEventArgs)e).Code == Keyboard.Key.LControl)
            {
                controlPressed = false;
            }
        }

        public static void SubscribeEvents(RenderWindow window)
        {
            window.Closed += Close;
            window.MouseButtonPressed += MousePressed;
            window.MouseButtonReleased += MouseReleased;
            window.KeyPressed += KeyPressed;
            window.KeyReleased += KeyReleased;
            window.MouseWheelScrolled += MouseWheelScrolled;
        }

        public static void DrawLine()
        {
            float x1 = ((TextField)ui["draw x1"]).GetFloat();
            float y1 = ((TextField)ui["draw y1"]).GetFloat();
            float z1 = ((TextField)ui["draw z1"]).GetFloat();

            float x2 = ((TextField)ui["draw x2"]).GetFloat();
            float y2 = ((TextField)ui["draw y2"]).GetFloat();
            float z2 = ((TextField)ui["draw z2"]).GetFloat();

            if (selectedObjects.Count == 1)
            {
                ((Line)userObjects[selectedObjects[0]]).X1 = x1;
                ((Line)userObjects[selectedObjects[0]]).Y1 = y1;
                ((Line)userObjects[selectedObjects[0]]).Z1 = z1;

                ((Line)userObjects[selectedObjects[0]]).X2 = x2;
                ((Line)userObjects[selectedObjects[0]]).Y2 = y2;
                ((Line)userObjects[selectedObjects[0]]).Z2 = z2;

                return;
            }

            userObjects.Add(new Line(x1, y1, z1, x2, y2, z2));
        }

        public static void Transit()
        {
            var dX = ((TextField)ui["trans x"]).GetFloat();
            var dY = ((TextField)ui["trans y"]).GetFloat();
            var dZ = ((TextField)ui["trans z"]).GetFloat();

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    line.X1 += dX;
                    line.X2 += dX;
                    line.Y1 += dY;
                    line.Y2 += dY;
                    line.Z1 += dZ;
                    line.Z2 += dZ;
                }

                ++currentIndex;
            }
        }

        public static void RotateX()
        {
            var angle = ((TextField)ui["rotate x"]).GetFloat();
            angle /= RADTODEG;

            var line1 = new Vector4(1, 0, 0, 0);
            var line2 = new Vector4(0, MathF.Cos(angle), -MathF.Sin(angle), 0);
            var line3 = new Vector4(0, MathF.Sin(angle), MathF.Cos(angle), 0);
            var line4 = new Vector4(0, 0, 0, 1);
            var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    var oldCoords1 = new Vector4(line.X1, line.Y1, line.Z1, 1);
                    var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                    newCoords1.X /= newCoords1.W;
                    newCoords1.Y /= newCoords1.W;
                    line.X1 = newCoords1.X;
                    line.Y1 = newCoords1.Y;
                    line.Z1 = newCoords1.Z;

                    var oldCoords2 = new Vector4(line.X2, line.Y2, line.Z2, 1);
                    var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                    newCoords2.X /= newCoords2.W;
                    newCoords2.Y /= newCoords2.W;
                    line.X2 = newCoords2.X;
                    line.Y2 = newCoords2.Y;
                    line.Z2 = newCoords2.Z;
                }

                ++currentIndex;
            }
        }

        public static void RotateY()
        {
            var angle = ((TextField)ui["rotate y"]).GetFloat();
            angle /= RADTODEG;

            var line1 = new Vector4(MathF.Cos(angle), 0, MathF.Sin(angle), 0);
            var line2 = new Vector4(0, 1, 0, 0);
            var line3 = new Vector4(-MathF.Sin(angle), 0, MathF.Cos(angle), 0);
            var line4 = new Vector4(0, 0, 0, 1);
            var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    var oldCoords1 = new Vector4(line.X1, line.Y1, line.Z1, 1);
                    var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                    newCoords1.X /= newCoords1.W;
                    newCoords1.Y /= newCoords1.W;
                    line.X1 = newCoords1.X;
                    line.Y1 = newCoords1.Y;
                    line.Z1 = newCoords1.Z;

                    var oldCoords2 = new Vector4(line.X2, line.Y2, line.Z2, 1);
                    var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                    newCoords2.X /= newCoords2.W;
                    newCoords2.Y /= newCoords2.W;
                    line.X2 = newCoords2.X;
                    line.Y2 = newCoords2.Y;
                    line.Z2 = newCoords2.Z;
                }

                ++currentIndex;
            }
        }

        public static void RotateZ()
        {
            var angle = ((TextField)ui["rotate z"]).GetFloat();
            angle /= RADTODEG;

            var line1 = new Vector4(MathF.Cos(angle), -MathF.Sin(angle), 0, 0);
            var line2 = new Vector4(MathF.Sin(angle), MathF.Cos(angle), 0, 0);
            var line3 = new Vector4(0, 0, 1, 0);
            var line4 = new Vector4(0, 0, 0, 1);
            var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    var oldCoords1 = new Vector4(line.X1, line.Y1, line.Z1, 1);
                    var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                    newCoords1.X /= newCoords1.W;
                    newCoords1.Y /= newCoords1.W;
                    line.X1 = newCoords1.X;
                    line.Y1 = newCoords1.Y;
                    line.Z1 = newCoords1.Z;

                    var oldCoords2 = new Vector4(line.X2, line.Y2, line.Z2, 1);
                    var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                    newCoords2.X /= newCoords2.W;
                    newCoords2.Y /= newCoords2.W;
                    line.X2 = newCoords2.X;
                    line.Y2 = newCoords2.Y;
                    line.Z2 = newCoords2.Z;
                }

                ++currentIndex;
            }
        }

        public static void Scale()
        {
            var sX = ((TextField)ui["scale x"]).GetFloat();
            var sY = ((TextField)ui["scale y"]).GetFloat();
            var sZ = ((TextField)ui["scale z"]).GetFloat();

            var line1 = new Vector4(sX, 0, 0, 0);
            var line2 = new Vector4(0, sY, 0, 0);
            var line3 = new Vector4(0, 0, sZ, 0);
            var line4 = new Vector4(0, 0, 0, 1);
            var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    var oldCoords1 = new Vector4(line.X1, line.Y1, line.Z1, 1);
                    var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                    newCoords1.X /= newCoords1.W;
                    newCoords1.Y /= newCoords1.W;
                    line.X1 = newCoords1.X;
                    line.Y1 = newCoords1.Y;
                    line.Z1 = newCoords1.Z;

                    var oldCoords2 = new Vector4(line.X2, line.Y2, line.Z2, 1);
                    var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                    newCoords2.X /= newCoords2.W;
                    newCoords2.Y /= newCoords2.W;
                    line.X2 = newCoords2.X;
                    line.Y2 = newCoords2.Y;
                    line.Z2 = newCoords2.Z;
                }

                ++currentIndex;
            }
        }

        public static void DeleteSelectedObjects()
        {
            selectedObjects.Sort();

            foreach (int i in selectedObjects.FastReverse())
            {
                userObjects.RemoveAt(i);
            }

            selectedObjects.Clear();
        }

        public static void Save()
        {
            Stream stream;
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "json files (*.json)|*.json";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    JsonSerializer.Serialize(stream, userObjects, options);
                    stream.Dispose();
                    stream.Close();
                }
            }
        }

        public static void Load()
        {
            Stream stream;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "json files (*.json)|*.json";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = openFileDialog.OpenFile()) != null)
                {
                    List<Line>? newList = JsonSerializer.Deserialize<List<Line>>(stream);
                    selectedObjects.Clear();

                    if (newList == null)
                    {
                        userObjects = new List<Line>();
                        return;
                    }

                    userObjects.Clear();

                    foreach(Line l in newList)
                    {
                        userObjects.Add(new Line(l.X1, l.Y1, l.Z1, l.X2, l.Y2, l.Z2));
                    }

                    stream.Dispose();
                    stream.Close();
                }
            }
        }

        public static void DrawHouse()
        {
            userObjects.Add(new Line(0, 0, 0, 100, 0, 0));
            userObjects.Add(new Line(100, 0, 0, 100, 0, 100));
            userObjects.Add(new Line(100, 0, 100, 0, 0, 100));
            userObjects.Add(new Line(0, 0, 100, 0, 0, 0));

            userObjects.Add(new Line(0, 100, 0, 100, 100, 0));
            userObjects.Add(new Line(100, 100, 0, 100, 100, 100));
            userObjects.Add(new Line(100, 100, 100, 0, 100, 100));
            userObjects.Add(new Line(0, 100, 100, 0, 100, 0));

            userObjects.Add(new Line(0, 0, 0, 0, 100, 0));
            userObjects.Add(new Line(100, 0, 0, 100, 100, 0));
            userObjects.Add(new Line(100, 0, 100, 100, 100, 100));
            userObjects.Add(new Line(0, 0, 100, 0, 100, 100));

            //userObjects.Add(new Line(33, 0, 100, 33, 80, 100));
            //userObjects.Add(new Line(67, 0, 100, 67, 80, 100));
            //userObjects.Add(new Line(33, 80, 100, 67, 80, 100));

            userObjects.Add(new Line(100, 33, 33, 100, 33, 67));
            userObjects.Add(new Line(100, 67, 33, 100, 67, 67));
            userObjects.Add(new Line(100, 33, 33, 100, 67, 33));
            userObjects.Add(new Line(100, 33, 67, 100, 67, 67));

            userObjects.Add(new Line(33, 33, 100, 33, 67, 100));
            userObjects.Add(new Line(67, 33, 100, 67, 67, 100));
            userObjects.Add(new Line(33, 33, 100, 67, 33, 100));
            userObjects.Add(new Line(33, 67, 100, 67, 67, 100));

            userObjects.Add(new Line(0, 33, 33, 0, 33, 67));
            userObjects.Add(new Line(0, 67, 33, 0, 67, 67));
            userObjects.Add(new Line(0, 33, 33, 0, 67, 33));
            userObjects.Add(new Line(0, 33, 67, 0, 67, 67));

            userObjects.Add(new Line(33, 33, 0, 33, 67, 0));
            userObjects.Add(new Line(67, 33, 0, 67, 67, 0));
            userObjects.Add(new Line(33, 33, 0, 67, 33, 0));
            userObjects.Add(new Line(33, 67, 0, 67, 67, 0));

            userObjects.Add(new Line(0, 100, 0, 50, 150, 0));
            userObjects.Add(new Line(100, 100, 0, 50, 150, 0));
            userObjects.Add(new Line(0, 100, 100, 50, 150, 100));
            userObjects.Add(new Line(100, 100, 100, 50, 150, 100));
            userObjects.Add(new Line(50, 150, 0, 50, 150, 100));
        }

        public static void Start()
        {
            SubscribeEvents(MAIN_WINDOW);
            Draw(MAIN_WINDOW);
            DrawHouse();

            while (MAIN_WINDOW.IsOpen)
            { 
                MAIN_WINDOW.DispatchEvents();
                
                if (rmbPressed)
                {
                    mainLoopHandler.HandleCameraRotation(ref startX, ref startY, ref phi, ref teta);
                }

                Draw(MAIN_WINDOW);
            }
        }
    }
}