using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace FaceDetectionHaar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();

            if (ofd.ShowDialog() != DialogResult.OK) return;

            var imagem = new Image<Bgr, byte>(ofd.FileName);

            var haarFace = new HaarCascade(@"..\..\haarcascade_frontalface_default.xml");

            var haarEye = new HaarCascade(@"..\..\frontalEyes35x16.xml");

            var haarMouth = new HaarCascade(@"..\..\haarcascade_mcs_mouth.xml");

            var haarNose = new HaarCascade(@"..\..\haarcascade_mcs_nose.xml");

            var gray = imagem.Convert<Gray, byte>();

            gray._EqualizeHist();

            var facesDetected = gray.DetectHaarCascade(haarFace, 1.05, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(24, 24))[0];

            foreach (var face in facesDetected)
            {
                gray.ROI = face.rect;

                var eyesDetected = gray.DetectHaarCascade(haarEye, 1.05, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(45, 11))[0];

                var haarLeftEye = new HaarCascade(@"..\..\haarcascade_mcs_lefteye.xml");
                var haarRigthEye = new HaarCascade(@"..\..\haarcascade_mcs_righteye.xml");

                var eyeLeftDetected = gray.DetectHaarCascade(haarLeftEye, 1.2, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(22, 5))[0];
                var eyeRightDetected = gray.DetectHaarCascade(haarRigthEye, 1.2, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(22, 5))[0];

                if(eyesDetected.Length == 0 && (eyeRightDetected.Length == 0 && eyeRightDetected.Length == 0)) continue;

                var mouthsDetected = gray.DetectHaarCascade(haarMouth, 1.2, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(25, 15))[0];

                var nosesDetected = gray.DetectHaarCascade(haarNose, 1.2, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(25, 15))[0];

                gray.ROI = Rectangle.Empty;

                var raio = (face.rect.Width + face.rect.Height) / (2 * 3.14159265359);

                if ((eyesDetected.Length > 0 || (eyeLeftDetected.Length > 0 && eyeRightDetected.Length > 0)) && (mouthsDetected.Length > 0 || nosesDetected.Length > 0))
                    //imagem.Draw(new CircleF(new PointF(face.rect.X * 2, face.rect.Y * 2 ), (float) raio) , new Bgr(Color.Purple), 3);
                    imagem.Draw(new CircleF(new Point(face.rect.Width * 2, face.rect.Height * 2), 20), new Bgr(Color.Brown), 1);

            }

            pictureBox1.Image = imagem.Bitmap;
        }
    }
}
