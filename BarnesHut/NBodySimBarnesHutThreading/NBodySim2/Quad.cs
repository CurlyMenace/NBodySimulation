using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NBodySim2
{
    public class Quad
    {
        private double xmid, ymid, length;

        public Quad(double _xmid, double _ymid, double _length)
        {
            xmid = _xmid;
            ymid = _ymid;
            length = _length;
        }

        public double getLength()
        {
            return length;
        }

        public bool contains(double _xmid, double _ymid)
        {
            if (_xmid <= xmid + length / 2.0 && _xmid >= xmid - length / 2.0 && _ymid <= ymid + length / 2.0 && _ymid >= ymid - length / 2.0)
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        public Quad NorthWest()
        {
            Quad newQuad = new Quad(xmid - length / 4.0, ymid + length / 4.0, length / 2.0);
            return newQuad;
        }

        public Quad NorthEast()
        {
            Quad newQuad = new Quad(xmid + length / 4.0, ymid + length / 4.0, length / 2.0);
            return newQuad;
        }

        public Quad SouthWest()
        {
            Quad newQuad = new Quad(xmid - length / 4.0, ymid - length / 4.0, length / 2.0);
            return newQuad;
        }

        public Quad SouthEast()
        {
            Quad newQuad = new Quad(xmid + length / 4.0, ymid - length / 4.0, length / 2.0);
            return newQuad;
        }
    }
}
