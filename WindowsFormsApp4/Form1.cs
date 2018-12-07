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

            //var memory

            var imagem = new Image<Bgr, byte>(ofd.FileName);

            var haarFace = new HaarCascade(@"..\..\haarcascade_frontalface_default.xml");

            var haarEye = new HaarCascade(@"..\..\haarcascade_eye.xml");

            var haarMouth = new HaarCascade(@"..\..\haarcascade_mcs_mouth.xml");

            var haarNose = new HaarCascade(@"..\..\haarcascade_mcs_nose.xml");

            var gray = imagem.Convert<Gray, byte>();

            gray._EqualizeHist();

            var facesDetected = gray.DetectHaarCascade(haarFace, 1.05, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20))[0];

            foreach (var face in facesDetected)
            {
                gray.ROI = face.rect;

                //var eyesDetected = gray.DetectHaarCascade(haarEye, 1.05, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20))[0];

                //if(eyesDetected.Length == 0) continue;

                var haarLeftEye = new HaarCascade(@"..\..\haarcascade_mcs_lefteye.xml");
                var haarRigthEye = new HaarCascade(@"..\..\haarcascade_mcs_righteye.xml");

                var eyeLeftDetected = gray.DetectHaarCascade(haarLeftEye, 1.2, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20))[0];
                var eyeRightDetected = gray.DetectHaarCascade(haarRigthEye, 1.2, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20))[0];

                if(eyeRightDetected.Length == 0 && eyeRightDetected.Length == 0) continue;

                var mouthsDetected = gray.DetectHaarCascade(haarMouth, 1.2, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20))[0];

                var nosesDetected = gray.DetectHaarCascade(haarNose, 1.2, 3, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20))[0];

                gray.ROI = Rectangle.Empty;

                if((eyeLeftDetected.Length > 0 && eyeRightDetected.Length > 0 ) && (mouthsDetected.Length > 0 || nosesDetected.Length > 0))
                    imagem.Draw(face.rect, new Bgr(Color.Purple), 4);
            }

            pictureBox1.Image = imagem.Bitmap;
        }
    }
}
