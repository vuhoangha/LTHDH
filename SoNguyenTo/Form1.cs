using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoNguyenTo
{
    public partial class Form1 : Form
    {
        public Dictionary<int, List<long>> LIST_PRIME;
        public int THREAD_SUCCESS;
        public int NUM_THREAD;
        public Stopwatch SW;

        public Form1()
        {
            InitializeComponent();
        }

        public void isAllThreadEnd()
        {
            THREAD_SUCCESS++;
            if (THREAD_SUCCESS == NUM_THREAD)
            {
                #region Gop cac list nho lai

                //totalList = totalList.OrderBy(p => p).ToList();
                List<long> totalList = new List<long>();
                for (int i = 0; i <= NUM_THREAD; i++)
                {
                    if (LIST_PRIME.ContainsKey(i))
                    {
                        totalList = totalList.Concat(LIST_PRIME[i]).ToList();
                    }
                }

                #endregion

                //SW.Stop();
                //MessageBox.Show(SW.ElapsedMilliseconds.ToString(), "", MessageBoxButtons.OK);

                string text = "count: " + totalList.Count + "\n\n";
                foreach (var item in totalList)
                {
                    text += " " + item;
                }

                System.IO.File.WriteAllText("output.txt", text);

                SW.Stop();
                MessageBox.Show(SW.ElapsedMilliseconds.ToString(), "", MessageBoxButtons.OK);

                btnCal.Invoke(new Action(delegate { btnCal.Enabled = true; }));
            }
        }

        public void init(int numThread)
        {
            THREAD_SUCCESS = 0;
            NUM_THREAD = numThread;
            LIST_PRIME = new Dictionary<int, List<long>>();
            for (int i = 0; i <= NUM_THREAD; i++)
            {
                LIST_PRIME[i] = new List<long>();
            }

            SW = Stopwatch.StartNew();
        }

        public void findPrime(List<long> listNum, int index)
        {
            if (LIST_PRIME.ContainsKey(index))
                foreach (var num in listNum)
                {
                    if (isPrime(num)) LIST_PRIME[index].Add(num);
                }
            isAllThreadEnd();
        }

        public bool isPrime(long num)
        {
            if (num == 2) return true;
            if (num == 1) return false;

            long limited = (long)Math.Sqrt(num);

            for (long i = 2; i <= limited; i++)
                if (num % i == 0) return false;
            return true;
        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            #region validate

            string stringNumStart = txtStart.Text;
            string stringNumEnd = txtEnd.Text;
            string stringNumThread = txtThread.Text;

            if (stringNumStart == "" || stringNumEnd == "" || stringNumThread == "")
            {
                MessageBox.Show("Bạn bắt buộc phải nhập đủ thông tin", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            long numStart;
            if (long.TryParse(stringNumStart, out numStart) == false)
            {
                MessageBox.Show("Số nhập vào chưa đúng định dạng", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            long numEnd;
            if (long.TryParse(stringNumEnd, out numEnd) == false)
            {
                MessageBox.Show("Số nhập vào chưa đúng định dạng", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int numThread;
            if (int.TryParse(stringNumThread, out numThread) == false)
            {
                MessageBox.Show("Số nhập vào chưa đúng định dạng", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numStart > numEnd)
            {
                MessageBox.Show("Số bắt đầu phải nhỏ hơn hoặc bằng số kết thúc", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numStart < 1)
            {
                MessageBox.Show("Số bắt đầu phải là số nguyên dương", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #endregion

            #region Set trang thai button

            btnCal.Enabled = false;

            #endregion

            #region Tinh toan

            init(numThread);

            #region Them cac so dac biet

            if (numStart <= 2 && 2 <= numEnd) LIST_PRIME[NUM_THREAD].Add(2);
            if (numStart <= 3 && 3 <= numEnd) LIST_PRIME[NUM_THREAD].Add(3);
            if (numStart <= 5 && 5 <= numEnd) LIST_PRIME[NUM_THREAD].Add(5);
            if (numStart <= 7 && 7 <= numEnd) LIST_PRIME[NUM_THREAD].Add(7);

            #endregion

            #region Phan chia cac so vao cac thread

            Dictionary<int, List<long>> dicGroup = new Dictionary<int, List<long>>();
            for (int i = 0; i < NUM_THREAD; i++)
            {
                dicGroup[i] = new List<long>();
            }

            int sodu = 0;
            for (long i = numStart; i <= numEnd; i++)
            {
                if (i % 6 != 1 && i % 6 != 5) continue;
                if (i % 3 == 0 || i % 5 == 0 || i % 7 == 0) continue;
                dicGroup[sodu].Add(i);
                sodu = (++sodu) % NUM_THREAD;
            }

            for (int i = 0; i < NUM_THREAD; i++)
            {
                int j = i;
                Thread thread = new Thread(() => this.findPrime(dicGroup[j], j));
                thread.Start();
            }

            #endregion

            #endregion

        }
    }
}
