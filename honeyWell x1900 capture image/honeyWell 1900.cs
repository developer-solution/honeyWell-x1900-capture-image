using System;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace honeyWell_x1900_capture_image
{
    public partial class mainFrm : Form
    {
        //المنفذ التسلسلي
        SerialPort _SerialPort;
        public mainFrm()
        {
            InitializeComponent();
            _SerialPort = new SerialPort();
        }

        private void mainFrm_Shown(object sender, EventArgs e)
        {
            //تعبئة المنافذ داخل ال ComboBox
            string[] ports = SerialPort.GetPortNames();
            cboPorts.Items.AddRange(ports);
        }

        //فتح المنفذ
        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            //في حال عدم إختيار أي منفذ يظهر رسالة خطأ
            if (cboPorts.SelectedIndex < 0)
            {
                MessageBox.Show("قم باختيار المنفذ وحاول مرة أخرى","فتح المنفذ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                //تعيين إعدادات المنفذ التسلسلي
                _SerialPort = new SerialPort(cboPorts.SelectedItem.ToString(), 9600, Parity.None, 8, StopBits.One);
                _SerialPort.Encoding = Encoding.Default;
                _SerialPort.DataReceived += new SerialDataReceivedEventHandler(_SerialPort_DataReceived);
                _SerialPort.Open();

                //إلغاء تفعيل زر فتح المنفذ وتفعيل زر إلتقاط صورة
                btnCapture.Enabled = _SerialPort.IsOpen;
                btnOpenPort.Enabled = !_SerialPort.IsOpen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الاتصال", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        //إلتقاط صورة
        private void btnCapture_Click(object sender, EventArgs e)
        {
            //إرسال أمر التصوير وجلب الصورة من القارئ
            _SerialPort.Write("\x16M\r" + imgCapture.hwPictureCmd);
        }

        //حدث إستقبال البيانات من المنفذ التسلسلي
        private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // في حال عدم وجود أي بيانات يخرج من العملية
            if (_SerialPort.BytesToRead <= 0)
                return;

            Thread.Sleep(150);

            string imgStr = _SerialPort.ReadExisting();//قراءة البيانات من المنفذ التسلسلي
            Image img = imgCapture.getImage(imgStr);//جلب الصورة

            if (img != null)
                pictureBox1.Image = img;//إظهار الصورة داخل المربع
        }

        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //عند الإغلاق في حال كان المنفذ مفتوح يقوم بإغلاقه
            if (_SerialPort.IsOpen)
                _SerialPort.Close();
        }
    }
}
