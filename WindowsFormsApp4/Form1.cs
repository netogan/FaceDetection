using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Size = System.Drawing.Size;

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

            var _classifierFrontalFace = new CascadeClassifier(@"..\..\objetos\haarcascade_frontalface_default.xml");

            var _classifierEyes = new CascadeClassifier(@"..\..\objetos\haarcascade_eye.xml");
            var _classifierEyeLeft = new CascadeClassifier(@"..\..\objetos\eye_left18x12.xml");
            var _classifierEyeRight = new CascadeClassifier(@"..\..\objetos\eye_right18x12.xml");

            var _classifierMouth = new CascadeClassifier(@"..\..\objetos\haarcascade_mcs_mouth.xml");
            var _classifieNose = new CascadeClassifier(@"..\..\objetos\haarcascade_mcs_nose.xml");
            var _classifieNose25x15 = new CascadeClassifier(@"..\..\objetos\nariz_25x15.xml");

            var gray = ConverterParaImageGray(imagem, null);

            var facesDetected = _classifierFrontalFace.DetectMultiScale(gray, 1.01, 3, new Size(24, 24));

            var faceValida = new List<Rectangle>();

            foreach (var face in facesDetected)
            {
                var grayFace = ConverterParaImageGray(imagem, face);

                var eyesDetected = _classifierEyes.DetectMultiScale(grayFace, 1.01, 1);

                var bocasDetectadas = _classifierMouth.DetectMultiScale(grayFace, 1.01, 1);

                var narizesDetectados = new Rectangle[]{};

                narizesDetectados = _classifieNose.DetectMultiScale(grayFace, 1.01, 1, new Size(25, 25));

                if(narizesDetectados.Length == 0)
                    narizesDetectados = _classifieNose25x15.DetectMultiScale(grayFace, 1.01, 1, new Size(25, 25));

                if (eyesDetected.Length > 0 && bocasDetectadas.Length > 0 && narizesDetectados.Length > 0)
                    faceValida.Add(face);
            }

            if (faceValida.Count == 1)
            {
                var grayFaceValida = ConverterParaImageGray(imagem, faceValida[0]);

                var eyeEsquerdo = _classifierEyeLeft.DetectMultiScale(grayFaceValida, 1.01, 2, new Size(25, 15));
                var eyeDireito = _classifierEyeRight.DetectMultiScale(grayFaceValida, 1.01, 2, new Size(25, 15));

                if (eyeEsquerdo.Length > 0 && eyeDireito.Length > 0)
                {
                    var imgFace = imagem.Copy();

                    var recFace = faceValida[0];

                    recFace.Inflate(65, 65);

                    imgFace.ROI = recFace;

                    pictureBox2.Image = imgFace.Bitmap;

                    imgFace.Bitmap.Save($@"E:\foto-{Guid.NewGuid()}.jpeg", ImageFormat.Jpeg);
                }
            }
            
            pictureBox1.Image = imagem.Bitmap;
        }

        private Image<Gray, byte> ConverterParaImageGray(Image<Bgr, byte> imagemOriginal, Rectangle? regiao)
        {
            var novaImagem = new Image<Gray, byte>(new Size(0, 0));

            novaImagem = regiao != null ? imagemOriginal.Copy(regiao.GetValueOrDefault()).Convert<Gray, byte>() : imagemOriginal.Convert<Gray, byte>();

            novaImagem._EqualizeHist();

            return novaImagem;
        }
    }
}
