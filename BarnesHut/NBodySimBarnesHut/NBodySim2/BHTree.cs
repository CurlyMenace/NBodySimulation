using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NBodySim2
{
    public class BHTree
    {
        private Body body;
        private Quad quad;
        private BHTree NorthWest;
        private BHTree NorthEast;
        private BHTree SouthWest;
        private BHTree SouthEast;

        public BHTree(Quad _quad)
        {
            quad = _quad;
            body = null;
            NorthWest = null;
            NorthEast = null;
            SouthWest = null;
            SouthEast = null;
        }

        public bool isExternal(BHTree _bstree)
        {
            if (_bstree.NorthWest == null && _bstree.NorthEast == null && _bstree.SouthWest == null && _bstree.SouthEast == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region FUCKINGOUCH
        public void insert(Body _body)
        {
            try
            {
                if (body == null)
                {
                    body = _body;
                }

                else if (isExternal(this) == false)
                {
                    body = _body.addBody(body);

                    Quad nw = quad.NorthWest();
                    if (_body.isIn(nw))
                    {
                        if (NorthWest == null)
                        {
                            NorthWest = new BHTree(nw);
                        }
                        NorthWest.insert(_body);
                    }
                    else
                    {
                        Quad ne = quad.NorthEast();
                        if (_body.isIn(ne))
                        {
                            if (NorthEast == null)
                            {
                                NorthEast = new BHTree(ne);
                            }
                            NorthEast.insert(_body);
                        }

                        else
                        {
                            Quad sw = quad.SouthWest();
                            if (_body.isIn(sw))
                            {
                                if (SouthWest == null)
                                {
                                    SouthWest = new BHTree(sw);
                                }
                                SouthWest.insert(_body);
                            }

                            else
                            {
                                Quad se = quad.SouthEast();
                                if (_body.isIn(se))
                                {
                                    if (SouthEast == null)
                                    {
                                        SouthEast = new BHTree(se);
                                    }
                                    SouthEast.insert(_body);
                                }
                            }
                        }
                    }
                }

                else if (isExternal(this))
                {
                    Body _bodyC = body;
                    Quad nw = quad.NorthWest();

                    if (_bodyC.isIn(nw))
                    {
                        if (NorthWest == null)
                        {
                            NorthWest = new BHTree(nw);
                        }
                        NorthWest.insert(_bodyC);
                    }
                    else
                    {
                        Quad ne = quad.NorthEast();
                        if (_bodyC.isIn(ne))
                        {
                            if (NorthEast == null)
                            {
                                NorthEast = new BHTree(ne);
                            }
                            NorthEast.insert(_bodyC);
                        }
                        else
                        {
                            Quad sw = quad.SouthWest();
                            if (_bodyC.isIn(sw))
                            {
                                if (SouthWest == null)
                                {
                                    SouthWest = new BHTree(sw);
                                }
                                SouthWest.insert(_bodyC);
                            }
                            else
                            {
                                Quad se = quad.SouthEast();
                                if (_bodyC.isIn(se))
                                {
                                    if (SouthEast == null)
                                    {
                                        SouthEast = new BHTree(se);
                                    }
                                    SouthEast.insert(_bodyC);
                                }
                            }
                        }

                    }
                    insert(_body);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        public void updateForce(Body _body)
        {
            if (isExternal(this))
            {
                if (body != _body)
                {
                    _body.addForce(body);
                }
            }
            else if (quad.getLength() / (body.calcDistance(_body)) < 2)
            {
                _body.addForce(body);
            }
            else
            {
                if (NorthWest != null) NorthWest.updateForce(_body);
                if (NorthEast != null) NorthEast.updateForce(_body);
                if (SouthWest != null) SouthWest.updateForce(_body);
                if (SouthEast != null) SouthEast.updateForce(_body);
            }
        }
    }
}
