using System;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;
using static PerfectRoom.Form1;
using static System.Windows.Forms.DataFormats;

namespace PerfectRoom
{
    public partial class Form1 : Form
    {
        Bitmap map;
        Random random = new Random();
        Vector3 cameraPosition;
        List<Polyhedron> polyhedrons = new();
        Vector3 upRight;
        Vector3 downRight;
        Vector3 downLeft;
        Vector3 upLeft;
        List<Light> lights = new();
        Material tempMaterial;

        int w;
        int h;

        public Form1()
        {
            InitializeComponent();
            w = pictureBox.Width;
            h = pictureBox.Height;
            map = new Bitmap(w, h);
            cameraPosition = new(0, 0, -10);
            polyhedrons.Add(Room(10, new(0, 0, 0)));
            polyhedrons.Add(Cube(2, new(3, 4, 2), new(10, 80, 150)));
            polyhedrons.Add(Cube(2, new(-2, 4, 1), new(90, 0, 70)));

            lights.Add(new(new(0, 0, 0), new(255, 255, 255)));
            ShowPolyhedrons();
        }

        private Polyhedron Cube(float side, Vector3 point, Vector3 color)
        {
            side /= 2;

            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-side, -side, -side);
            points[1] = new Vector3(side, -side, -side);
            points[2] = new Vector3(side, -side, side);
            points[3] = new Vector3(-side, -side, side);
            points[4] = new Vector3(-side, side, -side);
            points[5] = new Vector3(side, side, -side);
            points[6] = new Vector3(side, side, side);
            points[7] = new Vector3(-side, side, side);

            for (int i = 0; i < points.Length; i++)
                points[i] += point;

            Material material = new(0.5f, 0.1f, color);

            Polygon[] polygons = new Polygon[12];
            polygons[0] = new(new int[] { 0, 1, 2 }, new(material));
            polygons[1] = new(new int[] { 0, 2, 3 }, new(material));
            polygons[2] = new(new int[] { 0, 1, 5 }, new(material));
            polygons[3] = new(new int[] { 0, 5, 4 }, new(material));
            polygons[4] = new(new int[] { 0, 3, 7 }, new(material));
            polygons[5] = new(new int[] { 0, 7, 4 }, new(material));
            polygons[6] = new(new int[] { 1, 2, 6 }, new(material));
            polygons[7] = new(new int[] { 1, 6, 5 }, new(material));
            polygons[8] = new(new int[] { 2, 3, 7 }, new(material));
            polygons[9] = new(new int[] { 7, 2, 6 }, new(material));
            polygons[10] = new(new int[] { 5, 4, 6 }, new(material));
            polygons[11] = new(new int[] { 6, 4, 7 }, new(material));

            Polyhedron polyhedron = new(point, points, polygons);
            return polyhedron;
        }

        private Polyhedron Room(float side, Vector3 point)
        {
            side /= 2;

            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-side, -side, -side);
            points[1] = new Vector3(side, -side, -side);
            points[2] = new Vector3(side, -side, side);
            points[3] = new Vector3(-side, -side, side);
            points[4] = new Vector3(-side, side, -side);
            points[5] = new Vector3(side, side, -side);
            points[6] = new Vector3(side, side, side);
            points[7] = new Vector3(-side, side, side);

            upRight = points[5];
            downRight = points[1];
            downLeft = points[0];
            upLeft = points[4];

            Material material = new(0.1f, 0.8f, new(0, 0, 0));

            List<Polygon> polygons = new();
            //polygons.Add(new(new int[] { 0, 1, 5 }, new(material)));
            //polygons.Add(new(new int[] { 0, 5, 4 }, new(material)));
            material.color = new(0, 255, 0);
            polygons.Add(new(new int[] { 0, 2, 1 }, new(material)));
            polygons.Add(new(new int[] { 0, 3, 2 }, new(material)));
            material.color = new(0, 0, 255);
            polygons.Add(new(new int[] { 7, 0, 3 }, new(material)));
            polygons.Add(new(new int[] { 0, 4, 7 }, new(material)));
            material.color = new(255, 0, 0);
            polygons.Add(new(new int[] { 6, 1, 2 }, new(material)));
            polygons.Add(new(new int[] { 1, 6, 5 }, new(material)));
            material.color = new(255, 255, 0);
            polygons.Add(new(new int[] { 2, 3, 7 }, new(material)));
            polygons.Add(new(new int[] { 2, 7, 6 }, new(material)));
            material.color = new(0, 255, 255);
            polygons.Add(new(new int[] { 4, 5, 6 }, new(material)));
            polygons.Add(new(new int[] { 4, 6, 7 }, new(material)));

            Polyhedron polyhedron = new(point, points, polygons.ToArray());
            return polyhedron;
        }

        static private Vector3 Normalize(Vector3 v)
        {
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            if (length == 0)
                return v;
            return new Vector3(v.X / length, v.Y / length, v.Z / length);
        }

        public static float Scalar(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public Vector3[,] GetAllPixels()
        {
            var pixels = new Vector3[w, h];
            Vector3 stepUp = (upRight - upLeft) / (w - 1);
            Vector3 stepDown = (downRight - downLeft) / (w - 1);

            Vector3 up = new(upLeft.X, upLeft.Y, upLeft.Z);
            Vector3 down = new(downLeft.X, downLeft.Y, downLeft.Z);

            for (int i = 0; i < w; i++)
            {
                Vector3 stepY = (up - down) / (h - 1);
                Vector3 d = new(down.X, down.Y, down.Z);
                for (int j = 0; j < h; j++)
                {
                    pixels[i, j] = d;
                    d += stepY;
                }
                up += stepUp;
                down += stepDown;
            }
            return pixels;
        }

        private void ShowPolyhedrons()
        {
            var pixels = GetAllPixels();
            var colorPixels = new Color[w, h];
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    Ray r = new(cameraPosition, pixels[i, j]);
                    r.start = new Vector3(pixels[i, j].X, pixels[i, j].Y, pixels[i, j].Z);
                    Vector3 color = RayTrace(r, 4, 1);

                    if (color.X > 1.0f || color.Y > 1.0f || color.Z > 1.0f)
                        color = Normalize(color);
                    colorPixels[i, j] = Color.FromArgb((int)(255 * color.X), (int)(255 * color.Y), (int)(255 * color.Z));
                }
            }
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                    map.SetPixel(i, j, colorPixels[i, j]);
            }
            pictureBox.Image = map;
        }

        public static bool RayIntersectsTriangle(Ray ray, Vector3[] vectors, out float intersect)
        {
            intersect = 0;
            float EPS = 0.0001f;

            Vector3 edge1 = vectors[1] - vectors[0];
            Vector3 edge2 = vectors[2] - vectors[0];
            Vector3 pvec = Vector3.Cross(ray.direction, edge2);
            float det = Vector3.Dot(pvec, edge1);

            if (det > -EPS && det < EPS)
                return false;

            float inv_det = 1.0f / det;
            Vector3 tvec = ray.start - vectors[0];
            float u = inv_det * Vector3.Dot(tvec, pvec);

            if (u < 0 || u > 1)
                return false;

            Vector3 qvec = Vector3.Cross(tvec, edge1);
            float v = inv_det * Vector3.Dot(ray.direction, qvec);

            if (v < 0 || u + v > 1)
                return false;

            intersect = inv_det * Vector3.Dot(edge2, qvec);
            return true;
        }

        public static Vector3 NormalizeSide(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Normalize((p2 - p1) * (p3 - p1));
        }

        public bool FigureIntersection(Polyhedron polyhedron, Ray ray, out float intersect, out Vector3 normal)
        {
            intersect = -1;
            int polygonIndex = -1;
            normal = new();

            for (int i = 0; i < polyhedron.polygons.Length; i++)
            {
                int[] pointsIndexes = polyhedron.polygons[i].indexes;
                if (RayIntersectsTriangle(ray, new Vector3[3] {polyhedron.points[pointsIndexes[0]],
                    polyhedron.points[pointsIndexes[1]], polyhedron.points[pointsIndexes[2]] }, out float t) && (t < intersect || intersect == -1))
                {
                    intersect = t;
                    polygonIndex = i;
                }
            }

            if (polygonIndex != -1)
            {
                int[] pointsIndexes = polyhedron.polygons[polygonIndex].indexes;
                normal = NormalizeSide(polyhedron.points[pointsIndexes[0]],
                    polyhedron.points[pointsIndexes[1]], polyhedron.points[pointsIndexes[2]]);
                tempMaterial = new(polyhedron.polygons[polygonIndex].material);
                tempMaterial.color = polyhedron.polygons[polygonIndex].material.color;
                return true;
            }
            return false;
        }

        public bool IsVisible(Vector3 lightPosition, Vector3 hitPoint)
        {
            float max_t = (lightPosition - hitPoint).Length();
            Ray r = new(hitPoint, lightPosition);

            foreach (Polyhedron polyhedron in polyhedrons)
                if (FigureIntersection(polyhedron, r, out float t, out Vector3 n))
                    if (t < max_t && t > 0.0001f)
                        return false;
            return true;
        }

        public Vector3 RayTrace(Ray ray, int iter, float env)
        {
            float minPosition = -1;
            Vector3 normal = new(0);
            Material m = new();
            Vector3 color = new(0);

            if (iter <= 0)
                return new Vector3(0, 0, 0);

            foreach (var polyhedron in polyhedrons)
            {
                if (FigureIntersection(polyhedron, ray, out float intersect, out Vector3 n))
                {
                    if (intersect < minPosition || minPosition == -1)
                    {
                        minPosition = intersect;
                        normal = n;
                        m = new Material(tempMaterial);
                    }
                }
            }

            if (minPosition == -1)
                return color;

            if (Scalar(ray.direction, normal) > 0)
            {
                normal *= -1;
            }

            Vector3 hit_point = ray.start + ray.direction * minPosition;

            foreach (Light light in lights)
            {
                /*Vector3 amb = light.color * m.ambient;
                amb.X *= m.color.X;
                amb.Y *= m.color.Y;
                amb.Z *= m.color.Z;
                color = amb;*/

                if (IsVisible(light.position, hit_point))
                    color += light.Shade(hit_point, normal, m.color, m.diffuse);
            }

            return color;
        }

        public class Ray
        {
            public Vector3 start;
            public Vector3 direction;

            public Ray(Vector3 start, Vector3 end)
            {
                this.start = new(start.X, start.Y, start.Z);
                direction = Normalize(end - start);
            }

            public Ray() { }

            public Ray(Ray ray)
            {
                start = ray.start;
                direction = ray.direction;
            }
        }

        public class Light
        {
            public Vector3 position;
            public Vector3 color;

            public Light(Vector3 position, Vector3 color)
            {
                this.position = new Vector3(position.X, position.Y, position.Z);
                this.color = new Vector3(color.X, color.Y, color.Z);
            }

            public Vector3 Shade(Vector3 hitPoint, Vector3 normal, Vector3 colorObj, float diffuseCoef)
            {
                Vector3 dir = position - hitPoint;
                dir = Normalize(dir); // направление луча из источника света в точку удара

                Vector3 diff = diffuseCoef * color * Math.Max(Scalar(normal, dir), 0);
                return new Vector3(diff.X * colorObj.X, diff.Y * colorObj.Y, diff.Z * colorObj.Z);
            }
        }

        public class Material
        {
            public float ambient; // коэффициент принятия фонового освещения
            public float diffuse; // коэффициент принятия диффузного освещения
            public Vector3 color;

            public Material(float amb, float dif, Vector3 color)
            {
                ambient = amb;
                diffuse = dif;
                this.color = color;
            }

            public Material(Material m)
            {
                ambient = m.ambient;
                diffuse = m.diffuse;
                color = new(m.color.X, m.color.Y, m.color.Z);
            }

            public Material() { }
        }

        public class Polygon
        {
            public int[] indexes;
            public Material material;

            public Polygon(int[] indexes, Material material)
            {
                this.indexes = indexes;
                this.material = material;
            }
        }

        public class Polyhedron
        {
            public Vector3 point;
            public Vector3[] points;
            public Polygon[] polygons;

            public Polyhedron(Vector3 point, Vector3[] points, Polygon[] polygons)
            {
                this.point = point;
                this.points = points;
                this.polygons = polygons;
            }
        }
    }
}