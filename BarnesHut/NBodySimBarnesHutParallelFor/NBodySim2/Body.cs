using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NBodySim2
{
    public class Body
    {
        private static readonly double GConst = 6.673e-11;
        private static readonly double Solarmass = 1.98892e30;

        public double posX, posY;           //position
        public double velX, velY;           //velocity
        public double forcX, forcY;         //force
        public double mass;
        public SolidColorBrush color;

        public Body(double _posX, double _posY, double _velX, double _velY, double _mass, SolidColorBrush _color)
        {
            posX = _posX;
            posY = _posY;
            velX = _velX;
            velY = _velY;
            mass = _mass;
            color = _color;
        }

        public void Update(double delT)
        {
            velX += delT * forcX / mass;
            velY += delT * forcY / mass;
            posX += delT * velX;
            posY += delT * velY;
        }

        public double calcDistance(Body body)
        {
            double distanceX = posX - body.posX;
            double distanceY = posY - body.posY;
            return Math.Sqrt(posX * posX + posY * posY);
        }

        public void resetForce()
        {
            forcX = 0.0;
            forcY = 0.0;
        }

        public void addForce(Body bodyB)
        {
            Body bodyA = this;
            double epsilon = 3E4; //Softening parameter yo
            double distanceX = bodyB.posX - bodyA.posX;
            double distanceY = bodyB.posY - bodyA.posY;
            double dist = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            double F = (GConst * bodyA.mass * bodyB.mass) / (dist * dist + epsilon * epsilon);
            bodyA.forcX += F * distanceX / dist;
            bodyA.forcY += F * distanceY / dist;
        }

        public Body addBody(Body _body)
        {
            Body bodyA = this;
            double m = mass + _body.mass;
            double X = (bodyA.posX * bodyA.mass + _body.posX * _body.mass) / m;
            double Y = (bodyA.posY * bodyA.mass + _body.posY * _body.mass) / m;

            return new Body(X, Y, bodyA.velX, _body.velX, m, bodyA.color);
        }

        public bool isIn(Quad q)
        {
            return q.contains(posX, posY);
        }
           
    }
}
