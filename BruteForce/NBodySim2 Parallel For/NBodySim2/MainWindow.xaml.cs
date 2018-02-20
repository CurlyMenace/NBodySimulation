using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NBodySim2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Points> points;
        Random random = new Random();
        public static int N = 100;
        public Body[] bodies = new Body[N];
        public bool Run = false;

        //DispatcherTimer uiTimer;
        public MainWindow()
        {
            InitializeComponent();
            /*uiTimer = new DispatcherTimer();
            uiTimer.Tick += new EventHandler(uiTimerTick);
            uiTimer.Interval = new TimeSpan(0, 0, 0, 0, 25);
            uiTimer.Start();
            startTheBodies(N);*/
            runRun();
        }

        private async void runRun()
        {
            //As a default, the run method will be run asynchroniously
            //To ensure that it is not running on UI thread -
            //otherwise the program will break after 60 seconds
            //as the GUI is not loading
            await Task.Run(() => run());
        }
        private void uiTimerTick(object sender, EventArgs e)
        {
            if (Run)
            {
                paint();
                //run();
            }
        }

        private void run()
        {

            try
            {
                //Run test sizes from 100 to 5000
                for (int nSize = 0; nSize <= 5000; nSize += 1000)
                {
                    if (nSize == 0)
                    {
                        N = 100;
                    }

                    else
                    {
                        N = nSize;
                    }
                    bodies = new Body[N];
                    using (StreamWriter sw = new StreamWriter("resultsSize" + N + ".csv"))
                    {
                        //Run multiple tests
                        for (int tests = 0; tests < 10; tests++)
                        {
                            Stopwatch stopwatch = new Stopwatch();

                            stopwatch.Start();
                            startTheBodies(N);
                            //Run the add force 10 times
                            for (int i = 0; i < 10; i++)
                            {
                                addForce(N);
                            }
                            stopwatch.Stop();
                            sw.WriteLine(stopwatch.ElapsedMilliseconds);
                        }
                    }
                }
            }

            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        public void paint()
        {
            //Create a collection of points to render them if necessary
            Points point;
            points = new ObservableCollection<Points>();
            if (Run)
            {
                foreach (Body body in bodies)
                {
                    if (body != null)
                    {
                        point = new Points();
                        point.X = (int)Math.Round(body.posX * 1000 / 1e18);
                        point.Y = (int)Math.Round(body.posY * 1000 / 1e18);
                        points.Add(point);
                    }
                }
                HereComesTheArt.ItemsSource = points;
                addForce(N);
            }
        }
        public static double circlev(double posX, double posY)
        {
            //Some maths
            double solarmass = 1.98892e30;
            double r2 = Math.Sqrt(posX * posX + posY * posY);
            double numerator = (6.67e-11) * 1e6 * solarmass;

            return Math.Sqrt(numerator / r2);
        }

        public void startTheBodies(int N)
        {
            //Generates random bodies that are used in the simulation
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


                bodies[i] = new Body(px, py, vx, vy, mass);
            }
            Color clr = Colors.Red;
            bodies[0] = new Body(0, 0, 0, 0, 1e6 * solarmass);
        }

        public void addForce(int N)
        {
            //Used to add the force to each and every body based on other bodies

            //Splitting the work on either first, second or both forloops

            Parallel.For(0, N, i =>
            {
                bodies[i].resetForce();

                for(int j = 0; j < N; j++)
                {
                    if (i != j) bodies[i].addForce(bodies[j]);
                }
            });

            Parallel.For(0, N, i =>
            {

                bodies[i].Update(1e11);
            });
        }


        public static double exp(double lambda)
        {
            Random rng = new Random();

            //random between 0 and 1
            return -Math.Log(1 - rng.NextDouble()) / lambda;
        }
        private void RestartBtn_Click(object sender, RoutedEventArgs e)
        {
            N = int.Parse(NumOItemsTBox.Text);
            bodies = new Body[N];
            //run();
            startTheBodies(N);
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Run = false;
        }
    }
}
