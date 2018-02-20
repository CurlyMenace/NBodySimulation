using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NBodySim2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Points> points;
        Random random = new Random();
        public static int N = 1000;
        public Body[] bodies = new Body[N];
        public bool Run = false;
        Quad quad = new Quad(0, 0, 2 * 1e18);
        BHTree tree;

        //DispatcherTimer uiTimer;
        public MainWindow()
        {
            InitializeComponent();
            /*uiTimer = new DispatcherTimer();
            uiTimer.Tick += new EventHandler(uiTimerTick);
            uiTimer.Interval = new TimeSpan(0, 0, 0, 0, 25);
            uiTimer.Start();*/
            run();

        }

        private void uiTimerTick(object sender, EventArgs e)
        {
            if (Run)
            {
                //paint();
                //run();
            }
        }

        private void run()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            startTheBodies(N);
            for (int i = 0; i < 10; i++)
            {
                tree = new BHTree(quad);
                startThreads(N);
            }
            stopwatch.Stop();

            //MessageBox.Show(stopwatch.ElapsedMilliseconds.ToString());
        }
        public void paint()
        {
            Points point;
            points = new ObservableCollection<Points>();
            if (Run)
            {
                foreach (Body body in bodies)
                {
                    if (body != null)
                    {
                        point = new Points();
                        point.clr = body.color;
                        point.X = (int)Math.Round(body.posX * 1920 / 1e18);
                        point.Y = (int)Math.Round(body.posY * 1080 / 1e18);
                        points.Add(point);
                    }
                }
                HereComesTheArt.ItemsSource = points;
                startThreads(N);
            }
        }
        public static double circlev(double posX, double posY)
        {
            double solarmass = 1.98892e30;
            double r2 = Math.Sqrt(posX * posX + posY * posY);
            double numerator = (6.67e-11) * 1e6 * solarmass;

            return Math.Sqrt(numerator / r2);
        }

        public void startTheBodies(int N)
        {
            Run = true;
            double solarmass = 1.98892e30;

            for (int i = 0; i < N; i++)
            {
                double px = 1e18 * exp(-1.8) * (.5 - random.NextDouble());
                double py = 1e18 * exp(-1.8) * (.5 - random.NextDouble());
                double magv = circlev(px, py);

                double absangle = Math.Atan(Math.Abs(py / px));
                double thetav = Math.PI / 2 - absangle;
                double phiv = random.NextDouble() * Math.PI;
                double vx = -1 * Math.Sign(py) * Math.Cos(thetav) * magv;
                double vy = Math.Sign(px) * Math.Sin(thetav) * magv;

                if (random.NextDouble() <= .5)
                {
                    vx = -vx;
                    vy = -vy;
                }

                double mass = random.NextDouble() * solarmass * 10 + 1e20;

                int red = (int)Math.Floor(mass * 254 / (solarmass * 10 + 1e20));
                int blue = (int)Math.Floor(mass * 254 / (solarmass * 10 + 1e20));
                int green = 255;
                Color color = Color.FromArgb(0, Convert.ToByte(red), Convert.ToByte(blue), Convert.ToByte(green));
                SolidColorBrush clrB = new SolidColorBrush(color);
                bodies[i] = new Body(px, py, vx, vy, mass, clrB);
            }
            SolidColorBrush clr = new SolidColorBrush(Colors.Red);
            bodies[0] = new Body(0, 0, 0, 0, 1e6 * solarmass, clr);

        }

        public void startThreads(int N)
        {
            int numOThreads = Environment.ProcessorCount;
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < numOThreads; i++)
            {
                int start = (N / numOThreads) * i;
                int end = (N / numOThreads) + start - 1;
                Thread thread = new Thread(() => addForce(start, end), 10000);
                thread.Name = "T" + i;
                thread.Start();
                threads.Add(thread);
            }

            foreach (Thread T in threads)
            {
                T.Join();
            }


        }
        public void addForce(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if (bodies[i].isIn(quad))
                {
                    lock (tree)
                    {
                        tree.insert(bodies[i]);
                    }

                }
            }
            for (int i = start; i < end; i++)
            {

                bodies[i].resetForce();
                if (bodies[i].isIn(quad))
                {
                    lock (tree)
                    {
                        tree.updateForce(bodies[i]);
                    }
                    bodies[i].Update(1e11);
                }

            }


        }


        public static double exp(double lambda)
        {
            Random rng = new Random();

            //random between 0 and 1
            return -Math.Log(1 - rng.NextDouble()) / lambda;
        }
        private void RestartBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Run = false;
                N = int.Parse(NumOItemsTBox.Text);
                bodies = new Body[N];
                Run = true;
                run();
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Run = false;
        }
    }
}
