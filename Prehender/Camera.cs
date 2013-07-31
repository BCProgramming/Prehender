using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Prehender
{
    public class Camera
    {
        private float _xRotation;
        private float _yRotation;
        private float _zRotation;
        private float _Azimuth;
        private Vector3 _Location;
        private Vector3 _Target;
        private Vector3 _ActualLocation;
        private double _AspectRatio;
        private double _FOV = MathHelper.PiOver4;
        public float XRotation { get { return _xRotation; } set { _xRotation = value; } }
        public float YRotation { get { return _yRotation; } set { _yRotation = value; } }
        public float ZRotation { get { return _zRotation; } set { _zRotation = value; } }
        private Matrix4 _Projection, _Model;
        public Matrix4 Projection { get { return _Projection; } }
        public Matrix4 Model { get { return _Model; } }
        public Vector3 Location { get { return _Location; } set { _Location = value; } }
        public Vector3 ActualLocation { get { return _ActualLocation; } set { _ActualLocation = value; } }
        public Vector3 Target { get { return _Target; } set { _Target = value; } }
        public double AspectRatio { get { return _AspectRatio; } set { _AspectRatio = value; } }
        public double FOV { get { return _FOV; } set { _FOV = value; } }
        private Vector3 _Up = new Vector3(0, 1, 0);
        public void ZoomIn(float Amount)
        {
            _FOV -= Amount;
        }
        public void ZoomOut(float Amount)
        {
            _FOV += Amount;
        }
        public Camera(Vector3 pLocation, Vector3 pTarget)
        {
            _Location = pLocation;
            _Target = pTarget;
        }

       
        public void Apply()
        {
            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)_FOV, (float)_AspectRatio, 0.1f, 640);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
            _Projection = perspective;


            Matrix4 lookat = Matrix4.LookAt(_Location.X, _Location.Y, _Location.Z, _Target.X, _Target.Y, _Target.Z, _Up.X, _Up.Y, _Up.Z);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
            Vector3 tmp = Vector3.Zero;
            _ActualLocation = Vector3.TransformVector(tmp,lookat);
            _Model = lookat;
            GL.Rotate(_xRotation, 1.0f, 0, 0);
            
            GL.Rotate(_yRotation, 0.0f, 1.0f, 0.0f);
            GL.Rotate(_zRotation, 0.0f, 0.0f, 1.0f);
            
        }
    }
}
